using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace Ferrex
{
	// Token: 0x0200000C RID: 12
	[StaticConstructorOnStartup]
	public class Gizmo_CrafterPawnInfo : Command
	{
		// Token: 0x06000038 RID: 56 RVA: 0x00003EBC File Offset: 0x000020BC
		public Gizmo_CrafterPawnInfo(IPawnCrafter crafter)
		{
			this.crafter = crafter;
			defaultLabel = crafter.PawnBeingCrafted().Name.ToStringFull;
			defaultDesc = Translator.Translate(description);
			icon = Gizmo_CrafterPawnInfo.emptyIcon;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003F14 File Offset: 0x00002114
		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
		{
			GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth);
			float width = GetWidth(maxWidth);
			Rect rect = new Rect(topLeft.x + 10f, topLeft.y, width - 40f, width - 20f);
			Vector2 vector = new Vector2(width - 20f, width);
			GUI.DrawTexture(new Rect(rect.x, rect.y, vector.x, vector.y), PortraitsCache.Get(crafter.PawnBeingCrafted(), vector, default, 1f));
			return result;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003FAA File Offset: 0x000021AA
		public override void ProcessInput(Event ev)
		{
			base.ProcessInput(ev);
			Find.WindowStack.Add(new Dialog_InfoCard(crafter.PawnBeingCrafted()));
		}

		// Token: 0x04000043 RID: 67
		public IPawnCrafter crafter;

		// Token: 0x04000044 RID: 68
		public static Texture2D emptyIcon = ContentFinder<Texture2D>.Get("UI/Overlays/ThingLine", true);

		// Token: 0x04000045 RID: 69
		public string description = "FerrexGizmoPrinterFerrexInfoDescription";
	}
}
