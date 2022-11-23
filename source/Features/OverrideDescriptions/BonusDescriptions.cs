using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using CustomComponents.ExtendedDetails;
using fastJSON;
using Localize;

namespace MechEngineer.Features.OverrideDescriptions;

[CustomComponent("BonusDescriptions")]
public class BonusDescriptions : SimpleCustomComponent, IAdjustTooltipEquipment, IAdjustInventoryElement, IAfterLoad, IListComponent<string>
{
    public string[] Bonuses { get; set; } = null!;

    public void LoadList(IEnumerable<string> items)
    {
        Bonuses = items.ToArray();
    }

    public void AdjustTooltipEquipment(TooltipPrefab_Equipment tooltip, MechComponentDef componentDef)
    {
        // we list bonuses in the description, disable bonus sections
        AdjustTooltipEquipment_ShowBonusSection(tooltip,false);
    }

    internal static void AdjustTooltipEquipment_ShowBonusSection(TooltipPrefab_Equipment tooltip, bool? show = null)
    {
        show ??= !string.IsNullOrEmpty(tooltip.bonusesText.OriginalText) && tooltip.bonusesText.OriginalText != "-";
        var text = tooltip.bonusesText.transform.parent.parent.parent;
        text.gameObject.SetActive(show.Value);
    }

    public void AdjustInventoryElement(ListElementController_BASE_NotListView element)
    {
        var count = 0;
        foreach (var description in descriptions.Select(x => x.Long).Where(x => x != null).Take(2))
        {
            if (count == 0)
            {
                element.ItemWidget.gearBonusText.text = description;
                count++;
            }
            else if (count == 1)
            {
                element.ItemWidget.gearBonusTextB.text = description;
            }
        }
    }

    public void OnLoaded(Dictionary<string, object> values)
    {
        if (Bonuses.Length < 1)
        {
            return;
        }

        foreach (var bonus in Bonuses)
        {
            var split = bonus.Split(new[] {':'}, 2);
            var bonusKey = split[0].Trim();

            if (!OverrideDescriptionsFeature.Resources.TryGetValue(bonusKey, out var settings))
            {
                Log.Main.Warning?.Log($"Could not find bonus description \"{bonusKey}\" used by {Def.Description.Id}");
                continue;
            }

            var args = split.Length >= 2 ? split[1].Split(',').Select(c => c.Trim()).ToArray() : new string[0];

            var description = new BonusDescription(settings, args);
            descriptions.Add(description);
        }

        {
            var count = 0;
            foreach (var description in descriptions.Select(x => x.Short).Where(x => x != null).Take(2))
            {
                if (count == 0)
                {
                    Def.BonusValueA = description;
                    count++;
                }
                else if (count == 1)
                {
                    Def.BonusValueB = description;
                }
            }
        }

        AddTemplatedExtendedDetail(
            ExtendedDetails.GetOrCreate(Def),
            descriptions.Select(x => x.Full),
            OverrideDescriptionsFeature.settings.BonusDescriptionsElementTemplate,
            OverrideDescriptionsFeature.settings.BonusDescriptionsDescriptionTemplate,
            OverrideDescriptionsFeature.settings.DescriptionIdentifier
        );
    }

    internal static void AddTemplatedExtendedDetail(
        ExtendedDetails extended,
        IEnumerable<string?> elements,
        string elementTemplate,
        string descriptionTemplate,
        string identifier,
        UnitType unityType = UnitType.UNDEFINED)
    {
        var elementsText = string.Join("", elements.Where(x => x != null).Select(x => elementTemplate.Replace("{{element}}", x)).ToArray());
        var text = descriptionTemplate.Replace("{{elements}}", elementsText);
        var detail = new ExtendedDetail
        {
            UnitType = unityType,
            Index = -1,
            Text = text,
            Identifier = identifier
        };
        extended.AddDetail(detail);
    }

    [JsonIgnore]
    private readonly List<BonusDescription> descriptions = new();

    private class BonusDescription
    {
        internal BonusDescription(BonusDescriptionSettings settings, string[] values)
        {
            Short = Process(settings.Short, values);
            Long = Process(settings.Long, values);
            Full = Process(settings.Full, values);
        }

        public string? Short { get; }
        public string? Long { get; }
        public string? Full { get; }

        private string? Process(string? format, string[] values)
        {
            try
            {
                var objects = values.Select(x => (object)x).ToArray();
                return string.IsNullOrEmpty(format) ? null : new Text(format, objects).ToString();
            }
            catch (Exception e)
            {
                var message = $"Can't process '{format}'";
                Log.Main.Error?.Log(message, e);
                return message;
            }
        }
    }
}
