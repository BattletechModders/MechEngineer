using CustomComponents;

namespace MechEngineer.Features.CustomCapacities;

[CustomComponent("CapacityMod", AllowArray = true)]
public class CapacityModCustom : SimpleCustomComponent
{
    // Could be CarryOnMech or some custom name like Specialist
    public string Collection { get; set; } = null!;

    // Either operate on Capacity or the Usage of it, IsUsage is false by default
    public bool IsUsage { get; set; }

    // Used to define if a usage and capacity counts for a location, right now only used in CarryInHand for two-handed vs one-handed checks
    // can't be used for normal custom capacities (unclear if show a sum of all locations or each location would get its own stat..)
    public bool IsLocationRestricted { get; set; }

    // When an operation is being applied, lower is earlier. Defaults to 0. Secondary sorting key is based on the operation type enum order
    public int Priority { get; set; }

    // What operation to do
    public OperationType Operation { get; set; } = OperationType.Add;

    // The quantity to use for the operation.
    public float Quantity { get; set; }

    // A pre-defined factor to multiply the quantity with before executing the operation.
    // alternative would be to integrate a formula evaluator
    public QuantityFactorType QuantityFactorType { get; set; } = QuantityFactorType.One;
}

public enum OperationType
{
    Set,
    Add,
    Multiply
}

public enum QuantityFactorType
{
    One,
    ChassisTonnage
}
