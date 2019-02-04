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
    public partial class BonusDescriptions : SimpleCustomComponent, IAdjustTooltip, IAdjustInventoryElement, IAfterLoad
    {
        public string[] Bonuses { get; set; }

        public void AdjustTooltip(TooltipPrefab_Equipment tooltip, MechComponentDef componentDef)
        {
            var adapter = new TooltipPrefab_EquipmentAdapter(tooltip);
            //GUILogUtils.LogHierarchy(tooltip.transform);
            adapter.ShowBonuses = false;
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
                var split = bonus.Split(new[]{':'}, 2);
                var bonusKey = split[0].Trim();

                if (!Settings.TryGetValue(bonusKey, out var settings))
                {
                    Control.mod.Logger.LogError($"Could not find bonus description \"{bonusKey}\" used by {Def.Description.Id}");
                    continue;
                }

                var args = split.Length >= 2 ? split[1].Split(',').Select(c => c.Trim()).ToArray() : new string[0];

                var description = new BonusDescription(settings, args);
                descriptions.Add(description);
            }

            {
                var adapter = new MechComponentDefAdapter(Def);
                var count = 0;
                foreach (var description in descriptions.Select(x => x.Short).Where(x => x != null).Take(2))
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

            {
                var bonusTemplate = Control.settings.BonusDescriptionsBonusTemplate;
                var descriptionTemplate = Control.settings.BonusDescriptionsDescriptionTemplate;
                var adapter = new DescriptionDefAdapter(Def.Description);
                var bonuses = string.Join("", descriptions.Select(x => x.Full).Where(x => x != null).Select(x => bonusTemplate.Replace("{{bonus}}", x)).ToArray());
                adapter.Details = descriptionTemplate.Replace("{{bonuses}}", bonuses).Replace("{{originalDescription}}", adapter.Details);
            }
        }

        [JsonIgnore]
        private static Dictionary<string, BonusDescriptionSettings> Settings { get; set; } = new Dictionary<string, BonusDescriptionSettings>();


        internal static void Setup(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
        {
            Settings = SettingsResourcesTools.Enumerate<BonusDescriptionSettings>("BonusDescriptions", customResources)
                .ToDictionary(entry => entry.Bonus);
        }
        
        internal class BonusDescriptionSettings
        {
#pragma warning disable 649
            public string Bonus;
            public string Short;
            public string Long;
            public string Full;
#pragma warning restore 649
        }

        [JsonIgnore]
        private readonly List<BonusDescription> descriptions = new List<BonusDescription>();

        private class BonusDescription
        {
            internal BonusDescription(BonusDescriptionSettings settings, string[] values)
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
