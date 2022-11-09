#nullable disable
using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.MechLabSlots.FastMechStorage;

[HarmonyPatch(typeof(UnityEngine.UI.ScrollRect), nameof(UnityEngine.UI.ScrollRect.LateUpdate))]
public static class ScrollRect_LateUpdate_Patch
{
    [HarmonyPrefix]
    public static void Prefix(UnityEngine.UI.ScrollRect __instance)
    {
        try
        {
            CustomMechBayMechStorageWidgetTracker.Get(__instance)?.ScrollRectLateUpdate();
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.ClearInventory))]
public static class MechBayMechStorageWidget_ClearInventory_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance)
    {
        Control.Logger.Trace?.Log("MechBayMechStorageWidget.ClearInventory");
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                customWidget.ClearInventory();
                return false;
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.InitInventory), typeof(List<MechDef>), typeof(bool))]
public static class MechBayMechStorageWidget_InitInventory_MechDefs_Patch
{

    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance, List<MechDef> mechDefs, bool resetFilters)
    {
        Control.Logger.Trace?.Log("MechBayMechStorageWidget.InitInventory<MechDef>");
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                customWidget.InitInventory(mechDefs, resetFilters);
                return false;
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.InitInventory), typeof(List<ChassisDef>), typeof(bool))]
public static class MechBayMechStorageWidget_InitInventory_Chassis_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance, List<ChassisDef> chassisDefs, bool resetFilters)
    {
        Control.Logger.Trace?.Log("MechBayMechStorageWidget.InitInventory<ChassisDef>");
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                customWidget.InitInventory(chassisDefs, resetFilters);
                return false;
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.GetInventoryItem))]
public static class MechBayMechStorageWidget_GetInventoryItem_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance, string id, ref IMechLabDraggableItem __result)
    {
        Control.Logger.Trace?.Log("MechBayMechStorageWidget.GetInventoryItem");
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                __result = customWidget.GetInventoryItem(id);
                return false;
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.GetMechDefByGUID))]
public static class MechBayMechStorageWidget_GetMechDefByGUID_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance, string GUID, ref IMechLabDraggableItem __result)
    {
        Control.Logger.Trace?.Log("MechBayMechStorageWidget.GetMechDefByGUID");
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                __result = customWidget.GetMechDefByGUID(GUID);
                return false;
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.ApplyFiltering))]
public static class MechBayMechStorageWidget_ApplyFiltering_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance)
    {
        Control.Logger.Trace?.Log("MechBayMechStorageWidget.ApplyFiltering");
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                customWidget.FilterAndSort(true);
                return false;
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.SetSorting))]
public static class MechBayMechStorageWidget_SetSorting_Patch
{
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    public static bool Prefix(MechBayMechStorageWidget __instance)
    {
        Control.Logger.Trace?.Log("MechBayMechStorageWidget.SetSorting");
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                customWidget.FilterAndSort(false);
                return false;
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.SetData))]
public static class MechBayMechStorageWidget_SetData_Patch
{
    [HarmonyPrefix]
    public static bool Prefix()
    {
        Control.Logger.Trace?.Log("MechBayMechStorageWidget.SetData");
        return true;
    }
}

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.CreateLanceItem))]
public static class MechBayMechStorageWidget_CreateLanceItem_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance, MechDef def, ref LanceLoadoutMechItem __result)
    {
        Control.Logger.Trace?.Log("MechBayMechStorageWidget.CreateLanceItem");
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                __result = customWidget.CreateLanceItem(def);
                return false;
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.OnAddItem))]
public static class MechBayMechStorageWidget_OnAddItem_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance, IMechLabDraggableItem item, bool validate, ref bool __result)
    {
        Control.Logger.Trace?.Log("MechBayMechStorageWidget.OnAddItem");
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                __result = customWidget.OnAddItem(item, validate);
                return false;
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.OnRemoveItem))]
public static class MechBayMechStorageWidget_OnRemoveItem_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechBayMechStorageWidget __instance, IMechLabDraggableItem item, ref bool __result)
    {
        Control.Logger.Trace?.Log("MechBayMechStorageWidget.OnRemoveItem");
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                __result = customWidget.OnRemoveItem(item);
                return false;
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.OnItemGrab))]
public static class MechBayMechStorageWidget_OnItemGrab_Patch
{
    [HarmonyPrefix]
    public static void Prefix(MechBayMechStorageWidget __instance, IMechLabDraggableItem item)
    {
        Control.Logger.Trace?.Log("MechBayMechStorageWidget.OnItemGrab");
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                customWidget.OnItemGrab(ref item);
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.OnButtonClicked))]
public static class MechBayMechStorageWidget_OnButtonClicked_Patch
{
    [HarmonyPrefix]
    public static void Prefix(MechBayMechStorageWidget __instance, ref IMechLabDraggableItem item)
    {
        Control.Logger.Trace?.Log("MechBayMechStorageWidget.OnButtonClicked");
        try
        {
            if (CustomMechBayMechStorageWidgetTracker.TryGet(__instance, out var customWidget))
            {
                customWidget.OnButtonClicked(ref item);
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}
