using BattleTech;
using MechEngineer.Misc;

namespace MechEngineer.Features.AutoFix;

internal class GyroHandler : IAdjustUpgradeDef
{
    private static readonly Lazier<GyroHandler> Lazy = new();
    internal static GyroHandler Shared => Lazy.Value;

    private readonly IdentityHelper? identity;
    private readonly AdjustCompDefInvSizeHelper? resizer;

    public GyroHandler()
    {
        identity = AutoFixerFeature.settings.GyroCategorizer;

        if (identity == null)
        {
            return;
        }

        if (AutoFixerFeature.settings.GyroSlotChange != null)
        {
            resizer = new AdjustCompDefInvSizeHelper(identity, AutoFixerFeature.settings.GyroSlotChange);
        }
    }

    public void AdjustUpgradeDef(UpgradeDef upgradeDef)
    {
        resizer?.AdjustComponentDef(upgradeDef);
    }
}