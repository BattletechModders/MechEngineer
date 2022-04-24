namespace MechEngineer.Features.InvalidInventory;

public class InvalidInventorySettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Make sure invalid mech configurations can't be saved in skirmish and can't be fielded in the campaign.";
}