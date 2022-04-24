namespace MechEngineer.Features.InvalidInventory;

internal class InvalidInventoryFeature : Feature<InvalidInventorySettings>
{
    internal static readonly InvalidInventoryFeature Shared = new();

    internal override InvalidInventorySettings Settings => Control.settings.InvalidInventory;
}