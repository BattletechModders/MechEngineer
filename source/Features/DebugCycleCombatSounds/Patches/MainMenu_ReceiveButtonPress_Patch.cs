using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech.UI;
using HBS;
using HBS.Scripting.Reflection;
using MechEngineer.Misc;
using TMPro;

namespace MechEngineer.Features.DebugCycleCombatSounds.Patches;

[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.ReceiveButtonPress))]
public static class MainMenu_ReceiveButtonPress_Patch
{
    [UsedByHarmony]
    public static bool Prepare()
    {
        return DebugCycleCombatSoundsFeature.settings.DebugMainCycleSoundsOnReceiveButtonEnabled;
    }

    private static IEnumerator<string>? Iterator = null;

    // ReSharper disable once InconsistentNaming
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, TextMeshProUGUI ____version, string button)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (Iterator == null || button == DebugCycleCombatSoundsFeature.Shared.Settings.SpecificButton && !Iterator.MoveNext())
        {
            SceneSingletonBehavior<WwiseManager>.Instance.LoadCombatBanks();
            SceneSingletonBehavior<WwiseManager>.Instance.LoadCombatBanks();
            Iterator = EventIds();
        }

        var eventId = Iterator.Current;
        ____version.text = eventId;
        SceneSingletonBehavior<WwiseManager>.Instance.StopAllAudio();
        WwiseManager.PostEvent(Iterator.Current, WwiseManager.GlobalAudioObject);
        Log.Main.Info?.Log($"WwiseManager.PostEvent eventName={eventId}");

        // AudioEventList_aircraft.aircraft_leopard_destruction
        __runOriginal = false;
    }

    public static IEnumerator<string> EventIds()
    {
        var eventListAttr = typeof(WwiseEventList);
        var wwiseEnums = ReflectionUtil.TypesWithAttributeInAppDomain(typeof(WwiseManagedEnum)).ToList<Type>();
        foreach (var eType in wwiseEnums)
        {
            var typeName = eType.Name;
            if (!eType.IsDefined(eventListAttr, true))
            {
                continue;
            }
            var values = Enum.GetValues(eType);
            foreach (var eVal in values)
            {
                var valueName = eVal.ToString();
                var guid = typeName + "_" + valueName;
                yield return guid;
            }
        }
    }
}