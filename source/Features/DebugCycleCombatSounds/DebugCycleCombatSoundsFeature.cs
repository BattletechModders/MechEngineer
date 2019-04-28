using System;
using MechEngineer.Features.DebugCycleCombatSounds.Patches;

namespace MechEngineer.Features.DebugCycleCombatSounds
{
    internal class DebugCycleCombatSoundsFeature : Feature
    {
        internal static DebugCycleCombatSoundsFeature Shared = new DebugCycleCombatSoundsFeature();

        internal override bool Enabled => Control.settings.DebugCycleCombatSoundsFeatureEnabled;

        internal override Type[] Patches => new[]
        {
            typeof(MainMenu_ReceiveButtonPress_Patch)
        };
    }
}
