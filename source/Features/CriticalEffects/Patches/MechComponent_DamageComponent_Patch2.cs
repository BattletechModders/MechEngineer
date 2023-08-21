using BattleTech;

namespace MechEngineer.Features.CriticalEffects.Patches;

[HarmonyPatch(typeof(MechComponent), nameof(MechComponent.DamageComponent))]
public static class MechComponent_DamageComponent_Patch2
{
    [HarmonyPrepare]
    public static bool Prepare()
    {
        return CriticalEffectsFeature.settings.ShowComponentFloatie;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(MechComponent __instance)
    {
        MessagesHandler.PublishComponentState(__instance);
    }
}
