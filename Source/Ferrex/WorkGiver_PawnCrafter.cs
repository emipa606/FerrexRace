using RimWorld;
using Verse;
using Verse.AI;

namespace Ferrex;

public class WorkGiver_PawnCrafter : WorkGiver_Scanner
{
    private PawnCrafterWorkgiverProperties intWorkGiverProperties;

    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(WorkGiverProperties.defToScan);

    public override PathEndMode PathEndMode => PathEndMode.Touch;

    public PawnCrafterWorkgiverProperties WorkGiverProperties
    {
        get
        {
            if (intWorkGiverProperties == null)
            {
                intWorkGiverProperties = def.GetModExtension<PawnCrafterWorkgiverProperties>();
            }

            return intWorkGiverProperties;
        }
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Building_PawnCrafter { crafterStatus: CrafterStatus.Filling } building_PawnCrafter)
        {
            return false;
        }

        if (t.IsForbidden(pawn) ||
            !pawn.CanReserveAndReach(t, PathEndMode.Touch, pawn.NormalMaxDanger(), 1, -1, null, forced))
        {
            return false;
        }

        if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
        {
            return false;
        }

        var enumerable = building_PawnCrafter.orderProcessor.PendingRequests();
        var result = false;
        if (enumerable == null)
        {
            return false;
        }

        foreach (var request in enumerable)
        {
            if (FindIngredient(pawn, request) == null)
            {
                continue;
            }

            result = true;
            break;
        }

        return result;
    }

    public override Job JobOnThing(Pawn pawn, Thing crafterThing, bool forced = false)
    {
        var building_PawnCrafter = crafterThing as Building_PawnCrafter;
        var enumerable = building_PawnCrafter?.orderProcessor.PendingRequests();
        if (enumerable == null)
        {
            return null;
        }

        foreach (var thingOrderRequest in enumerable)
        {
            var thing = FindIngredient(pawn, thingOrderRequest);
            if (thing != null)
            {
                return new Job(WorkGiverProperties.fillJob, thing, crafterThing)
                {
                    count = (int)thingOrderRequest.amount
                };
            }
        }

        return null;
    }

    private Thing FindIngredient(Pawn pawn, ThingOrderRequest request)
    {
        if (request == null)
        {
            return null;
        }

        var extraPredicate = request.ExtraPredicate();

        bool validator(Thing x)
        {
            return !x.IsForbidden(pawn) && pawn.CanReserve(x) && extraPredicate(x);
        }

        return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, request.Request(),
            PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, validator);
    }
}