namespace MechEngineer.Features.NewSaveFolder
{
    internal class NewSaveFolderSettings : ISettings
    {
        public bool Enabled { get; set; } = false;
        public string EnabledDescription => "Redirects all save operations to use one folder for all save related data.";

        public string Path = "Mods/Saves";
    }
}