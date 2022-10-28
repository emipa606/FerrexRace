using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Ferrex;

public class PawnCrafterProperties : DefModExtension
{
    public List<ThingOrderRequest> costList = new List<ThingOrderRequest>();

    public string crafterMaterialNeedText = "FerrexPrinterNeed";

    public string crafterMaterialsText = "FerrexPrinterMaterials";

    public string crafterNutritionText = "FerrexNutrition";

    public string crafterPawnCustomizationCraftAdulthoodBackstoryButton = "FerrexCustomizationSecondIdentity";

    public string crafterPawnCustomizationCraftButton = "FerrexCustomizationPrint";

    public string crafterPawnCustomizationCraftChildhoodBackstoryButton = "FerrexCustomizationFirstIdentity";

    public string crafterPawnCustomizationCraftRollFemaleButton = "FerrexCustomizationRollFemale";

    public string crafterPawnCustomizationCraftRollMaleButton = "FerrexCustomizationRollMale";

    public string crafterPawnCustomizationHairColor = "FerrexCustomizationChangeColor";

    public string crafterPawnCustomizationNone = "FerrexNone";

    public string crafterPawnCustomizationPawnBackstoryTag = "FerrexBackstory";

    public string crafterPawnCustomizationTitle = "FerrexCustomization";

    public string crafterPawnGizmoPawnInfoDescription = "FerrexGizmoPrinterAndroidInfoDescription";

    public string crafterPawnGizmoStartCraftDescription = "FerrexGizmoTogglePrintingStartLabel";

    public string crafterPawnGizmoStartCraftLabel = "FerrexGizmoTogglePrintingStartLabel";

    public string crafterPawnGizmoStopCraftDescription = "FerrexGizmoTogglePrintingStopLabel";

    public string crafterPawnGizmoStopCraftLabel = "FerrexGizmoTogglePrintingStopLabel";

    public string crafterProgressText = "FerrexPrinterProgress";

    public string crafterStatusEnumText = "FerrexPrinterStatusEnum";

    public string crafterStatusText = "FerrexPrinterStatus";

    public int defaultSkillLevel = 6;

    public HediffDef hediffOnPawnCrafted;

    public string pawnCraftedLetterLabel = "FerrexPrintedLetterLabel";

    public string pawnCraftedLetterText = "FerrexPrintedLetterDescription";

    public PawnKindDef pawnKind;

    public float powerConsumptionFactorIdle = 0.1f;

    public int resourceTick = 2500;

    public List<SkillRequirement> skills = new List<SkillRequirement>();

    public ThoughtDef thoughtOnPawnCrafted;

    public int ticksToCraft = 60000;

    public float ResourceTicks()
    {
        return (float)Math.Ceiling(ticksToCraft / (double)resourceTick);
    }
}