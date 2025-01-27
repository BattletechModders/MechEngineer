using System;
using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.CriticalEffects;

internal readonly struct ComponentCriticals
{
    internal readonly MechComponent component;
    private AbstractActor actor => component.parent;

    internal ComponentCriticals(MechComponent component)
    {
        this.component = component;
        if (actor == null)
        {
            throw new NullReferenceException($"{nameof(actor)} is null");
        }
    }

    internal int ComponentHittableCount()
    {
        return ComponentHitMax() - ComponentHitCount();
    }

    internal int ComponentHitMax()
    {
        var stat = component.StatCollection.MECriticalSlotsHitMax();
        if (stat.CreateIfMissing()) { // move to Mech.init and remove "CreateIfMissing" from StatAdapter
            var componentDef = component.componentDef;
            var size = componentDef.InventorySize;
            // TODO fix size correctly for location:
            // introduce fake items for overflow location that is crit linked and overwrite component hit max for original + crit linked
            var additionalSize =
                componentDef.Is<DynamicSlots.DynamicSlots>(out var slot)
                && slot.InnerAdjacentOnly ? slot.ReservedSlots : 0;
            stat.SetDefault(size + additionalSize);
        }
        return stat.Get();
    }

    internal int ComponentHitCount(int? setHits = null)
    {
        var stat = component.StatCollection.MECriticalSlotsHit();
        stat.CreateIfMissing(); // move to Mech.init and remove "CreateIfMissing" from StatAdapter
        if (setHits.HasValue)
        {
            stat.SetValue(setHits.Value);
        }
        return stat.Get();
    }

    internal int ComponentHitArmoredCount(int? setHits = null)
    {
        var stat = component.StatCollection.MECriticalSlotsHitArmored();
        stat.CreateIfMissing(); // move to Mech.init and remove "CreateIfMissing" from StatAdapter
        if (setHits.HasValue)
        {
            stat.SetValue(setHits.Value);
        }
        return stat.Get();
    }
}