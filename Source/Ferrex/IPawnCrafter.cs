using System;
using Verse;

namespace Ferrex
{
	// Token: 0x02000004 RID: 4
	public interface IPawnCrafter
	{
		// Token: 0x06000009 RID: 9
		Pawn PawnBeingCrafted();

		// Token: 0x0600000A RID: 10
		CrafterStatus PawnCrafterStatus();

		// Token: 0x0600000B RID: 11
		void InitiatePawnCrafting();

		// Token: 0x0600000C RID: 12
		void StopPawnCrafting();
	}
}
