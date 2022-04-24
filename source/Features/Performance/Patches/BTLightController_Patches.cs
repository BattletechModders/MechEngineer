using System.Collections.Generic;
using BattleTech.Rendering;
using Harmony;

namespace MechEngineer.Features.Performance.Patches;

// lazy sort lights only when accessing it
public static class BTLightController_State
{
    public static bool RequiresSorting = false;
}

[HarmonyPatch(typeof(BTLightController), nameof(BTLightController.GetLightArray))]
public static class BTLightController_GetLightArray_Patch
{
    public static void Prefix(List<BTLight> ___lightList)
    {
        if (BTLightController_State.RequiresSorting)
        {
            ___lightList.Sort();
            BTLightController_State.RequiresSorting = false;
        }
    }
}
[HarmonyPatch(typeof(BTLightController), nameof(BTLightController.ProcessCommandBufferLegacy))]
public static class BTLightController_ProcessCommandBufferLegacy_Patch
{
    public static void Prefix(List<BTLight> ___lightList)
    {
        if (BTLightController_State.RequiresSorting)
        {
            ___lightList.Sort();
            BTLightController_State.RequiresSorting = false;
        }
    }
}

[HarmonyPatch(typeof(BTLightController), nameof(BTLightController.SortList))]
public static class BTLightController_SortList_Patch
{
    public static bool Prefix()
    {
        BTLightController_State.RequiresSorting = true;
        return false;
    }
}
[HarmonyPatch(typeof(BTLightController), nameof(BTLightController.AddLight))]
public static class BTLightController_AddLight_Patch
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return instructions.MethodReplacer(
            AccessTools.Method(typeof(List<BTLight>), nameof(List<BTLight>.Sort)),
            AccessTools.Method(typeof(BTLightController_AddLight_Patch), nameof(Sort))
        );
    }

    public static void Sort(this List<BTLight> @this)
    {
        BTLightController_State.RequiresSorting = true;
    }
}