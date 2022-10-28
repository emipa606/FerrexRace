using RimWorld;
using Verse;

namespace Ferrex;

public class Building_FerrexCrafter : Building_PawnCrafter
{
    public override void InitiatePawnCrafting()
    {
        Find.WindowStack.Add(new CustomizePawnWindow(this));
    }

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
            crafterProperties.pawnCraftedLetterText.Translate(pawnBeingCrafted.NameFullColored),
            LetterDefOf.PositiveEvent,
            pawnBeingCrafted);
        Find.LetterStack.ReceiveLetter(let);
    }
}