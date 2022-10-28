using UnityEngine;
using Verse;

namespace Ferrex;

[StaticConstructorOnStartup]
public class Gizmo_ToggleCrafting : Command
{
    public static Texture2D startIcon = ContentFinder<Texture2D>.Get("UI/Commands/Trade");

    public static Texture2D stopIcon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel");

    public IPawnCrafter crafter;

    public string descriptionStart = "FerrexGizmoTogglePrintingStartDescription";

    public string descriptionStop = "FerrexGizmoTogglePrintingStopDescription";

    public string labelStart = "FerrexGizmoTogglePrintingStartLabel";

    public string labelStop = "FerrexGizmoTogglePrintingStopLabel";

    public Gizmo_ToggleCrafting(IPawnCrafter crafter)
    {
        this.crafter = crafter;
        if (crafter.PawnCrafterStatus() == CrafterStatus.Idle)
        {
            defaultLabel = labelStart.Translate();
            defaultDesc = descriptionStart.Translate();
            icon = startIcon;
            return;
        }

        if (crafter.PawnCrafterStatus() != CrafterStatus.Crafting &&
            crafter.PawnCrafterStatus() != CrafterStatus.Filling)
        {
            return;
        }

        defaultLabel = labelStop.Translate();
        defaultDesc = descriptionStop.Translate();
        icon = stopIcon;
    }

    public override void ProcessInput(Event ev)
    {
        base.ProcessInput(ev);
        if (crafter.PawnCrafterStatus() == CrafterStatus.Idle)
        {
            crafter.InitiatePawnCrafting();
            return;
        }

        crafter.StopPawnCrafting();
    }
}