namespace MechEngineer.Features.DebugSaveMechToFile
{
    internal class DebugSaveMechToFileFeature : Feature<DebugSaveMechToFileSettings>
    {
        internal static DebugSaveMechToFileFeature Shared = new();

        internal override DebugSaveMechToFileSettings Settings => Control.settings.DebugSaveMechToFile;
    }
}