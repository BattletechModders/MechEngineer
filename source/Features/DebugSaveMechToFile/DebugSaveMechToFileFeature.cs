namespace MechEngineer.Features.DebugSaveMechToFile
{
    internal class DebugSaveMechToFileFeature : Feature
    {
        internal static DebugSaveMechToFileFeature Shared = new DebugSaveMechToFileFeature();

        internal override bool Enabled => Control.settings.SaveMechDefOnMechLabConfirm;
    }
}
