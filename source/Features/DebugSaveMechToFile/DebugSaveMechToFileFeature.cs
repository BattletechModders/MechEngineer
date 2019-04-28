using System;
using MechEngineer.Features.DebugSaveMechToFile.Patches;

namespace MechEngineer.Features.DebugSaveMechToFile
{
    internal class DebugSaveMechToFileFeature : Feature
    {
        internal static DebugSaveMechToFileFeature Shared = new DebugSaveMechToFileFeature();

        internal override bool Enabled => Control.settings.SaveMechDefOnMechLabConfirm;

        internal override Type[] Patches => new[]
        {
            typeof(MechLabPanel_DoConfirmRefit_Patch)
        };
    }
}
