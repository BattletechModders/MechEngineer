using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;

namespace MechEngineer
{
    internal class MechDefAutoFixCategory
    {
        internal IAutoFixMechDef AutoFixMechDef;
        internal bool AutoFixSetting;
        internal string CompanyStatKey;

        internal void SetShouldFix(StatCollection companyStats)
        {
            ShouldFix = AutoFixSetting && (companyStats == null || !companyStats.ContainsStatistic(CompanyStatKey));
        }

        internal bool ShouldFix { get; private set; }

        internal void SetFixed(StatCollection companyStats) 
        {
            companyStats?.AddStatistic(CompanyStatKey, 1);
        }

        internal static MechDefAutoFixCategory[] Fixers => new[] {
            new MechDefAutoFixCategory
            {
                AutoFixMechDef = CockpitHandler.Shared,
                AutoFixSetting = Control.settings.AutoFixMechDefCockpitAdder != null,
                CompanyStatKey = "MechEngineer_AutoFixMechDefCockpit"
            },
            new MechDefAutoFixCategory
            {
                AutoFixMechDef = GyroHandler.Shared,
                AutoFixSetting = Control.settings.AutoFixMechDefGyroAdder != null,
                CompanyStatKey = "MechEngineer_AutoFixMechDefGyro"
            },
            new MechDefAutoFixCategory
            {
                AutoFixMechDef = ArmorHandler.Shared,
                AutoFixSetting = Control.settings.AutoFixMechDefArmorAdder != null,
                CompanyStatKey = "MechEngineer_AutoFixArmor"
            },
            new MechDefAutoFixCategory
            {
                AutoFixMechDef = StructureHandler.Shared,
                AutoFixSetting = Control.settings.AutoFixMechDefStructureAdder != null,
                CompanyStatKey = "MechEngineer_AutoFixStructure"
            },
            new MechDefAutoFixCategory
            {
                AutoFixMechDef = EngineCoreRefHandler.Shared,
                AutoFixSetting = Control.settings.AutoFixMechDefEngine,
                CompanyStatKey = "MechEngineer_AutoFixMechDefEngine"
            }
        };
    }

    internal static class MechDefAutoFixFacade
    {
        private static void AutoFixMechDef(MechDef mechDef, IEnumerable<MechDefAutoFixCategory> fixers)
        {
            if (Control.settings.AutoFixMechDefSkip.Contains(mechDef.Description.Id))
            {
                return;
            }

            mechDef.Refresh();

            float originalTotalTonnage = 0, maxValue = 0;
            MechStatisticsRules.CalculateTonnage(mechDef, ref originalTotalTonnage, ref maxValue);

            foreach (var fixer in fixers)
            {
                if (fixer.ShouldFix)
                {
                    fixer.AutoFixMechDef.AutoFixMechDef(mechDef, originalTotalTonnage);
                }
            }
        }

        internal static void PostProcessAfterLoading(DataManager dataManager)
        {
            var fixers = MechDefAutoFixCategory.Fixers;
            foreach (var fixer in fixers)
            {
                fixer.SetShouldFix(null);
            }

            foreach (var keyValuePair in dataManager.MechDefs)
            {
                AutoFixMechDef(keyValuePair.Value, fixers);
            }
        }

        internal static void InitCompanyStats(StatCollection companyStats)
        {
            var fixers = MechDefAutoFixCategory.Fixers;
            foreach (var fixer in fixers)
            {
                fixer.SetShouldFix(companyStats);
                if (fixer.ShouldFix)
                {
                    fixer.SetFixed(companyStats);
                }
            }
        }

        internal static void Rehydrate(StatCollection companyStats, List<MechDef> mechs)
        {
            var fixers = MechDefAutoFixCategory.Fixers;
            foreach (var fixer in fixers)
            {
                fixer.SetShouldFix(companyStats);
            }

            foreach (var mech in mechs)
            {
                AutoFixMechDef(mech, fixers);
            }

            foreach (var fixer in fixers)
            {
                if (fixer.ShouldFix)
                {
                    fixer.SetFixed(companyStats);
                }
            }
        }
    }
}