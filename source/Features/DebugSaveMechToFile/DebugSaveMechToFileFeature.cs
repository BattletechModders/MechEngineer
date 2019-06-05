namespace MechEngineer.Features.DebugSaveMechToFile
{
    internal class DebugSaveMechToFileFeature : Feature<DebugSaveMechToFileSettings>
    {
        internal static DebugSaveMechToFileFeature Shared = new DebugSaveMechToFileFeature();

        internal override DebugSaveMechToFileSettings Settings => Control.settings.DebugSaveMechToFile;
    }
}
