using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Ferrex;

public class Building_PawnCrafter : Building, IThingHolder, IStoreSettingsParent, IPawnCrafter
{
    protected PawnCrafterProperties crafterProperties;

    public CrafterStatus crafterStatus;

    public int craftingTicksLeft;

    protected CompFlickable flickableComp;

    public ThingOwner<Thing> ingredients = new ThingOwner<Thing>();

    public StorageSettings inputSettings;

    public int nextResourceTick;

    public ThingOrderProcessor orderProcessor;

    public Pawn pawnBeingCrafted;

    protected CompPowerTrader powerComp;

    public virtual void InitiatePawnCrafting()
    {
        pawnBeingCrafted = PawnGenerator.GeneratePawn(crafterProperties.pawnKind, Faction);
        crafterStatus = CrafterStatus.Filling;
    }

    public virtual void StopPawnCrafting()
    {
        crafterStatus = CrafterStatus.Idle;
        pawnBeingCrafted?.Destroy();

        pawnBeingCrafted = null;
        ingredients.TryDropAll(InteractionCell, Map, ThingPlaceMode.Near);
    }

    public Pawn PawnBeingCrafted()
    {
        return pawnBeingCrafted;
    }

    public CrafterStatus PawnCrafterStatus()
    {
        return crafterStatus;
    }

    public void Notify_SettingsChanged()
    {
    }

    public bool StorageTabVisible => true;

    public StorageSettings GetStoreSettings()
    {
        return inputSettings;
    }

    public StorageSettings GetParentStoreSettings()
    {
        return def.building.fixedStorageSettings;
    }

    public void GetChildHolders(List<IThingHolder> outChildren)
    {
    }

    public ThingOwner GetDirectlyHeldThings()
    {
        return ingredients;
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        powerComp = GetComp<CompPowerTrader>();
        flickableComp = GetComp<CompFlickable>();
        if (inputSettings == null)
        {
            inputSettings = new StorageSettings(this);
            if (def.building.defaultStorageSettings != null)
            {
                inputSettings.CopyFrom(def.building.defaultStorageSettings);
            }
        }

        crafterProperties = def.GetModExtension<PawnCrafterProperties>();
        orderProcessor = new ThingOrderProcessor(ingredients, inputSettings);
        if (crafterProperties != null)
        {
            orderProcessor.requestedItems.AddRange(crafterProperties.costList);
        }

        AdjustPowerNeed();
    }

