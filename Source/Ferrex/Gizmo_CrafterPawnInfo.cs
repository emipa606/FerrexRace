using RimWorld;
using UnityEngine;
using Verse;

namespace Ferrex;

[StaticConstructorOnStartup]
public class Gizmo_CrafterPawnInfo : Command
{
    public static Texture2D emptyIcon = ContentFinder<Texture2D>.Get("UI/Overlays/ThingLine");

    public IPawnCrafter crafter;

    public string description = "FerrexGizmoPrinterFerrexInfoDescription";

    public Gizmo_CrafterPawnInfo(IPawnCrafter crafter)
    {
        this.crafter = crafter;
        defaultLabel = crafter.PawnBeingCrafted().Name.ToStringFull;
        defaultDesc = description.Translate();
        icon = emptyIcon;
    }

    public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
    {
        var result = base.GizmoOnGUI(topLeft, maxWidth, parms);
        var width = GetWidth(maxWidth);
        var rect = new Rect(topLeft.x + 10f, topLeft.y, width - 40f, width - 20f);
        var vector = new Vector2(width - 20f, width);
        GUI.DrawTexture(new Rect(rect.x, rect.y, vector.x, vector.y),
            PortraitsCache.Get(crafter.PawnBeingCrafted(), vector, Rot4.South));
        return result;
    }

    public override void ProcessInput(Event ev)
    {
        base.ProcessInput(ev);
        Find.WindowStack.Add(new Dialog_InfoCard(crafter.PawnBeingCrafted()));
    }
}