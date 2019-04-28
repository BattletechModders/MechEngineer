using System;
using MechEngineer.Features.ShutdownInjuryProtection.Patches;

namespace MechEngineer.Features.ShutdownInjuryProtection
{
    internal class ShutdownInjuryProtectionFeature : Feature
    {
        internal static ShutdownInjuryProtectionFeature Shared = new ShutdownInjuryProtectionFeature();

        internal override bool Enabled => Control.settings.ShutdownInjuryEnabled || Control.settings.HeatDamageInjuryEnabled;

        internal override Type[] Patches => new[]
        {
            typeof(Mech_CheckForHeatDamage_Patch),
            typeof(Mech_InitEffectStats_Patch),
            typeof(MechShutdownSequence_CheckForHeatDamage_Patch),
        };
    }
}