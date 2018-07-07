using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    public static class Global
    {
        public static MechDef ActiveMechDef => ActiveMechDefFromLab ?? SelectedMechDefFromBay;

        #region MechLabPanel

        private static MechDef ActiveMechDefFromLab => ActiveMechLabPanel?.activeMechDef;
        private static readonly WeakReference MechLabPanelReference = new WeakReference(null);
        public static MechLabPanel ActiveMechLabPanel
        {
            private set => MechLabPanelReference.Target = value;
            get => MechLabPanelReference.Target as MechLabPanel;
        }

        [HarmonyPatch(typeof(MechLabPanel), "LoadMech")]
        public static class MechLabPanel_LoadMech_Patch
        {
            public static void Postfix(MechLabPanel __instance)
            {
                try
                {
                    ActiveMechLabPanel = __instance;
                }
                catch (Exception e)
                {
                    Control.mod.Logger.LogError(e);
                }
            }
        }

        [HarmonyPatch(typeof(MechLabPanel), "ExitMechLab")]
        public static class MechLabPanel_ExitMechLab_Patch
        {
            public static void Postfix(MechLabPanel __instance)
            {
                try
                {
                    ActiveMechLabPanel = null;
                }
                catch (Exception e)
                {
                    Control.mod.Logger.LogError(e);
                }
            }
        }

        #endregion

        #region MechBayPanel

        private static MechDef SelectedMechDefFromBay => 
            ActiveMechBayPanel == null ? null :
                Traverse.Create(ActiveMechBayPanel).Field("selectedMech").GetValue<MechBayMechUnitElement>()?.MechDef;
        private static readonly WeakReference MechBayPanelReference = new WeakReference(null);
        public static MechBayPanel ActiveMechBayPanel
        {
            private set => MechBayPanelReference.Target = value;
            get => MechBayPanelReference.Target as MechBayPanel;
        }

        [HarmonyPatch(typeof(MechBayPanel), "SelectMech")]
        public static class MechBayPanel_SelectMech_Patch
        {
            public static void Postfix(MechBayPanel __instance)
            {
                try
                {
                    ActiveMechBayPanel = __instance;
                }
                catch (Exception e)
                {
                    Control.mod.Logger.LogError(e);
                }
            }
        }

        [HarmonyPatch(typeof(MechBayPanel), "CloseMechBay")]
        public static class MechBayPanel_CloseMechBay_Patch
        {
            public static void Postfix(MechBayPanel __instance)
            {
                try
                {
                    ActiveMechBayPanel = null;
                }
                catch (Exception e)
                {
                    Control.mod.Logger.LogError(e);
                }
            }
        }

        #endregion
    }
}