using MechEngineer.Features.DebugScreenshotMechs.Patches;

namespace MechEngineer.Features.DebugScreenshotMechs;

public class DebugScreenshotMechsSettings : ISettings
{
    public bool Enabled { get; set; } = false;
    public string EnabledDescription => "Edit a mech then cancel, it then will go through all mechs and make screenshots for them. To improve loading times use the TagManager to blacklist all components so the MechLab doesn't have to load them.";

    public bool CaptureScreenshots { get; set; } = true;
    public string CaptureScreenshotsDescription { get; set; } = "If you just want to dump the game objects stats, skip this.";

    public bool DumpGameObjectCounts { get; set; } = false;
    public string DumpGameObjectCountsDescription { get; set; } = $"Dumps scene memory stats into {SceneMemoryStatsDumper.MemoryFilename}. Uses a heuristic that is optimized for finding uix memory leaks in the MechLab.";

    public string ScreenshotDirectoryPath { get; set; } = "screenshots";
    public string ScreenshotDirectoryPathDescription = "Create the directory manually first, otherwise it won't save any of the images.";

    public bool OnlyInvalidMechs { get; set; } = false;
    public string OnlyInvalidMechsDescription = "Only make screenshots of mechs with invalid configurations.";
}