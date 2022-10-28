using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Ferrex;

public class ThingOrderProcessor
{
    public List<ThingOrderRequest> requestedItems = new List<ThingOrderRequest>();

    public StorageSettings storageSettings;

    public ThingOwner thingHolder;

    public ThingOrderProcessor()
    {
    }

    public ThingOrderProcessor(ThingOwner thingHolder, StorageSettings storageSettings)
    {
        this.thingHolder = thingHolder;
        this.storageSettings = storageSettings;
    }

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