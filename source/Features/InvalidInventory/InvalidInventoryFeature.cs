namespace MechEngineer.Features.InvalidInventory
{
    internal class InvalidInventoryFeature : Feature
    {
        internal static InvalidInventoryFeature Shared = new InvalidInventoryFeature();

        internal override bool Enabled => settings?.Enabled ?? false;

        internal static Settings settings => Control.settings.InvalidInventory;

        public class Settings
        {
            public bool Enabled = true;
        }
    }
}
