using System.Collections.Generic;
using System.Reflection;
using BattleTech.UI.Tooltips;
using Harmony;

namespace CustomComponents
{
    [HarmonyPatch(typeof(TooltipManager), "SetActiveTooltip")]
    public static class TooltipManagerSetActiveTooltipPatch
    {
        private static ICustomComponent customComponent;

        public static void Prefix(object data)
        {
            customComponent = data as ICustomComponent;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .MethodReplacer(
                    AccessTools.Property(typeof(MemberInfo), "Name").GetGetMethod(),
                    AccessTools.Method(typeof(TooltipManagerSetActiveTooltipPatch), "OverrideName")
                );
        }

        public static void Postfix()
        {
            customComponent = null;
        }


        public static string OverrideName(this MemberInfo @this)
        {
            var name = customComponent == null ? @this.Name : Registry.GetTooltipTypeForCustomType(customComponent.CustomType).Name;
            return name;
        }
    }
}