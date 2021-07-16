﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Ferrex
{
    // Token: 0x02000009 RID: 9
    public class Building_PawnCrafter : Building, IThingHolder, IStoreSettingsParent, IPawnCrafter
    {
        // Token: 0x04000016 RID: 22
        protected PawnCrafterProperties crafterProperties;

        // Token: 0x04000011 RID: 17
        public CrafterStatus crafterStatus;

        // Token: 0x04000018 RID: 24
        public int craftingTicksLeft;

        // Token: 0x04000015 RID: 21
        protected CompFlickable flickableComp;

        // Token: 0x04000010 RID: 16
        public ThingOwner<Thing> ingredients = new ThingOwner<Thing>();

        // Token: 0x04000013 RID: 19
        public StorageSettings inputSettings;

        // Token: 0x04000019 RID: 25
        public int nextResourceTick;

        // Token: 0x04000017 RID: 23
        public ThingOrderProcessor orderProcessor;

        // Token: 0x04000012 RID: 18
        public Pawn pawnBeingCrafted;

        // Token: 0x04000014 RID: 20
        protected CompPowerTrader powerComp;

        // Token: 0x06000021 RID: 33 RVA: 0x000027EE File Offset: 0x000009EE
        public virtual void InitiatePawnCrafting()
        {
            pawnBeingCrafted = PawnGenerator.GeneratePawn(crafterProperties.pawnKind, Faction);
            crafterStatus = CrafterStatus.Filling;
        }

        // Token: 0x06000023 RID: 35 RVA: 0x0000283E File Offset: 0x00000A3E
        public virtual void StopPawnCrafting()
        {
            crafterStatus = CrafterStatus.Idle;
            if (pawnBeingCrafted != null)
            {
                pawnBeingCrafted.Destroy();
            }

            pawnBeingCrafted = null;
            ingredients.TryDropAll(InteractionCell, Map, ThingPlaceMode.Near);
        }

        // Token: 0x0600002B RID: 43 RVA: 0x00002EC3 File Offset: 0x000010C3
        public Pawn PawnBeingCrafted()
        {
            return pawnBeingCrafted;
        }

        // Token: 0x0600002C RID: 44 RVA: 0x00002ECB File Offset: 0x000010CB
        public CrafterStatus PawnCrafterStatus()
        {
            return crafterStatus;
        }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000018 RID: 24 RVA: 0x0000254E File Offset: 0x0000074E
        public bool StorageTabVisible => true;

        // Token: 0x06000029 RID: 41 RVA: 0x00002EA9 File Offset: 0x000010A9
        public StorageSettings GetStoreSettings()
        {
            return inputSettings;
        }

        // Token: 0x0600002A RID: 42 RVA: 0x00002EB1 File Offset: 0x000010B1
        public StorageSettings GetParentStoreSettings()
        {
            return def.building.fixedStorageSettings;
        }

        // Token: 0x06000019 RID: 25 RVA: 0x00002551 File Offset: 0x00000751
        public void GetChildHolders(List<IThingHolder> outChildren)
        {
        }

        // Token: 0x0600001A RID: 26 RVA: 0x00002553 File Offset: 0x00000753
        public ThingOwner GetDirectlyHeldThings()
        {
            return ingredients;
        }

        // Token: 0x0600001B RID: 27 RVA: 0x0000255C File Offset: 0x0000075C
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

        // Token: 0x0600001C RID: 28 RVA: 0x0000261C File Offset: 0x0000081C
        public override void PostMake()
        {
            base.PostMake();
            inputSettings = new StorageSettings(this);
            if (def.building.defaultStorageSettings != null)
            {
                inputSettings.CopyFrom(def.building.defaultStorageSettings);
            }
        }

        // Token: 0x0600001D RID: 29 RVA: 0x00002668 File Offset: 0x00000868
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

        // Token: 0x0600001E RID: 30 RVA: 0x000026F3 File Offset: 0x000008F3
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (mode != DestroyMode.Vanish)
            {
                ingredients.TryDropAll(PositionHeld, MapHeld, ThingPlaceMode.Near);
            }

            base.Destroy(mode);
        }

        // Token: 0x0600001F RID: 31 RVA: 0x0000271C File Offset: 0x0000091C
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

        // Token: 0x06000020 RID: 32 RVA: 0x000027B8 File Offset: 0x000009B8
        public virtual bool ReadyToCraft()
        {
            var enumerable = orderProcessor.PendingRequests();
            var isEnumerable = enumerable == null || !enumerable.Any();

            return crafterStatus == CrafterStatus.Filling && isEnumerable;
        }

        // Token: 0x06000022 RID: 34 RVA: 0x00002813 File Offset: 0x00000A13
        public virtual void StartPrinting()
        {
            craftingTicksLeft = crafterProperties.ticksToCraft;
            nextResourceTick = crafterProperties.resourceTick;
            crafterStatus = CrafterStatus.Crafting;
        }

        // Token: 0x06000024 RID: 36 RVA: 0x00002880 File Offset: 0x00000A80
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

        // Token: 0x06000025 RID: 37 RVA: 0x00002920 File Offset: 0x00000B20
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
                crafterProperties.pawnCraftedLetterText.Translate(pawnBeingCrafted.Name), LetterDefOf.PositiveEvent,
                pawnBeingCrafted);
            Find.LetterStack.ReceiveLetter(let);
        }

        // Token: 0x06000026 RID: 38 RVA: 0x00002A24 File Offset: 0x00000C24
        public override string GetInspectString()
        {
            var stringBuilder = new StringBuilder(base.GetInspectString());
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(
                crafterProperties.crafterStatusText.Translate(
                    (crafterProperties.crafterStatusEnumText + (int) crafterStatus).Translate()));
            if (crafterStatus == CrafterStatus.Crafting)
            {
                stringBuilder.AppendLine(crafterProperties.crafterProgressText.Translate(
                    ((crafterProperties.ticksToCraft - (float) craftingTicksLeft) / crafterProperties.ticksToCraft)
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
                stringBuilder.Append(thing.LabelCap + "; ");
            }

            return stringBuilder.ToString().TrimEndNewlines();
        }

        // Token: 0x06000027 RID: 39 RVA: 0x00002C28 File Offset: 0x00000E28
        public override void Tick()
        {
            base.Tick();
            AdjustPowerNeed();
            if (flickableComp != null && (flickableComp == null || !flickableComp.SwitchIsOn))
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
                                    (int) Math.Ceiling(thingOrderRequest.amount /
                                                       (crafterProperties.ticksToCraft /
                                                        (float) crafterProperties.resourceTick)),
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

        // Token: 0x06000028 RID: 40 RVA: 0x00002E20 File Offset: 0x00001020
        public void AdjustPowerNeed()
        {
            if (flickableComp != null && (flickableComp == null || !flickableComp.SwitchIsOn))
            {
                powerComp.PowerOutput = 0f;
                return;
            }

            if (crafterStatus == CrafterStatus.Crafting)
            {
                powerComp.PowerOutput = -powerComp.Props.basePowerConsumption;
                return;
            }

            powerComp.PowerOutput =
                -powerComp.Props.basePowerConsumption * crafterProperties.powerConsumptionFactorIdle;
        }
    }
}