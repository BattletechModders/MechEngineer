using System;
using JetBrains.Annotations;

namespace MechEngineer.Misc;

// since HarmonyTargetMethod, HarmonyTargetMethods and HarmonyPrepare are buggy, we use this attribute instead
[MeansImplicitUse(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Itself)]
public class UsedByModTekAttribute : Attribute
{

}