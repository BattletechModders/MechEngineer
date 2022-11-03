using BattleTech;
using Harmony;
using HBS.Util;
using System;
using System.Reflection;
using MechEngineer.Misc;

namespace MechEngineer.Features.TagManager.Patches;

[HarmonyPatch]
public static class JSONSerializationUtility_RehydrateObjectFromDictionary_Patch
{
    [UsedByHarmony]
    public static MethodBase TargetMethod()
    {
        return typeof(JSONSerializationUtility).GetMethod(nameof(JSONSerializationUtility.RehydrateObjectFromDictionary), BindingFlags.NonPublic | BindingFlags.Static)!;
    }

    [HarmonyPriority(Priority.High)]
    [HarmonyPostfix]
    public static void Postfix(object target)
    {
        try
        {
            if (target is MechComponentDef componentDef)
            {
                TagManagerFeature.Shared.ManageComponentTags(componentDef);
            }
            else if (target is MechDef mechDef)
            {
                TagManagerFeature.Shared.ManageMechTags(mechDef);
            }
            else if (target is PilotDef pilotDef)
            {
                TagManagerFeature.Shared.ManagePilotTags(pilotDef);
            }
            else if (target is LanceDef lanceDef)
            {
                TagManagerFeature.Shared.ManageLanceTags(lanceDef);
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}