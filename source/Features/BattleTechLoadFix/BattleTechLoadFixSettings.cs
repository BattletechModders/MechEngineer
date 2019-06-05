namespace MechEngineer.Features.BattleTechLoadFix
{
    public class BattleTechLoadFixSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Fixes a vanilla bug by loading unused components so they get displayed in the mech lab.";
    }
}