namespace MechEngineer.Features.InvalidInventory
{
    internal class InvalidInventoryFeature : Feature<InvalidInventorySettings>
    {
        internal static InvalidInventoryFeature Shared = new InvalidInventoryFeature();

        internal override InvalidInventorySettings Settings => Control.settings.InvalidInventory;
    }
}
