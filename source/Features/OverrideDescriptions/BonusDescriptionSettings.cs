using MechEngineer.Misc;

namespace MechEngineer.Features.OverrideDescriptions;

[UsedBy(User.FastJson)]
internal class BonusDescriptionSettings
{
    public string Bonus = null!;
    public string? Short { get; set; }
    public string? Long { get; set; }
    public string? Full { get; set; }
}