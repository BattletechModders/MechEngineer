using BattleTech;
using MechEngineer.Misc;

namespace MechEngineer.Features.AutoFix;

internal class CockpitHandler : IAdjustUpgradeDef
{
    private static readonly Lazier<CockpitHandler> Lazy = new();
    internal static CockpitHandler Shared => Lazy.Value;

    private readonly IdentityHelper? identity;
    private readonly AdjustCompDefTonnageHelper? reweighter;
    private readonly AdjustCompDefInvSizeHelper? resizer;

    public CockpitHandler()
    {
        identity = AutoFixerFeature.settings.CockpitCategorizer;

        if (identity == null)
        {
            return;
        }

        if (AutoFixerFeature.settings.CockpitTonnageChange != null)
        {
            reweighter = new AdjustCompDefTonnageHelper(identity, AutoFixerFeature.settings.CockpitTonnageChange);
        }

        if (AutoFixerFeature.settings.CockpitSlotChange != null)
        {
            resizer = new AdjustCompDefInvSizeHelper(identity, AutoFixerFeature.settings.CockpitSlotChange);
        }
    }

    public void AdjustUpgradeDef(UpgradeDef upgradeDef)
    {
        reweighter?.AdjustComponentDef(upgradeDef);
        resizer?.AdjustComponentDef(upgradeDef);
    }
}