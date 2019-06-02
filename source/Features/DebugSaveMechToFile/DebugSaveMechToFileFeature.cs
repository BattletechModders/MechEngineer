namespace MechEngineer.Features.DebugSaveMechToFile
{
    internal class DebugSaveMechToFileFeature : Feature
    {
        internal static DebugSaveMechToFileFeature Shared = new DebugSaveMechToFileFeature();

        internal override bool Enabled => settings?.Enabled ?? false;

        internal Settings settings => Control.settings.DebugSaveMechToFile;

        public class Settings
        {
            public bool Enabled = false;
        }
    }
}
