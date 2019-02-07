﻿using System.Collections.Generic;
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

        internal static void SetMechDefAutoFixCategory()
        {
            Fixers.Clear();
            void Add(MechDefAutoFixCategory cat)
            {
                if (cat.AutoFixSetting)
                {
                    Fixers.Add(cat);
                }
            }

            //Add(new MechDefAutoFixCategory
            //{
            //    AutoFixMechDef = ArmActuatorHandler.Shared,
            //    AutoFixSetting = Control.settings.AutoFixMechDefArmActuator,
            //    CompanyStatKey = "MechEngineer_AutoFixMechDefArmActuator"
            //});

            Add(new MechDefAutoFixCategory
            {
                AutoFixMechDef = EngineHandler.Shared,
                AutoFixSetting = Control.settings.AutoFixMechDefEngine,
                CompanyStatKey = "MechEngineer_AutoFixMechDefEngine"
            });

            Add(new MechDefAutoFixCategory
            {
                AutoFixMechDef = ChassisHandler.Shared,
                AutoFixSetting = Control.settings.AutoFixChassisDefSlotsChanges != null,
                CompanyStatKey = "MechEngineer_AutoFixMechDefByChassisDefSlotsChanges"
            });
        }

        internal static List<MechDefAutoFixCategory> Fixers = new List<MechDefAutoFixCategory>();
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

            foreach (var fixer in fixers)
            {
                if (fixer.ShouldFix)
                {
                    fixer.AutoFixMechDef.AutoFixMechDef(mechDef);
                }
            }
        }

        internal static void PostProcessAfterLoading(DataManager dataManager)
        {
            var fixers = MechDefAutoFixCategory.Fixers;
            if (fixers.Count == 0)
            {
                return;
            }

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
            if (fixers.Count == 0)
            {
                return;
            }

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
            if (fixers.Count == 0)
            {
                return;
            }

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