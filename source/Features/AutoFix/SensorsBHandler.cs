using BattleTech;
using MechEngineer.Misc;

namespace MechEngineer.Features.AutoFix;

internal class SensorsBHandler : IAdjustUpgradeDef
{
    private static readonly Lazier<SensorsBHandler> Lazy = new();
    internal static SensorsBHandler Shared => Lazy.Value;

    private readonly IdentityHelper? identity;
    private readonly AdjustCompDefInvSizeHelper? resizer;

    public SensorsBHandler()
    {
        identity = AutoFixerFeature.settings.SensorsBCategorizer;

        if (identity == null)
        {
            return;
        }

        if (AutoFixerFeature.settings.SensorsBSlotChange != null)
        {
            resizer = new AdjustCompDefInvSizeHelper(identity, AutoFixerFeature.settings.SensorsBSlotChange);
        }
    }

    public void AdjustUpgradeDef(UpgradeDef upgradeDef)
    {
        if (identity?.IsCustomType(upgradeDef) ?? false)
        {
            upgradeDef.AllowedLocations = ChassisLocations.Head;
        }
        resizer?.AdjustComponentDef(upgradeDef);
    }
}