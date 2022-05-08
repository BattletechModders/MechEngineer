using System;
using Harmony;
using HBS.Logging;

namespace MechEngineer.Features.BetterLog.Patches;

[HarmonyPatch(typeof(Logger), nameof(Logger.SetLoggerLevel))]
internal static class Logger_SetLoggerLevel_Patch
{
    [HarmonyPostfix]
    public static void Postfix(string name)
    {
        try
        {
            BetterLogFeature.OnSetLoggerLevel(name);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}