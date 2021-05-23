using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MechEngineer.Features.ModTekLoader
{
    internal class ModTekLoaderFeature : Feature<ModTekLoaderSettings>
    {
        internal static ModTekLoaderFeature Shared = new();

        internal override ModTekLoaderSettings Settings => Control.settings.ModTekLoader;

        internal override bool Enabled => Directory.Exists(AssetBundleOverridePath) && base.Enabled;

        internal string AssetBundleOverridePath => Path.Combine(Control.mod.Directory, Settings.AssetBundleOverridePath);

        internal override void SetupFeatureLoaded()
        {
            base.SetupFeatureLoaded();

            Manifest = Directory.GetFiles(AssetBundleOverridePath)
                .Where(x => x.EndsWith(".json"))
                .ToDictionary(Path.GetFileNameWithoutExtension);
        }

        internal Dictionary<string, string> Manifest;
    }
}
