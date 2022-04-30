using BattleTech;
using MechEngineer.Features.OverrideTonnage;
using UnityEngine;

namespace MechEngineer.Features.ArmorMaximizer;

public static class ArmorUtils
{
    //Takes TONNAGE_PER_ARMOR_POINT and multiplies it by the ArmorFactor provided by equipped items.
    public static float TonPerPointWithFactor(MechDef mechDef)
    {
        var tonPerPoint = UnityGameInstance.BattleTechGame.MechStatisticsConstants.TONNAGE_PER_ARMOR_POINT;
        var armorFactor = WeightsUtils.CalculateArmorFactor(mechDef);
        var adjustedTonPerPoint = tonPerPoint * armorFactor;
        return adjustedTonPerPoint;
    }

    public static bool IsDivisible(float x, float y)
    {
        if (y < 0) y *= -1f;
        return (x % y) == 0f;
    }
    public static float RoundUp(float x, float y)
    {
        if (y < 0) y *= -1f;
        x = Mathf.Ceil(x / y);
        return x * y;
    }
    public static float RoundDown(float x, float y)
    {
        if (y < 0) y *= -1f;
        x = Mathf.Floor(x / y);
        return x * y;
    }
}