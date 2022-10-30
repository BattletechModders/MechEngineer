namespace MechEngineer.Misc;

// since HarmonyTargetMethod, HarmonyTargetMethods and HarmonyPrepare are buggy, we use this attribute instead
internal class UsedByHarmonyAttribute : UsedByAttribute
{
    internal UsedByHarmonyAttribute() : base(User.Harmony)
    {
    }
}