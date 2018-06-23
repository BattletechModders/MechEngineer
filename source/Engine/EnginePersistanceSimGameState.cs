using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    public static class EnginePersistanceSimGameState
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .MethodReplacer(
                    AccessTools.Method(typeof(BaseComponentRef), "get_Def"),
                    AccessTools.Method(typeof(EnginePersistanceSimGameState), "Def")
                )
                .MethodReplacer(
                    AccessTools.Method(typeof(BaseComponentRef), "get_ComponentDefID"),
                    AccessTools.Method(typeof(EnginePersistanceSimGameState), "ComponentDefID")
                )
                .MethodReplacer(
                    AccessTools.Method(typeof(SimGameState), "AddItemStat", new[] { typeof(string), typeof(Type), typeof(bool) }),
                    AccessTools.Method(typeof(EnginePersistanceSimGameState), "AddItemStat")
                )
                .MethodReplacer(
                    AccessTools.Method(typeof(SimGameState), "RemoveItemStat", new[] { typeof(string), typeof(Type), typeof(bool) }),
                    AccessTools.Method(typeof(EnginePersistanceSimGameState), "RemoveItemStat")
                );
        }

        internal static MechComponentRef mechComponentRef;

        internal static string ComponentDefID(this BaseComponentRef @this)
        {
            mechComponentRef = @this as MechComponentRef;
            return @this.ComponentDefID;
        }

        internal static MechComponentDef Def(this BaseComponentRef @this)
        {
            mechComponentRef = @this as MechComponentRef;
            return @this.Def;
        }

        internal static void AddItemStat(this SimGameState sim, string id, Type type, bool damaged)
        {
            try
            {
                if (mechComponentRef != null)
                {
                    EnginePersistence.AddInternalItemsStat(sim, mechComponentRef, id, type, damaged);
                }
                mechComponentRef = null;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            sim.AddItemStat(id, type, damaged);
        }

        internal static void RemoveItemStat(this SimGameState sim, string id, Type type, bool damaged)
        {
            try
            {
                if (mechComponentRef != null)
                {
                    EnginePersistence.RemoveInternalItemsStat(sim, mechComponentRef, id, type, damaged);
                }
                mechComponentRef = null;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            sim.RemoveItemStat(id, type, damaged);
        }
    }
}