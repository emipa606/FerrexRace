using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace Ferrex
{
    // Token: 0x0200000B RID: 11
    public class CustomizePawnWindow : Window
    {
        // Token: 0x04000040 RID: 64
        private static readonly Vector2 PawnPortraitSize = new Vector2(100f, 140f);

        // Token: 0x04000041 RID: 65
        private static readonly SimpleCurve LevelRandomCurve = new SimpleCurve
        {
            new CurvePoint(0f, 0f),
            new CurvePoint(0.5f, 150f),
            new CurvePoint(4f, 150f),
            new CurvePoint(5f, 25f),
            new CurvePoint(10f, 5f),
            new CurvePoint(15f, 0f)
        };

        // Token: 0x04000042 RID: 66
        private static readonly SimpleCurve LevelFinalAdjustmentCurve = new SimpleCurve
        {
            new CurvePoint(0f, 0f),
            new CurvePoint(10f, 10f),
            new CurvePoint(20f, 16f),
            new CurvePoint(27f, 20f)
        };

        // Token: 0x0400003C RID: 60
        public Backstory newAdulthoodBackstory;

        // Token: 0x0400003B RID: 59
        public Backstory newChildhoodBackstory;

        // Token: 0x0400003A RID: 58
        public Pawn newPawn;

        // Token: 0x0400003D RID: 61
        public Pair<int, Trait> newTrait;

        // Token: 0x04000039 RID: 57
        public Building_PawnCrafter pawnCrafter;

        // Token: 0x0400003F RID: 63
        public int traitPos = -1;

        // Token: 0x0400003E RID: 62
        public bool traitsChanged;

        // Token: 0x06000031 RID: 49 RVA: 0x00003054 File Offset: 0x00001254
        public CustomizePawnWindow(Building_PawnCrafter pawnCrafter)
        {
            this.pawnCrafter = pawnCrafter;
            newPawn = GetNewPawn();
        }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000030 RID: 48 RVA: 0x00003043 File Offset: 0x00001243
        public override Vector2 InitialSize => new Vector2(640f, 480f);

        // Token: 0x06000032 RID: 50 RVA: 0x00003078 File Offset: 0x00001278
        public override void DoWindowContents(Rect inRect)
        {
            var crafterProperties = pawnCrafter.def.GetModExtension<PawnCrafterProperties>();
            if (newChildhoodBackstory != null)
            {
                newPawn.story.childhood = newChildhoodBackstory;
                newChildhoodBackstory = null;
                RefreshPawn();
            }

            if (newAdulthoodBackstory != null)
            {
                newPawn.story.adulthood = newAdulthoodBackstory;
                newAdulthoodBackstory = null;
                RefreshPawn();
            }

            if (traitsChanged)
            {
                var first = newTrait.First;
                var second = newTrait.Second;
                if (first < newPawn.story.traits.allTraits.Count)
                {
                    newPawn.story.traits.allTraits.RemoveAt(first);
                    newPawn.story.traits.allTraits.Insert(first, second);
                    if (newPawn.workSettings != null)
                    {
                        newPawn.workSettings.Notify_DisabledWorkTypesChanged();
                    }

                    if (newPawn.skills != null)
                    {
                        newPawn.skills.Notify_SkillDisablesChanged();
                    }

                    if (!newPawn.Dead && newPawn.RaceProps.Humanlike)
                    {
                        newPawn.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();
                    }
                }
                else
                {
                    newPawn.story.traits.GainTrait(second);
                }

                newTrait = default;
                traitsChanged = false;
                traitPos = -1;
                RefreshPawn();
            }

            var rect = new Rect(inRect)
            {
                width = PawnPortraitSize.x + 16f,
                height = PawnPortraitSize.y + 16f
            };
            rect = rect.CenteredOnXIn(inRect);
            rect = rect.CenteredOnYIn(inRect);
            rect.x += 16f;
            rect.y += 16f;
            if (newPawn != null)
            {
                var rect2 = new Rect(rect.xMin + ((rect.width - PawnPortraitSize.x) / 2f) - 10f, rect.yMin + 20f,
                    PawnPortraitSize.x, PawnPortraitSize.y);
                GUI.DrawTexture(rect2, PortraitsCache.Get(newPawn, PawnPortraitSize, Rot4.South));
                Widgets.InfoCardButton(rect2.xMax - 16f, rect2.y, newPawn);
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(new Rect(0f, 0f, inRect.width, 32f),
                    crafterProperties.crafterPawnCustomizationTitle.Translate());
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleLeft;
                var num = 32f;
                var rect3 = new Rect(32f, num, 240f, 24f);
                if (newPawn.Name is NameTriple nameTriple)
                {
                    var rect4 = new Rect(rect3);
                    rect4.width *= 0.333f;
                    var rect5 = new Rect(rect3);
                    rect5.width *= 0.333f;
                    rect5.x += rect5.width;
                    var rect6 = new Rect(rect3);
                    rect6.width *= 0.333f;
                    rect6.x += rect5.width * 2f;
                    var first2 = nameTriple.First;
                    var nick = nameTriple.Nick;
                    var last = nameTriple.Last;
                    CharacterCardUtility.DoNameInputRect(rect4, ref first2, 12);
                    if (nameTriple.Nick == nameTriple.First || nameTriple.Nick == nameTriple.Last)
                    {
                        GUI.color = new Color(1f, 1f, 1f, 0.5f);
                    }

                    CharacterCardUtility.DoNameInputRect(rect5, ref nick, 9);
                    GUI.color = Color.white;
                    CharacterCardUtility.DoNameInputRect(rect6, ref last, 12);
                    if (nameTriple.First != first2 || nameTriple.Nick != nick || nameTriple.Last != last)
                    {
                        newPawn.Name = new NameTriple(first2, nick, last);
                    }

                    TooltipHandler.TipRegion(rect4, "FirstNameDesc".Translate());
                    TooltipHandler.TipRegion(rect5, "ShortIdentifierDesc".Translate());
                    TooltipHandler.TipRegion(rect6, "LastNameDesc".Translate());
                }
                else
                {
                    rect3.width = 999f;
                    Text.Font = GameFont.Medium;
                    Widgets.Label(rect3, newPawn.Name.ToStringFull);
                    Text.Font = GameFont.Small;
                }

                var rect7 = new Rect(rect.x + rect.width + 16f, rect.y + 32f,
                    inRect.width - (rect.x + rect.width + 16f), 32f);
                Text.Font = GameFont.Medium;
                if (Widgets.ButtonText(rect7, crafterProperties.crafterPawnCustomizationCraftButton.Translate(), true,
                    false))
                {
                    pawnCrafter.pawnBeingCrafted = newPawn;
                    pawnCrafter.crafterStatus = CrafterStatus.Filling;
                    Close();
                }

                Text.Font = GameFont.Small;
                if (Widgets.ButtonText(new Rect(304f, num, 120f, 24f),
                    crafterProperties.crafterPawnCustomizationCraftRollFemaleButton.Translate(), true, false))
                {
                    newPawn.Destroy();
                    newPawn = GetNewPawn();
                }

                if (Widgets.ButtonText(new Rect(424f, num, 120f, 24f),
                    crafterProperties.crafterPawnCustomizationCraftRollMaleButton.Translate(), true, false))
                {
                    newPawn.Destroy();
                    newPawn = GetNewPawn(Gender.Male);
                }

                num += 26f;
                var rect8 = new Rect(32f, num, 240f, 24f);
                Widgets.DrawBox(rect8);
                Widgets.DrawHighlightIfMouseover(rect8);
                string text;
                if (newPawn.story.childhood != null)
                {
                    text = crafterProperties.crafterPawnCustomizationCraftChildhoodBackstoryButton.Translate() + " " +
                           newPawn.story.childhood.titleShort;
                }
                else
                {
                    text = crafterProperties.crafterPawnCustomizationCraftChildhoodBackstoryButton.Translate() + " " +
                           crafterProperties.crafterPawnCustomizationNone.Translate();
                }

                if (Widgets.ButtonText(rect8, text, true, false))
                {
                    FloatMenuUtility.MakeMenu(from backstory in BackstoryDatabase.allBackstories.Select(
                            delegate(KeyValuePair<string, Backstory> backstoryPair)
                            {
                                var keyValuePair = backstoryPair;
                                return keyValuePair.Value;
                            })
                        where backstory.spawnCategories.Contains(crafterProperties
                            .crafterPawnCustomizationPawnBackstoryTag) && backstory.slot == BackstorySlot.Childhood
                        select backstory, backstory => backstory.titleShort,
                        backstory => delegate { newChildhoodBackstory = backstory; });
                }

                if (newPawn.story.childhood != null)
                {
                    TooltipHandler.TipRegion(rect8, newPawn.story.childhood.FullDescriptionFor(newPawn));
                }

                var rect9 = new Rect(304f, num, 240f, 24f);
                Widgets.DrawBox(rect9);
                Widgets.DrawHighlightIfMouseover(rect9);
                string text2;
                if (newPawn.story.adulthood != null)
                {
                    text2 = crafterProperties.crafterPawnCustomizationCraftAdulthoodBackstoryButton.Translate() + " " +
                            newPawn.story.adulthood.titleShort;
                }
                else
                {
                    text2 = crafterProperties.crafterPawnCustomizationCraftAdulthoodBackstoryButton.Translate() + " " +
                            crafterProperties.crafterPawnCustomizationNone.Translate();
                }

                if (Widgets.ButtonText(rect9, text2, true, false))
                {
                    FloatMenuUtility.MakeMenu(from backstory in BackstoryDatabase.allBackstories.Select(
                            delegate(KeyValuePair<string, Backstory> backstoryPair)
                            {
                                var keyValuePair = backstoryPair;
                                return keyValuePair.Value;
                            })
                        where backstory.spawnCategories.Contains(crafterProperties
                            .crafterPawnCustomizationPawnBackstoryTag) && backstory.slot == BackstorySlot.Adulthood
                        select backstory, backstory => backstory.titleShort,
                        backstory => delegate { newAdulthoodBackstory = backstory; });
                }

                if (newPawn.story.adulthood != null)
                {
                    TooltipHandler.TipRegion(rect9, newPawn.story.adulthood.FullDescriptionFor(newPawn));
                }

                num += 48f;
                var vector = new Vector2(32f, num);
                SkillUI.DrawSkillsOf(newPawn, vector, SkillUI.SkillDrawMode.Gameplay);
                num = rect.y + rect.height;
                var num2 = rect.x + 24f;
                Text.Anchor = TextAnchor.MiddleCenter;
                foreach (var trait in newPawn.story.traits.allTraits)
                {
                    var rect10 = new Rect(num2, num, 256f, 24f);
                    Widgets.DrawBox(rect10);
                    Widgets.DrawHighlightIfMouseover(rect10);
                    Widgets.Label(rect10, trait.LabelCap);
                    TooltipHandler.TipRegion(rect10, trait.TipString(newPawn));
                    num += 26f;
                }
            }

            Text.Anchor = 0;
        }

        // Token: 0x06000033 RID: 51 RVA: 0x00003A50 File Offset: 0x00001C50
        public virtual void RefreshPawn()
        {
            typeof(Pawn_StoryTracker)
                .GetField("cachedDisabledWorkTypes", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(newPawn.story, null);
            newPawn.skills.Notify_SkillDisablesChanged();
            var allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
            foreach (var skillDef in allDefsListForReading)
            {
                var num = FinalLevelOfSkill(newPawn, skillDef);
                var skill = newPawn.skills.GetSkill(skillDef);
                skill.Level = num;
                skill.passion = Passion.None;
                if (skill.TotallyDisabled)
                {
                    continue;
                }

                var num2 = num * 0.11f;
                var value = Rand.Value;
                if (value < num2)
                {
                    skill.passion = value < num2 * 0.2f ? Passion.Major : Passion.Minor;
                }

                skill.xpSinceLastLevel =
                    Rand.Range(skill.XpRequiredForLevelUp * 0.1f, skill.XpRequiredForLevelUp * 0.9f);
            }
        }

        // Token: 0x06000034 RID: 52 RVA: 0x00003B50 File Offset: 0x00001D50
        protected virtual int FinalLevelOfSkill(Pawn pawn, SkillDef sk)
        {
            float num;
            if (sk.usuallyDefinedInBackstories)
            {
                num = Rand.RangeInclusive(0, 4);
            }
            else
            {
                num = Rand.ByCurve(LevelRandomCurve);
            }

            foreach (var backstory in from bs in pawn.story.AllBackstories
                where bs != null
                select bs)
            {
                foreach (var keyValuePair in backstory.skillGainsResolved)
                {
                    if (keyValuePair.Key == sk)
                    {
                        num += keyValuePair.Value * Rand.Range(1f, 1.4f);
                    }
                }
            }

            foreach (var trait in pawn.story.traits.allTraits)
            {
                if (trait.CurrentData.skillGains.TryGetValue(sk, out var num2))
                {
                    num += num2;
                }
            }

            num = LevelFinalAdjustmentCurve.Evaluate(num);
            return Mathf.Clamp(Mathf.RoundToInt(num), 0, 20);
        }

        // Token: 0x06000035 RID: 53 RVA: 0x00003CB0 File Offset: 0x00001EB0
        public virtual Pawn GetNewPawn(Gender gender = Gender.Female)
        {
            var modExtension = pawnCrafter.def.GetModExtension<PawnCrafterProperties>();
            var pawnKindDef = modExtension == null ? PawnKindDef.Named("Human") : modExtension.pawnKind;

            var pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnKindDef, pawnCrafter.Faction,
                PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, false, 1f, false, true, true,
                false, false, false, false, false, 0, 0, null, 1, null, null, null, null, null, null, null, gender));
            if (pawn.workSettings != null)
            {
                pawn.workSettings.Notify_DisabledWorkTypesChanged();
            }

            if (pawn.skills != null)
            {
                pawn.skills.Notify_SkillDisablesChanged();
            }

            if (!pawn.Dead && pawn.RaceProps.Humanlike)
            {
                pawn.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();
            }

            return pawn;
        }
    }
}