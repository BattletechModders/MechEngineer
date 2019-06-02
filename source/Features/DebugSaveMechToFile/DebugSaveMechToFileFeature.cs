namespace MechEngineer.Features.DebugSaveMechToFile
{
    internal class DebugSaveMechToFileFeature : Feature<BaseSettings>
    {
        internal static DebugSaveMechToFileFeature Shared = new DebugSaveMechToFileFeature();

        internal override BaseSettings Settings => Control.settings.DebugSaveMechToFile;
    }
}