    public override void PostMake()
    {
        base.PostMake();
        inputSettings = new StorageSettings(this);
        if (def.building.defaultStorageSettings != null)
        {
            inputSettings.CopyFrom(def.building.defaultStorageSettings);
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Deep.Look(ref ingredients, "ingredients");
        Scribe_Values.Look(ref crafterStatus, "crafterStatus");
        Scribe_Values.Look(ref craftingTicksLeft, "craftingTicksLeft");
        Scribe_Values.Look(ref nextResourceTick, "nextResourceTick");
        Scribe_Deep.Look(ref pawnBeingCrafted, "pawnBeingCrafted");
        Scribe_Deep.Look(ref inputSettings, "inputSettings");
    }

    public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
    {
        if (mode != DestroyMode.Vanish)
        {
            ingredients.TryDropAll(PositionHeld, MapHeld, ThingPlaceMode.Near);
        }

        base.Destroy(mode);
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        var list = new List<Gizmo>(base.GetGizmos());
        if (pawnBeingCrafted != null)
        {
            list.Insert(0, new Gizmo_CrafterPawnInfo(this)
            {
                description = crafterProperties.crafterPawnGizmoPawnInfoDescription
            });
        }

        if (crafterStatus != CrafterStatus.Finished)
        {
            list.Insert(0, new Gizmo_ToggleCrafting(this)
            {
                labelStart = crafterProperties.crafterPawnGizmoStartCraftLabel,
                descriptionStart = crafterProperties.crafterPawnGizmoStartCraftDescription,
                labelStop = crafterProperties.crafterPawnGizmoStopCraftLabel,
                descriptionStop = crafterProperties.crafterPawnGizmoStopCraftDescription
            });
        }

        return list;
    }

    public virtual bool ReadyToCraft()
    {
        var enumerable = orderProcessor.PendingRequests();
        var isEnumerable = enumerable == null || !enumerable.Any();

        return crafterStatus == CrafterStatus.Filling && isEnumerable;
    }

    public virtual void StartPrinting()
    {
        craftingTicksLeft = crafterProperties.ticksToCraft;
        nextResourceTick = crafterProperties.resourceTick;
        crafterStatus = CrafterStatus.Crafting;
    }

    public virtual void ExtraCrafterTickAction()
    {
        var status = crafterStatus;
        if (status != CrafterStatus.Filling)
        {
            if (status != CrafterStatus.Crafting)
            {
                return;
            }

            if (powerComp.PowerOn && Current.Game.tickManager.TicksGame % 100 == 0)
            {
                FleckMaker.ThrowSmoke(Position.ToVector3(), Map, 1.33f);
            }
        }
        else if (powerComp.PowerOn && Current.Game.tickManager.TicksGame % 300 == 0)
        {
            FleckMaker.ThrowSmoke(Position.ToVector3(), Map, 1f);
        }
    }

    public virtual void FinishAction()
    {
        GenSpawn.Spawn(pawnBeingCrafted, InteractionCell, Map);
        if (crafterProperties.hediffOnPawnCrafted != null)
        {
            pawnBeingCrafted.health.AddHediff(crafterProperties.hediffOnPawnCrafted);
        }

        if (crafterProperties.thoughtOnPawnCrafted != null)
        {
            pawnBeingCrafted.needs.mood.thoughts.memories.TryGainMemory(crafterProperties.thoughtOnPawnCrafted);
        }

        var let = LetterMaker.MakeLetter(
            crafterProperties.pawnCraftedLetterLabel.Translate(pawnBeingCrafted.Name.ToStringShort),
            crafterProperties.pawnCraftedLetterText.Translate(pawnBeingCrafted.NameFullColored),
            LetterDefOf.PositiveEvent,
            pawnBeingCrafted);
        Find.LetterStack.ReceiveLetter(let);
    }

    public override string GetInspectString()
    {
        var stringBuilder = new StringBuilder(base.GetInspectString());
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(
            crafterProperties.crafterStatusText.Translate(
                (crafterProperties.crafterStatusEnumText + (int)crafterStatus).Translate()));
        if (crafterStatus == CrafterStatus.Crafting)
        {
            stringBuilder.AppendLine(crafterProperties.crafterProgressText.Translate(
                ((crafterProperties.ticksToCraft - (float)craftingTicksLeft) / crafterProperties.ticksToCraft)
                .ToStringPercent()));
        }

        if (crafterStatus == CrafterStatus.Filling)
        {
            var continueCrafting = true;
            foreach (var thingOrderRequest in orderProcessor.requestedItems)
            {
                var num = ingredients.TotalStackCountOfDef(thingOrderRequest.thingDef);
                if (!(num < thingOrderRequest.amount))
                {
                    continue;
                }

                stringBuilder.Append(
                    crafterProperties.crafterMaterialNeedText.Translate(thingOrderRequest.amount - num,
                        thingOrderRequest.thingDef.LabelCap) + " ");
                continueCrafting = false;
            }

            if (!continueCrafting)
            {
                stringBuilder.AppendLine();
            }
        }

        if (ingredients.Count > 0)
        {
            stringBuilder.Append(crafterProperties.crafterMaterialsText.Translate() + " ");
        }

        foreach (var thing in ingredients)
        {
            stringBuilder.Append($"{thing.LabelCap}; ");
        }

        return stringBuilder.ToString().TrimEndNewlines();
    }

    public override void Tick()
    {
        base.Tick();
        AdjustPowerNeed();
        if (flickableComp != null && flickableComp is not { SwitchIsOn: true })
        {
            return;
        }

        switch (crafterStatus)
        {
            case CrafterStatus.Filling:
            {
                ExtraCrafterTickAction();
                var enumerable = orderProcessor.PendingRequests();
                var isEnumerable = enumerable == null || !enumerable.Any();

                if (isEnumerable)
                {
                    StartPrinting();
                }

                break;
            }
            case CrafterStatus.Crafting:
                ExtraCrafterTickAction();
                if (powerComp.PowerOn)
                {
                    nextResourceTick--;
                    if (nextResourceTick <= 0)
                    {
                        nextResourceTick = crafterProperties.resourceTick;
                        using var enumerator = orderProcessor.requestedItems.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            var thingOrderRequest = enumerator.Current;
                            if (ingredients.All(thing => thing.def != thingOrderRequest?.thingDef))
                            {
                                continue;
                            }

                            var thing2 = ingredients.First(thing =>
                                thing.def == thingOrderRequest?.thingDef);

                            if (thingOrderRequest == null)
                            {
                                continue;
                            }

                            var count = Math.Min(
                                (int)Math.Ceiling(thingOrderRequest.amount /
                                                  (crafterProperties.ticksToCraft /
                                                   (float)crafterProperties.resourceTick)),
                                thing2.stackCount);
                            ingredients.Take(thing2, count).Destroy();
                        }
                    }

                    if (craftingTicksLeft > 0)
                    {
                        craftingTicksLeft--;
                        return;
                    }

                    crafterStatus = CrafterStatus.Finished;
                }

                break;
            case CrafterStatus.Finished:
                if (pawnBeingCrafted != null)
                {
                    ExtraCrafterTickAction();
                    ingredients.ClearAndDestroyContents();
                    FinishAction();
                    pawnBeingCrafted = null;
                    crafterStatus = CrafterStatus.Idle;
                }

                break;
            default:
                return;
        }
    }

    public void AdjustPowerNeed()
    {
        if (flickableComp != null && flickableComp is not { SwitchIsOn: true })
        {
            powerComp.PowerOutput = 0f;
            return;
        }

        if (crafterStatus == CrafterStatus.Crafting)
        {
            powerComp.PowerOutput = -powerComp.Props.PowerConsumption;
            return;
        }

        powerComp.PowerOutput =
            -powerComp.Props.PowerConsumption * crafterProperties.powerConsumptionFactorIdle;
    }
}