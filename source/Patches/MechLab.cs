using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    public static class MechLab
    {
        private static readonly WeakReference GlobalReference = new WeakReference(null);

        public static MechLabPanel Current
        {
            set { GlobalReference.Target = value; }
            get { return GlobalReference.Target as MechLabPanel; }
        }

        [HarmonyPatch(typeof(MechLabPanel), "LoadMech")]
        public static class MechLabPanelLoadMechPatch
        {
            public static void Postfix(MechLabPanel __instance)
            {
                try
                {
                    Current = __instance;
                }
                catch (Exception e)
                {
                    Control.mod.Logger.LogError(e);
                }
            }
        }

        [HarmonyPatch(typeof(MechLabPanel), "ExitMechLab")]
        public static class MechLabPanelExitMechLabPatch
        {
            public static void Postfix(MechLabPanel __instance)
            {
                try
                {
                    Current = null;
                }
                catch (Exception e)
                {
                    Control.mod.Logger.LogError(e);
                }
            }
        }
    }
}