using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using fastJSON;

namespace MechEngineer.Features.OverrideDescriptions
{
    [CustomComponent("BonusDescriptions")]
    public class BonusDescriptions : SimpleCustomComponent, IAdjustTooltip, IAdjustInventoryElement, IAfterLoad
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
            
            AddBonusDescriptions(
                Def.Description,
                descriptions.Select(x => x.Full),
                Control.settings.BonusDescriptionsElementTemplate,
                Control.settings.BonusDescriptionsDescriptionTemplate
            );
        }

        internal static void AddBonusDescriptions(
            DescriptionDef descriptionDef,
            IEnumerable<string> elements,
            string elementTemplate,
            string descriptionTemplate)
        {
            var adapter = new DescriptionDefAdapter(descriptionDef);
            var bonuses = string.Join("", elements.Where(x => x != null).Select(x => elementTemplate.Replace("{{element}}", x)).ToArray());
            adapter.Details = descriptionTemplate.Replace("{{elements}}", bonuses).Replace("{{originalDescription}}", adapter.Details);
        }

        [JsonIgnore]
        private static Dictionary<string, BonusDescriptionSettings> Settings { get; set; } = new Dictionary<string, BonusDescriptionSettings>();


        internal static void SetupResources(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
        {
            Settings = SettingsResourcesTools.Enumerate<BonusDescriptionSettings>("MEBonusDescriptions", customResources)
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
                Short = Process(settings.Short, values);
                Long = Process(settings.Long, values);
                Full = Process(settings.Full, values);
            }

            public string Short { get; }
            public string Long { get; }
            public string Full { get; }

            private string Process(string format, string[] values)
            {
                try
                {
                    return string.IsNullOrEmpty(format) ? null : string.Format(format, values);
                }
                catch (Exception e)
                {
                    var message = $"Can't process '{format}'";
                    Control.mod.Logger.LogError(message, e);
                    return message;
                }
            }
        }
    }
}
