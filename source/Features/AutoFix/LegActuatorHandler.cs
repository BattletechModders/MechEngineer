using System.Collections.Generic;
using BattleTech;
using CustomComponents;
using MechEngineer.Misc;

namespace MechEngineer.Features.AutoFix;

// this isn't yet leg actuators, but we still did reduce the legs size
internal class LegActuatorHandler : IAdjustUpgradeDef, IPreProcessor
{
    private static readonly Lazier<LegActuatorHandler> Lazy = new();
    internal static LegActuatorHandler Shared => Lazy.Value;

    private readonly IdentityHelper? identity;
    private readonly AdjustCompDefInvSizeHelper? resizer;

    public LegActuatorHandler()
    {
        identity = AutoFixerFeature.settings.LegUpgradesCategorizer;

        if (identity == null)
        {
            return;
        }

        if (AutoFixerFeature.settings.LegUpgradesSlotChange != null)
        {
            resizer = new AdjustCompDefInvSizeHelper(identity, AutoFixerFeature.settings.LegUpgradesSlotChange);
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