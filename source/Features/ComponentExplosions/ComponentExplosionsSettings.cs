namespace MechEngineer.Features.ComponentExplosions
{
    public class ComponentExplosionsSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Allows each component to define destructive forces in case they explode, also implements proper CASE.";

        public bool DisableVanillaAmmunitionBoxDefCanExplode => true;
        public string DisableVanillaAmmunitionBoxDefCanExplodeDescription => "Disables vanilla CanExplode on AmmunitionBoxDef.";

        public bool DisableVanillaMechComponentDefCanExplode => true;
        public string DisableVanillaMechComponentDefCanExplodeDescription => "Disables vanilla CanExplode on MechComponentDef.";
    }
}