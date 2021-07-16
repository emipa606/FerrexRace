using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Ferrex
{
    // Token: 0x0200000A RID: 10
    public class PawnCrafterProperties : DefModExtension
    {
        // Token: 0x0400001B RID: 27
        public List<ThingOrderRequest> costList = new List<ThingOrderRequest>();

        // Token: 0x04000022 RID: 34
        public string crafterMaterialNeedText = "FerrexPrinterNeed";

        // Token: 0x04000021 RID: 33
        public string crafterMaterialsText = "FerrexPrinterMaterials";

        // Token: 0x04000023 RID: 35
        public string crafterNutritionText = "FerrexNutrition";

        // Token: 0x0400002B RID: 43
        public string crafterPawnCustomizationCraftAdulthoodBackstoryButton = "FerrexCustomizationSecondIdentity";

        // Token: 0x04000027 RID: 39
        public string crafterPawnCustomizationCraftButton = "FerrexCustomizationPrint";

        // Token: 0x0400002A RID: 42
        public string crafterPawnCustomizationCraftChildhoodBackstoryButton = "FerrexCustomizationFirstIdentity";

        // Token: 0x04000028 RID: 40
        public string crafterPawnCustomizationCraftRollFemaleButton = "FerrexCustomizationRollFemale";

        // Token: 0x04000029 RID: 41
        public string crafterPawnCustomizationCraftRollMaleButton = "FerrexCustomizationRollMale";

        // Token: 0x04000026 RID: 38
        public string crafterPawnCustomizationHairColor = "FerrexCustomizationChangeColor";

        // Token: 0x04000024 RID: 36
        public string crafterPawnCustomizationNone = "FerrexNone";

        // Token: 0x04000031 RID: 49
        public string crafterPawnCustomizationPawnBackstoryTag = "FerrexBackstory";

        // Token: 0x04000025 RID: 37
        public string crafterPawnCustomizationTitle = "FerrexCustomization";

        // Token: 0x0400002C RID: 44
        public string crafterPawnGizmoPawnInfoDescription = "FerrexGizmoPrinterAndroidInfoDescription";

        // Token: 0x0400002E RID: 46
        public string crafterPawnGizmoStartCraftDescription = "FerrexGizmoTogglePrintingStartLabel";

        // Token: 0x0400002D RID: 45
        public string crafterPawnGizmoStartCraftLabel = "FerrexGizmoTogglePrintingStartLabel";

        // Token: 0x04000030 RID: 48
        public string crafterPawnGizmoStopCraftDescription = "FerrexGizmoTogglePrintingStopLabel";

        // Token: 0x0400002F RID: 47
        public string crafterPawnGizmoStopCraftLabel = "FerrexGizmoTogglePrintingStopLabel";

        // Token: 0x04000020 RID: 32
        public string crafterProgressText = "FerrexPrinterProgress";

        // Token: 0x0400001F RID: 31
        public string crafterStatusEnumText = "FerrexPrinterStatusEnum";

        // Token: 0x0400001E RID: 30
        public string crafterStatusText = "FerrexPrinterStatus";

        // Token: 0x04000038 RID: 56
        public int defaultSkillLevel = 6;

        // Token: 0x04000034 RID: 52
        public HediffDef hediffOnPawnCrafted;

        // Token: 0x0400001C RID: 28
        public string pawnCraftedLetterLabel = "FerrexPrintedLetterLabel";

        // Token: 0x0400001D RID: 29
        public string pawnCraftedLetterText = "FerrexPrintedLetterDescription";

        // Token: 0x0400001A RID: 26
        public PawnKindDef pawnKind;

        // Token: 0x04000036 RID: 54
        public float powerConsumptionFactorIdle = 0.1f;

        // Token: 0x04000033 RID: 51
        public int resourceTick = 2500;

        // Token: 0x04000037 RID: 55
        public List<SkillRequirement> skills = new List<SkillRequirement>();

        // Token: 0x04000035 RID: 53
        public ThoughtDef thoughtOnPawnCrafted;

        // Token: 0x04000032 RID: 50
        public int ticksToCraft = 60000;

        // Token: 0x0600002E RID: 46 RVA: 0x00002EE6 File Offset: 0x000010E6
        public float ResourceTicks()
        {
            return (float) Math.Ceiling(ticksToCraft / (double) resourceTick);
        }
    }
}