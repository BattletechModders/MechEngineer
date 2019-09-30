namespace MechEngineer.Features.DebugScreenshotMechs
{
    public class DebugScreenshotMechsSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Edit a mech then cancel, it then will go through all mechs and make screenshots for them. To improve loading times use the TagManager to blacklist all components so the MechLab doesn't have to load them.";

        public string ScreenshotDirectoryPath { get; set; } = "screenshots";
        public string ScreenshotDirectoryPathDescription = "Create the directory manually first, otherwise it won't save any of the images.";
    }
}