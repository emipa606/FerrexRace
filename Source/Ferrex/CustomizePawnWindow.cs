using System;
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
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00003043 File Offset: 0x00001243
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(640f, 480f);
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003054 File Offset: 0x00001254
		public CustomizePawnWindow(Building_PawnCrafter pawnCrafter)
		{
			this.pawnCrafter = pawnCrafter;
			newPawn = GetNewPawn(Gender.Female);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003078 File Offset: 0x00001278
		public override void DoWindowContents(Rect inRect)
		{
			PawnCrafterProperties crafterProperties = pawnCrafter.def.GetModExtension<PawnCrafterProperties>();
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
				int first = newTrait.First;
				Trait second = newTrait.Second;
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
            Rect rect = new Rect(inRect)
            {
                width = CustomizePawnWindow.PawnPortraitSize.x + 16f,
                height = CustomizePawnWindow.PawnPortraitSize.y + 16f
            };
            rect = GenUI.CenteredOnXIn(rect, inRect);
			rect = GenUI.CenteredOnYIn(rect, inRect);
			rect.x += 16f;
			rect.y += 16f;
			if (newPawn != null)
			{
				Rect rect2 = new Rect(rect.xMin + (rect.width - CustomizePawnWindow.PawnPortraitSize.x) / 2f - 10f, rect.yMin + 20f, CustomizePawnWindow.PawnPortraitSize.x, CustomizePawnWindow.PawnPortraitSize.y);
				GUI.DrawTexture(rect2, PortraitsCache.Get(newPawn, CustomizePawnWindow.PawnPortraitSize, default, 1f));
				Widgets.InfoCardButton(rect2.xMax - 16f, rect2.y, newPawn);
				Text.Font = GameFont.Medium;
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.Label(new Rect(0f, 0f, inRect.width, 32f), Translator.Translate(crafterProperties.crafterPawnCustomizationTitle));
				Text.Font = GameFont.Small;
				Text.Anchor = TextAnchor.MiddleLeft;
				float num = 32f;
				Rect rect3 = new Rect(32f, num, 240f, 24f);
                if (newPawn.Name is NameTriple nameTriple)
                {
                    Rect rect4 = new Rect(rect3);
                    rect4.width *= 0.333f;
                    Rect rect5 = new Rect(rect3);
                    rect5.width *= 0.333f;
                    rect5.x += rect5.width;
                    Rect rect6 = new Rect(rect3);
                    rect6.width *= 0.333f;
                    rect6.x += rect5.width * 2f;
                    string first2 = nameTriple.First;
                    string nick = nameTriple.Nick;
                    string last = nameTriple.Last;
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
                    TooltipHandler.TipRegion(rect4, Translator.Translate("FirstNameDesc"));
                    TooltipHandler.TipRegion(rect5, Translator.Translate("ShortIdentifierDesc"));
                    TooltipHandler.TipRegion(rect6, Translator.Translate("LastNameDesc"));
                }
                else
                {
                    rect3.width = 999f;
                    Text.Font = GameFont.Medium;
                    Widgets.Label(rect3, newPawn.Name.ToStringFull);
                    Text.Font = GameFont.Small;
                }
                Rect rect7 = new Rect(rect.x + rect.width + 16f, rect.y + 32f, inRect.width - (rect.x + rect.width + 16f), 32f);
				Text.Font = GameFont.Medium;
				if (Widgets.ButtonText(rect7, Translator.Translate(crafterProperties.crafterPawnCustomizationCraftButton), true, false, true))
				{
					pawnCrafter.pawnBeingCrafted = newPawn;
					pawnCrafter.crafterStatus = CrafterStatus.Filling;
					Close(true);
				}
				Text.Font = GameFont.Small;
				if (Widgets.ButtonText(new Rect(304f, num, 120f, 24f), Translator.Translate(crafterProperties.crafterPawnCustomizationCraftRollFemaleButton), true, false, true))
				{
					newPawn.Destroy(DestroyMode.Vanish);
					newPawn = GetNewPawn(Gender.Female);
				}
				if (Widgets.ButtonText(new Rect(424f, num, 120f, 24f), Translator.Translate(crafterProperties.crafterPawnCustomizationCraftRollMaleButton), true, false, true))
				{
					newPawn.Destroy(DestroyMode.Vanish);
					newPawn = GetNewPawn(Gender.Male);
				}
				num += 26f;
				Rect rect8 = new Rect(32f, num, 240f, 24f);
				Widgets.DrawBox(rect8, 1);
				Widgets.DrawHighlightIfMouseover(rect8);
				string text;
				if (newPawn.story.childhood != null)
				{
					text = Translator.Translate(crafterProperties.crafterPawnCustomizationCraftChildhoodBackstoryButton) + " " + newPawn.story.childhood.titleShort;
				}
				else
				{
					text = Translator.Translate(crafterProperties.crafterPawnCustomizationCraftChildhoodBackstoryButton) + " " + Translator.Translate(crafterProperties.crafterPawnCustomizationNone);
				}
				if (Widgets.ButtonText(rect8, text, true, false, true))
				{
					FloatMenuUtility.MakeMenu<Backstory>(from backstory in BackstoryDatabase.allBackstories.Select(delegate(KeyValuePair<string, Backstory> backstoryPair)
					{
						KeyValuePair<string, Backstory> keyValuePair = backstoryPair;
						return keyValuePair.Value;
					})
					where backstory.spawnCategories.Contains(crafterProperties.crafterPawnCustomizationPawnBackstoryTag) && backstory.slot == BackstorySlot.Childhood
					select backstory, (Backstory backstory) => backstory.titleShort, (Backstory backstory) => delegate()
					{
						newChildhoodBackstory = backstory;
					});
				}
				if (newPawn.story.childhood != null)
				{
					TooltipHandler.TipRegion(rect8, newPawn.story.childhood.FullDescriptionFor(newPawn));
				}
				Rect rect9 = new Rect(304f, num, 240f, 24f);
				Widgets.DrawBox(rect9, 1);
				Widgets.DrawHighlightIfMouseover(rect9);
				string text2;
				if (newPawn.story.adulthood != null)
				{
					text2 = Translator.Translate(crafterProperties.crafterPawnCustomizationCraftAdulthoodBackstoryButton) + " " + newPawn.story.adulthood.titleShort;
				}
				else
				{
					text2 = Translator.Translate(crafterProperties.crafterPawnCustomizationCraftAdulthoodBackstoryButton) + " " + Translator.Translate(crafterProperties.crafterPawnCustomizationNone);
				}
				if (Widgets.ButtonText(rect9, text2, true, false, true))
				{
					FloatMenuUtility.MakeMenu<Backstory>(from backstory in BackstoryDatabase.allBackstories.Select(delegate(KeyValuePair<string, Backstory> backstoryPair)
					{
						KeyValuePair<string, Backstory> keyValuePair = backstoryPair;
						return keyValuePair.Value;
					})
					where backstory.spawnCategories.Contains(crafterProperties.crafterPawnCustomizationPawnBackstoryTag) && backstory.slot == BackstorySlot.Adulthood
					select backstory, (Backstory backstory) => backstory.titleShort, (Backstory backstory) => delegate()
					{
						newAdulthoodBackstory = backstory;
					});
				}
				if (newPawn.story.adulthood != null)
				{
					TooltipHandler.TipRegion(rect9, newPawn.story.adulthood.FullDescriptionFor(newPawn));
				}
				num += 48f;
				Vector2 vector = new Vector2(32f, num);
				SkillUI.DrawSkillsOf(newPawn, vector, SkillUI.SkillDrawMode.Gameplay);
				num = rect.y + rect.height;
				float num2 = rect.x + 24f;
				Text.Anchor = TextAnchor.MiddleCenter;
				for (int i = 0; i < newPawn.story.traits.allTraits.Count; i++)
				{
					Rect rect10 = new Rect(num2, num, 256f, 24f);
					Widgets.DrawBox(rect10, 1);
					Widgets.DrawHighlightIfMouseover(rect10);
					Widgets.Label(rect10, newPawn.story.traits.allTraits[i].LabelCap);
					TooltipHandler.TipRegion(rect10, newPawn.story.traits.allTraits[i].TipString(newPawn));
					num += 26f;
				}
			}
			Text.Anchor = 0;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003A50 File Offset: 0x00001C50
		public virtual void RefreshPawn()
		{
			typeof(Pawn_StoryTracker).GetField("cachedDisabledWorkTypes", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(newPawn.story, null);
			newPawn.skills.Notify_SkillDisablesChanged();
			List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
			for (int i = 0; i < allDefsListForReading.Count; i++)
			{
				SkillDef skillDef = allDefsListForReading[i];
				int num = FinalLevelOfSkill(newPawn, skillDef);
				SkillRecord skill = newPawn.skills.GetSkill(skillDef);
				skill.Level = num;
				skill.passion = Passion.None;
				if (!skill.TotallyDisabled)
				{
					float num2 = (float)num * 0.11f;
					float value = Rand.Value;
					if (value < num2)
					{
						if (value < num2 * 0.2f)
						{
							skill.passion = Passion.Major;
						}
						else
						{
							skill.passion = Passion.Minor;
						}
					}
					skill.xpSinceLastLevel = Rand.Range(skill.XpRequiredForLevelUp * 0.1f, skill.XpRequiredForLevelUp * 0.9f);
				}
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00003B50 File Offset: 0x00001D50
		protected virtual int FinalLevelOfSkill(Pawn pawn, SkillDef sk)
		{
			float num;
			if (sk.usuallyDefinedInBackstories)
			{
				num = (float)Rand.RangeInclusive(0, 4);
			}
			else
			{
				num = Rand.ByCurve(CustomizePawnWindow.LevelRandomCurve);
			}
			foreach (Backstory backstory in from bs in pawn.story.AllBackstories
			where bs != null
			select bs)
			{
				foreach (KeyValuePair<SkillDef, int> keyValuePair in backstory.skillGainsResolved)
				{
					if (keyValuePair.Key == sk)
					{
						num += (float)keyValuePair.Value * Rand.Range(1f, 1.4f);
					}
				}
			}
			for (int i = 0; i < pawn.story.traits.allTraits.Count; i++)
			{
                if (pawn.story.traits.allTraits[i].CurrentData.skillGains.TryGetValue(sk, out int num2))
                {
                    num += (float)num2;
                }
            }
			num = CustomizePawnWindow.LevelFinalAdjustmentCurve.Evaluate(num);
			return Mathf.Clamp(Mathf.RoundToInt(num), 0, 20);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003CB0 File Offset: 0x00001EB0
		public virtual Pawn GetNewPawn(Gender gender = Gender.Female)
		{
			PawnCrafterProperties modExtension = pawnCrafter.def.GetModExtension<PawnCrafterProperties>();
			PawnKindDef pawnKindDef;
			if (modExtension == null)
			{
				pawnKindDef = PawnKindDef.Named("Human");
			}
			else
			{
				pawnKindDef = modExtension.pawnKind;
			}
			Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnKindDef, pawnCrafter.Faction, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, false, 1f, false, true, true, false, false, false, false, false, 0, null, 1, null, null, null, null, null, null, null, new Gender?(gender), null, null));
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

		// Token: 0x04000039 RID: 57
		public Building_PawnCrafter pawnCrafter;

		// Token: 0x0400003A RID: 58
		public Pawn newPawn;

		// Token: 0x0400003B RID: 59
		public Backstory newChildhoodBackstory;

		// Token: 0x0400003C RID: 60
		public Backstory newAdulthoodBackstory;

		// Token: 0x0400003D RID: 61
		public Pair<int, Trait> newTrait;

		// Token: 0x0400003E RID: 62
		public bool traitsChanged;

		// Token: 0x0400003F RID: 63
		public int traitPos = -1;

		// Token: 0x04000040 RID: 64
		private static readonly Vector2 PawnPortraitSize = new Vector2(100f, 140f);

		// Token: 0x04000041 RID: 65
		private static readonly SimpleCurve LevelRandomCurve = new SimpleCurve
		{
			{
				new CurvePoint(0f, 0f),
				true
			},
			{
				new CurvePoint(0.5f, 150f),
				true
			},
			{
				new CurvePoint(4f, 150f),
				true
			},
			{
				new CurvePoint(5f, 25f),
				true
			},
			{
				new CurvePoint(10f, 5f),
				true
			},
			{
				new CurvePoint(15f, 0f),
				true
			}
		};

		// Token: 0x04000042 RID: 66
		private static readonly SimpleCurve LevelFinalAdjustmentCurve = new SimpleCurve
		{
			{
				new CurvePoint(0f, 0f),
				true
			},
			{
				new CurvePoint(10f, 10f),
				true
			},
			{
				new CurvePoint(20f, 16f),
				true
			},
			{
				new CurvePoint(27f, 20f),
				true
			}
		};
	}
}
