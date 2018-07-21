using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    public class RefreshSlotElementHandler : IRefreshSlotElement
    {
        public static RefreshSlotElementHandler Shared = new RefreshSlotElementHandler();

        public void RefreshSlotElement(MechLabItemSlotElement element, MechLabPanel mechLab)
        {
            EngineCoreRefHandler.Shared.RefreshSlotElement(element, mechLab);
            WeightsHandler.Shared.RefreshSlotElement(element, mechLab);
        }

        public void RefreshData(MechLabPanel mechLab)
        {
            foreach (var element in mechLab.Elements())
            {
                RefreshSlotElement(element, mechLab);
            }
        }
    }

    internal interface IRefreshSlotElement
    {
        void RefreshSlotElement(MechLabItemSlotElement element, MechLabPanel panel);
    }

    public static class MechLabExtensions
    {
        public static IEnumerable<MechLabItemSlotElement> Elements(this MechLabPanel mechLab)
        {
            return MechDefSlots.Locations
                .Select(location => mechLab.GetLocationWidget((ArmorLocation) location))
                .Select(widget => new MechLabLocationWidgetAdapter(widget))
                .SelectMany(adapter => adapter.LocalInventory);
        }
    }
}
