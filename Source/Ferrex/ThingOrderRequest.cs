using System;
using System.Xml;
using RimWorld;
using Verse;

namespace Ferrex
{
    // Token: 0x02000006 RID: 6
    public class ThingOrderRequest
    {
        // Token: 0x0400000A RID: 10
        public float amount;

        // Token: 0x04000008 RID: 8
        public bool nutrition;

        // Token: 0x04000007 RID: 7
        public ThingDef thingDef;

        // Token: 0x04000009 RID: 9
        public ThingFilter thingFilter;

        // Token: 0x06000010 RID: 16 RVA: 0x000022BC File Offset: 0x000004BC
        public ThingRequest Request()
        {
            if (thingDef != null)
            {
                return ThingRequest.ForDef(thingDef);
            }

            if (nutrition)
            {
                return ThingRequest.ForGroup(ThingRequestGroup.FoodSourceNotPlantOrTree);
            }

            return ThingRequest.ForUndefined();
        }

        // Token: 0x06000011 RID: 17 RVA: 0x000022E8 File Offset: 0x000004E8
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
                    return !(thing is Corpse corpse) || !corpse.IsDessicated();
                }

                return false;
            };
        }

        // Token: 0x06000012 RID: 18 RVA: 0x00002354 File Offset: 0x00000554
        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            if (xmlRoot.ChildNodes.Count != 1)
            {
                Log.Error("Misconfigured ThingOrderRequest: " + xmlRoot.OuterXml);
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

            amount = (float) ParseHelper.FromString(xmlRoot.FirstChild.Value, typeof(float));
        }
    }
}