// Decompiled with JetBrains decompiler
// Type: OnePointArmorStep.MechLabLocationWidget_OnRearArmorAdd_Patch
// Assembly: OnePointArmorStep, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5B99AEB-44F5-4628-8A84-C6DA28EAF682
// Assembly location: D:\Steam\steamapps\common\BATTLETECH\Mods\OnePointArmorAdjustment\OnePointArmorStep.dll

using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.ArmorMaximizer.Patches;

    [HarmonyPatch(typeof (MechLabLocationWidget), nameof(MechLabLocationWidget.OnRearArmorAdd))]
    public static class MechLabLocationWidget_OnRearArmorAdd_Patch
    {
        public static bool Prefix(MechLabLocationWidget __instance) => ArmorMaximizerHandler.handleArmorUpdate(__instance, true, +1f);
    }