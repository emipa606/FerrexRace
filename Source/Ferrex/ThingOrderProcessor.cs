using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Ferrex
{
    // Token: 0x02000005 RID: 5
    public class ThingOrderProcessor
    {
        // Token: 0x04000006 RID: 6
        public List<ThingOrderRequest> requestedItems = new List<ThingOrderRequest>();

        // Token: 0x04000005 RID: 5
        public StorageSettings storageSettings;

        // Token: 0x04000004 RID: 4
        public ThingOwner thingHolder;

        // Token: 0x0600000D RID: 13 RVA: 0x00002278 File Offset: 0x00000478
        public ThingOrderProcessor()
        {
        }

        // Token: 0x0600000E RID: 14 RVA: 0x0000228B File Offset: 0x0000048B
        public ThingOrderProcessor(ThingOwner thingHolder, StorageSettings storageSettings)
        {
            this.thingHolder = thingHolder;
            this.storageSettings = storageSettings;
        }

        // Token: 0x0600000F RID: 15 RVA: 0x000022AC File Offset: 0x000004AC
        public IEnumerable<ThingOrderRequest> PendingRequests()
        {
            foreach (var requestedItem in requestedItems)
            {
                float num = thingHolder.TotalStackCountOfDef(requestedItem.thingDef);
                if (!(num < requestedItem.amount))
                {
                    continue;
                }

                var thingOrderRequest = new ThingOrderRequest
                {
                    thingDef = requestedItem.thingDef,
                    amount = requestedItem.amount - num
                };
                yield return thingOrderRequest;
            }
        }
    }
}