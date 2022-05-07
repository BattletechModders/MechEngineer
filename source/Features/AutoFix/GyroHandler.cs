using System.Collections.Generic;
using BattleTech;
using CustomComponents;
using MechEngineer.Misc;

namespace MechEngineer.Features.AutoFix;

internal class GyroHandler : IAdjustUpgradeDef, IPreProcessor
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

    public void PreProcess(object target, Dictionary<string, object> values)
    {
        identity?.PreProcess(target, values);
    }

    public void AdjustUpgradeDef(UpgradeDef upgradeDef)
    {
        resizer?.AdjustComponentDef(upgradeDef);
    }
}