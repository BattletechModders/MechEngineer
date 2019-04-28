using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.OverrideDescriptions.Patches;

namespace MechEngineer.Features.OverrideDescriptions
{
    internal class OverrideDescriptionsFeature: Feature, IAdjustSlotElement, IAdjustTooltip, IAdjustInventoryElement
    {
        internal static OverrideDescriptionsFeature Shared = new OverrideDescriptionsFeature();

        internal override bool Enabled => Control.settings.FeatureOverrideDescriptionsEnabled;
        internal override string Topic => nameof(Features.OverrideDescriptions);
        internal override Type[] Patches => new[]
        {
            typeof(ListElementController_BASE_NotListView_SetComponentTooltipData_Patch),
            typeof(MechLabPanel_CreateMechComponentItem_Patch),
            typeof(MechLabPanel_ValidateLoadout_Patch),
            typeof(TooltipPrefab_EquipmentSetData_Patch),
        };

        internal override void SetupFeatureLoaded()
        {
            Registry.RegisterSimpleCustomComponents(typeof(BonusDescriptions));
        }

        internal static Dictionary<string, BonusDescriptionSettings> Settings { get; set; } = new Dictionary<string, BonusDescriptionSettings>();

        internal override void SetupResources(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
        {
            Settings = SettingsResourcesTools.Enumerate<BonusDescriptionSettings>("MEBonusDescriptions", customResources)
                .ToDictionary(entry => entry.Bonus);
        }

        public void AdjustSlotElement(MechLabItemSlotElement element, MechLabPanel panel)
        {
            foreach (var cc in element.ComponentRef.Def.GetComponents<IAdjustSlotElement>())
            {
                cc.AdjustSlotElement(element, panel);
            }
        }

        public void RefreshData(MechLabPanel panel)
        {
            foreach (var element in Elements(panel))
            {
                AdjustSlotElement(element, panel);
            }
        }

        public void AdjustTooltip(TooltipPrefab_Equipment tooltip, MechComponentDef componentDef)
        {
            foreach (var cc in componentDef.GetComponents<IAdjustTooltip>())
            {
                cc.AdjustTooltip(tooltip, componentDef);
            }
        }

        public void AdjustInventoryElement(ListElementController_BASE_NotListView element)
        {
            var componentDef = element?.componentDef;
            if (componentDef == null)
            {
                return;
            }

            foreach (var cc in componentDef.GetComponents<IAdjustInventoryElement>())
            {
                cc.AdjustInventoryElement(element);
            }
        }

        private static IEnumerable<MechLabItemSlotElement> Elements(MechLabPanel panel)
        {
            return MechDefBuilder.Locations
                .Select(location => panel.GetLocationWidget((ArmorLocation) location))
                .Select(widget => new MechLabLocationWidgetAdapter(widget))
                .SelectMany(adapter => adapter.LocalInventory);
        }
    }
}
