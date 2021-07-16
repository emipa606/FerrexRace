using UnityEngine;
using Verse;

namespace Ferrex
{
    // Token: 0x0200000D RID: 13
    [StaticConstructorOnStartup]
    public class Gizmo_ToggleCrafting : Command
    {
        // Token: 0x04000046 RID: 70
        public static Texture2D startIcon = ContentFinder<Texture2D>.Get("UI/Commands/Trade");

        // Token: 0x04000047 RID: 71
        public static Texture2D stopIcon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel");

        // Token: 0x04000048 RID: 72
        public IPawnCrafter crafter;

        // Token: 0x0400004A RID: 74
        public string descriptionStart = "FerrexGizmoTogglePrintingStartDescription";

        // Token: 0x0400004C RID: 76
        public string descriptionStop = "FerrexGizmoTogglePrintingStopDescription";

        // Token: 0x04000049 RID: 73
        public string labelStart = "FerrexGizmoTogglePrintingStartLabel";

        // Token: 0x0400004B RID: 75
        public string labelStop = "FerrexGizmoTogglePrintingStopLabel";

        // Token: 0x0600003C RID: 60 RVA: 0x00003FF0 File Offset: 0x000021F0
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

        // Token: 0x0600003D RID: 61 RVA: 0x000040AB File Offset: 0x000022AB
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
}