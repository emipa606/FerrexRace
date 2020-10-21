using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Ferrex
{
	// Token: 0x02000003 RID: 3
	public class WorkGiver_PawnCrafter : WorkGiver_Scanner
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForDef(WorkGiverProperties.defToScan);
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x0000206A File Offset: 0x0000026A
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.Touch;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000004 RID: 4 RVA: 0x0000206D File Offset: 0x0000026D
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

		// Token: 0x06000005 RID: 5 RVA: 0x00002090 File Offset: 0x00000290
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
            if (!(t is Building_PawnCrafter building_PawnCrafter) || building_PawnCrafter.crafterStatus != CrafterStatus.Filling)
            {
                return false;
            }
            if (t.IsForbidden(pawn) || !pawn.CanReserveAndReach(t, PathEndMode.Touch, pawn.NormalMaxDanger(), 1, -1, null, forced))
			{
				return false;
			}
			if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
			{
				return false;
			}
			IEnumerable<ThingOrderRequest> enumerable = building_PawnCrafter.orderProcessor.PendingRequests();
			bool result = false;
			if (enumerable != null)
			{
				foreach (ThingOrderRequest request in enumerable)
				{
					if (FindIngredient(pawn, request) != null)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002148 File Offset: 0x00000348
		public override Job JobOnThing(Pawn pawn, Thing crafterThing, bool forced = false)
		{
			Building_PawnCrafter building_PawnCrafter = crafterThing as Building_PawnCrafter;
			IEnumerable<ThingOrderRequest> enumerable = building_PawnCrafter.orderProcessor.PendingRequests();
			if (enumerable != null)
			{
				foreach (ThingOrderRequest thingOrderRequest in enumerable)
				{
					Thing thing = FindIngredient(pawn, thingOrderRequest);
					if (thing != null)
					{
						return new Job(WorkGiverProperties.fillJob, thing, crafterThing)
						{
							count = (int)thingOrderRequest.amount
						};
					}
				}
			}
			return null;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021E0 File Offset: 0x000003E0
		private Thing FindIngredient(Pawn pawn, ThingOrderRequest request)
        {
			if (request != null)
			{
				Predicate<Thing> extraPredicate = request.ExtraPredicate();
                bool validator(Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, -1, null, false) && extraPredicate(x);
                return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, request.Request(), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
			}
			return null;
		}

		// Token: 0x04000003 RID: 3
		private PawnCrafterWorkgiverProperties intWorkGiverProperties;
	}
}
