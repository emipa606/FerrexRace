using RimWorld;
using Verse;

namespace Ferrex
{
    // Token: 0x02000007 RID: 7
    public class Building_FerrexCrafter : Building_PawnCrafter
    {
        // Token: 0x06000015 RID: 21 RVA: 0x00002423 File Offset: 0x00000623
        public override void InitiatePawnCrafting()
        {
            Find.WindowStack.Add(new CustomizePawnWindow(this));
        }

        // Token: 0x06000016 RID: 22 RVA: 0x00002438 File Offset: 0x00000638
        public override void FinishAction()
        {
            if (crafterProperties.hediffOnPawnCrafted != null)
            {
                pawnBeingCrafted.health.AddHediff(crafterProperties.hediffOnPawnCrafted);
            }

            if (crafterProperties.thoughtOnPawnCrafted != null)
            {
                pawnBeingCrafted.needs.mood.thoughts.memories.TryGainMemory(crafterProperties.thoughtOnPawnCrafted);
            }

            DropPodUtility.DropThingsNear(InteractionCell, Map, new Thing[]
            {
                pawnBeingCrafted
            });
            var let = LetterMaker.MakeLetter(
                crafterProperties.pawnCraftedLetterLabel.Translate(pawnBeingCrafted.Name.ToStringShort),
                crafterProperties.pawnCraftedLetterText.Translate(pawnBeingCrafted.Name), LetterDefOf.PositiveEvent,
                pawnBeingCrafted);
            Find.LetterStack.ReceiveLetter(let);
        }
    }
}