using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace MechEngineer.Features.HardpointFix.Public;

public static class CalculatorSetup
{
    internal static WeaponComponentPrefabCalculator? SharedCalculator;

    public static void Setup(ChassisDef? chassisDef, List<MechComponentRef>? componentRefs)
    {
        if (chassisDef?.HardpointDataDef?.HardpointData == null)
        {
            return;
        }

        if (componentRefs == null || componentRefs.Count == 0)
        {
            return;
        }

        componentRefs = componentRefs
            .Where(c => c != null)
            .Where(c => c.ComponentDefType == ComponentType.Weapon)
            .Select(c =>
            {
                if (c.DataManager == null)
                {
                    c.DataManager = UnityGameInstance.BattleTechGame.DataManager;
                    c.RefreshComponentDef();
                }
                return c;
            })
            .Where(c => c.Def is WeaponDef)
            .ToList();

        if (componentRefs.Count == 0)
        {
            return;
        }

        try
        {
            SharedCalculator = new WeaponComponentPrefabCalculator(chassisDef, componentRefs);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }

    public static void Reset()
    {
        SharedCalculator = null;
    }
}
