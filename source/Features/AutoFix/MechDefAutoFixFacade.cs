using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;
using CustomComponents;

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

            AutoFixer.Shared.RegisterSaveMechFixer(MechDefAutoFixFacade.AutoFix);
        }

        internal static List<MechDefAutoFixCategory> Fixers = new List<MechDefAutoFixCategory>();
    }

    internal static class MechDefAutoFixFacade
    {
        public static void AutoFix(List<MechDef> mechDefs, SimGameState simgame)
        {
            var fixers = MechDefAutoFixCategory.Fixers;

            foreach (var fixer in fixers)
            {
                fixer.SetShouldFix(simgame?.CompanyStats);
            }

            foreach (var mechDef in mechDefs)
            {
                if (Control.settings.AutoFixMechDefSkip.Contains(mechDef.Description.Id))
                {
                    continue;
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

            if (simgame != null)
            {
                foreach (var fixer in fixers)
                {
                    fixer.SetShouldFix(simgame.CompanyStats);
                    if (fixer.ShouldFix)
                    {
                        fixer.SetFixed(simgame.CompanyStats);
                    }
                }
            }
        }
    }
}