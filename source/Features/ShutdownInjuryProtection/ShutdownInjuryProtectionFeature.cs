using BattleTech;
using MechEngineer.Features.ShutdownInjuryProtection.Patches;
using MechEngineer.Misc;

namespace MechEngineer.Features.ShutdownInjuryProtection;

internal class ShutdownInjuryProtectionFeature : Feature<ShutdownInjuryProtectionSettings>
{
    internal static readonly ShutdownInjuryProtectionFeature Shared = new();

    internal override bool Enabled => base.Enabled && (settings.ShutdownInjuryEnabled || settings.HeatDamageInjuryEnabled);

    internal override ShutdownInjuryProtectionSettings Settings => Control.settings.ShutdownInjuryProtection;

    internal static ShutdownInjuryProtectionSettings settings => Shared.Settings;

    internal static void SetInjury(Mech mech, string sourceID, int stackItemUID)
    {
        if (!mech.IsOverheated)
        {
            return;
        }

        mech.SetInjury(sourceID, stackItemUID, Pilot_InjuryReasonDescription_Patch.InjuryReasonOverheated, mech.GUID == sourceID ? DamageType.OverheatSelf : DamageType.Overheat);
    }
}