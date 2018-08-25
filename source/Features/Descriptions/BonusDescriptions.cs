using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using fastJSON;

namespace MechEngineer
{
    [CustomComponent("BonusDescriptions")]
    public class BonusDescriptions : SimpleCustomComponent, IAdjustTooltip, IAdjustInventoryElement, IAfterLoad
    {
        public string[] Bonuses { get; set; }

        public void AdjustTooltip(TooltipPrefab_Equipment tooltip, MechComponentDef componentDef)
        {
            var adapter = new TooltipPrefab_EquipmentAdapter(tooltip);

            var additional = string.Join("\r\n", Descriptions.Select(x => x.Full).Where(x => x != null).Select(x => $"    {x}").ToArray());
            adapter.detailText.text = "Bonuses:<b><color=#F79B26FF>\r\n" + additional + "</color></b>\r\n\r\n" + adapter.detailText.text;
            //GUILogUtils.LogHierarchy(tooltip.transform);
            adapter.ShowBonuses = false;
        }

        public void AdjustInventoryElement(ListElementController_BASE_NotListView element)
        {
            var count = 0;
            foreach (var description in Descriptions.Select(x => x.Long).Where(x => x != null).Take(2))
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
            
            var adapter = new MechComponentDefAdapter(Def);
            foreach (var bonus in Bonuses)
            {
                var split = bonus.Split(new[]{':'}, 2);
                var bonusKey = split[0].Trim();

                if (!Settings.TryGetValue(bonusKey, out var settings))
                {
                    continue;
                }

                var args = split.Length >= 2 ? split[1].Split(',').Select(c => c.Trim()).ToArray() : new string[0];

                var description = new BonusDescription(settings, args);
                Descriptions.Add(description);
            }

            var count = 0;
            foreach (var description in Descriptions.Select(x => x.Short).Where(x => x != null).Take(2))
            {
                if (count == 0)
                {
                    adapter.BonusValueA = description;
                    count++;
                }
                else if (count == 1)
                {
                    adapter.BonusValueB = description;
                }
            }
        }

        [JsonIgnore]
        private static Dictionary<string, MechEngineerSettings.BonusDescriptionSettings> Settings { get; } = Control.settings.BonusDescriptions.ToDictionary(x => x.Bonus);

        [JsonIgnore]
        private List<BonusDescription> Descriptions = new List<BonusDescription>();

        private class BonusDescription
        {
            internal BonusDescription(MechEngineerSettings.BonusDescriptionSettings settings, string[] values)
            {
                Short = string.IsNullOrEmpty(settings.Short) ? null : string.Format(settings.Short, values);
                Long = string.IsNullOrEmpty(settings.Long) ? null : string.Format(settings.Long, values);
                Full = string.IsNullOrEmpty(settings.Full) ? null : string.Format(settings.Full, values);
            }

            public string Short { get; }
            public string Long { get; }
            public string Full { get; }
        }
    }
}
