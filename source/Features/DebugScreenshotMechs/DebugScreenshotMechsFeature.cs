using BattleTech;
using System.IO;

namespace MechEngineer.Features.DebugScreenshotMechs
{
    internal class DebugScreenshotMechsFeature : Feature<DebugScreenshotMechsSettings>
    {
        internal static DebugScreenshotMechsFeature Shared = new DebugScreenshotMechsFeature();

        internal override DebugScreenshotMechsSettings Settings => Control.settings.DebugScreenshotMechs;
        
        private string screenshotDirectoryPath;
        internal string ScreenshotPath(MechDef mechDef)
        {
            if (screenshotDirectoryPath == null)
            {
                screenshotDirectoryPath = Path.Combine(Control.mod.Directory, Settings.ScreenshotDirectoryPath);
            }

            if (!Directory.Exists(screenshotDirectoryPath))
            {
                return null;
            }

            return Path.Combine(screenshotDirectoryPath, $"{mechDef.Description.Id}.png");
        }
    }
}
