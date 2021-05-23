namespace MechEngineer.Features.ModTekLoader
{
    public class ModTekLoaderSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Adds some fixes for merging DLC content, quick hack only for ME for now. Doesn't support dependencies or caching.";

        public string AssetBundleOverridePath { get; set; } = "assetBundle";
        public string AssetBundleOverridePathDescription => "Path containing all merge patches.";
    }
}