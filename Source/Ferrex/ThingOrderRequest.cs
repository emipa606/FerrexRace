using System;
using System.Xml;
using RimWorld;
using Verse;

namespace Ferrex;

public class ThingOrderRequest
{
    public float amount;

    public bool nutrition;

    public ThingDef thingDef;

    public ThingFilter thingFilter;

    public ThingRequest Request()
    {
        if (thingDef != null)
        {
            return ThingRequest.ForDef(thingDef);
        }

        return nutrition
            ? ThingRequest.ForGroup(ThingRequestGroup.FoodSourceNotPlantOrTree)
            : ThingRequest.ForUndefined();
    }

    public Predicate<Thing> ExtraPredicate()
    {
        if (!nutrition)
        {
            return _ => true;
        }

        if (thingFilter == null)
        {
            return delegate(Thing thing)
            {
                var def = thing.def;
                return def != null && !def.ingestible.IsMeal && thing.def.IsNutritionGivingIngestible;
            };
        }

        return delegate(Thing thing)
        {
            if (thingFilter.Allows(thing) && thing.def.IsNutritionGivingIngestible)
            {
                return thing is not Corpse corpse || !corpse.IsDessicated();
            }

            return false;
        };
    }

    public void LoadDataFromXmlCustom(XmlNode xmlRoot)
    {
        if (xmlRoot.ChildNodes.Count != 1)
        {
            Log.Error($"Misconfigured ThingOrderRequest: {xmlRoot.OuterXml}");
            return;
        }

        if (xmlRoot.Name.ToLower() == "nutrition")
        {
            nutrition = true;
        }
        else
        {
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "thingDef", xmlRoot.Name);
        }

        amount = (float)ParseHelper.FromString(xmlRoot.FirstChild.Value, typeof(float));
    }
}