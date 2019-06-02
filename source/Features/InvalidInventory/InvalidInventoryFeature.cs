namespace MechEngineer.Features.InvalidInventory
{
    internal class InvalidInventoryFeature : Feature<BaseSettings>
    {
        internal static InvalidInventoryFeature Shared = new InvalidInventoryFeature();

        internal override BaseSettings Settings => Control.settings.InvalidInventory;
    }
}
