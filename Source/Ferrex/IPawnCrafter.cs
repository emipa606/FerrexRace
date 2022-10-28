using Verse;

namespace Ferrex;

public interface IPawnCrafter
{
    Pawn PawnBeingCrafted();

    CrafterStatus PawnCrafterStatus();

    void InitiatePawnCrafting();

    void StopPawnCrafting();
}