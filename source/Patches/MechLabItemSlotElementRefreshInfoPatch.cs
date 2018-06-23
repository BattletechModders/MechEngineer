using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechLabItemSlotElement), "RefreshInfo")]
    internal static class MechLabItemSlotElementRefreshInfoPatch
    {
        private static EngineRef engineRef;

        internal static void Prefix(this MechLabItemSlotElement __instance)
        {
            try
            {
                engineRef = __instance.ComponentRef.GetEngineRef();
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        internal static void Postfix()
        {
            engineRef = null;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .MethodReplacer(
                    AccessTools.Method(typeof(MechComponentDef), "get_BonusValueA"),
                    AccessTools.Method(typeof(MechLabItemSlotElementRefreshInfoPatch), "OverrideBonusValueA")
                )
                .MethodReplacer(
                    AccessTools.Method(typeof(MechComponentDef), "get_BonusValueB"),
                    AccessTools.Method(typeof(MechLabItemSlotElementRefreshInfoPatch), "OverrideBonusValueB")
                );
        }

        internal static string OverrideBonusValueA(this MechComponentDef @this)
        {
            try
            {
                if (engineRef != null)
                {
                    return engineRef.BonusValueA;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return @this.BonusValueA;
        }

        internal static string OverrideBonusValueB(this MechComponentDef @this)
        {
            try
            {
                if (engineRef != null)
                {
                    return engineRef.BonusValueB;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return @this.BonusValueB;
        }
    }
}