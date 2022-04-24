using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using MechEngineer.Features.DynamicSlots;

namespace MechEngineer.Features.OverrideDescriptions;

internal class OverrideDescriptionsFeature : Feature<OverrideDescriptionsSettings>, IAdjustSlotElement, IAdjustTooltipEquipment, IAdjustTooltipWeapon, IAdjustInventoryElement
{
    internal static readonly OverrideDescriptionsFeature Shared = new();

    internal override OverrideDescriptionsSettings Settings => Control.settings.OverrideDescriptions;

    internal static OverrideDescriptionsSettings settings => Shared.Settings;

    internal override void SetupFeatureLoaded()
    {
        Registry.RegisterSimpleCustomComponents(typeof(BonusDescriptions));
    }

    internal static Dictionary<string, BonusDescriptionSettings> Resources { get; set; } = new();

    internal override void SetupResources(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
    {
        Resources = SettingsResourcesTools.Enumerate<BonusDescriptionSettings>("MEBonusDescriptions", customResources)
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

    public void AdjustTooltipEquipment(TooltipPrefab_Equipment tooltip, MechComponentDef componentDef)
    {
        foreach (var cc in componentDef.GetComponents<IAdjustTooltipEquipment>())
        {
            cc.AdjustTooltipEquipment(tooltip, componentDef);
        }
    }

    public void AdjustTooltipWeapon(TooltipPrefab_Weapon tooltip, MechComponentDef componentDef)
    {
        foreach (var cc in componentDef.GetComponents<IAdjustTooltipWeapon>())
        {
            cc.AdjustTooltipWeapon(tooltip, componentDef);
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
            .Select(location => panel.GetLocationWidget((ArmorLocation)location))
            .SelectMany(widget => widget.localInventory);
    }
}