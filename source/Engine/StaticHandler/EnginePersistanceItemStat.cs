using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    public static class EnginePersistanceItemStat
    {
        internal static MechComponentRef mechComponentRef;

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .MethodReplacer(
                    AccessTools.Method(typeof(BaseComponentRef), "get_Def"),
                    AccessTools.Method(typeof(EnginePersistanceItemStat), "Def")
                )
                .MethodReplacer(
                    AccessTools.Method(typeof(BaseComponentRef), "get_ComponentDefID"),
                    AccessTools.Method(typeof(EnginePersistanceItemStat), "ComponentDefID")
                )
                .MethodReplacer(
                    AccessTools.Method(typeof(SimGameState), "AddItemStat", new[] {typeof(string), typeof(Type), typeof(bool)}),
                    AccessTools.Method(typeof(EnginePersistanceItemStat), "AddItemStat")
                )
                .MethodReplacer(
                    AccessTools.Method(typeof(SimGameState), "RemoveItemStat", new[] {typeof(string), typeof(Type), typeof(bool)}),
                    AccessTools.Method(typeof(EnginePersistanceItemStat), "RemoveItemStat")
                );
        }

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