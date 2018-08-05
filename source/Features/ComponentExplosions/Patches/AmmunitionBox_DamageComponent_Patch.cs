using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(AmmunitionBox), nameof(AmmunitionBox.DamageComponent))]
    public static class AmmunitionBox_DamageComponent_Patch
    {
        //private static FastInvokeHandler handler;

        //public static bool Prefix(AmmunitionBox __instance, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        //{
        //    Control.mod.Logger.LogDebug($"AmmunitionBox_DamageComponent_Patch Prefix");
        //    try
        //    {
        //        var method = AccessTools.Method(typeof(MechComponent), nameof(MechComponent.DamageComponent));
        //        if (handler == null)
        //        {
        //            handler = MethodInvoker.GetHandler(method);
        //        }
        //        handler.Invoke((MechComponent)__instance, new object[] {hitInfo, damageLevel, applyEffects});

        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        Control.mod.Logger.LogError(e);
        //    }

        //    return true;
        //}

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var count = 0;
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldarg_3)
                {
                    count++;
                }

                if (count == 2)
                {
                    instruction.operand = null;
                    instruction.opcode = OpCodes.Ret;
                    yield return instruction;
                    yield break;
                }
                yield return instruction;
            }
        }
    }
}