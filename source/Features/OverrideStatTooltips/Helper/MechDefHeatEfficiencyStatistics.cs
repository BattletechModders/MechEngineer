﻿using BattleTech;
using MechEngineer.Features.Engines;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.Engines.StaticHandler;
using MechEngineer.Features.OverrideTonnage;
using System.Linq;

namespace MechEngineer.Features.OverrideStatTooltips.Helper
{
    internal class MechDefHeatEfficiencyStatistics
    {
        internal int HeatSinking { get; }
        internal int AlphaStrike { get; }
        internal int JumpHeat { get; }
        internal int MaxHeat { get; }
        internal int Overheat { get; }

        internal MechDefHeatEfficiencyStatistics(MechDef mechDef)
        {
            this.mechDef = mechDef;

            Engine = mechDef.GetEngine();
            EngineHeatSinking = (int) (Engine?.EngineHeatDissipation ?? 0);

            HeatSinkCapacity = GetHeatSinkCapacity();
            HeatSinking = (int)((EngineHeatSinking + HeatSinkCapacity) * MechStatisticsRules.Combat.Heat.GlobalHeatSinkMultiplier);
            AlphaStrike = (int)(GetHeatGenerated() * GetWeaponHeatMultiplier());
            JumpHeat = GetJumpHeat();
            MaxHeat = GetMaxHeat();
            Overheat = GetOverheat();
        }
        
        private Engine Engine { get; }
        private int EngineHeatSinking { get; }
        private int HeatSinkCapacity { get; }

        private readonly MechDef mechDef;
        private readonly StatCollection statCollection = new StatCollection();

        internal float GetStatisticRating()
        {
            return AlphaStrike < 1
                ? 0
                : MechStatUtils.NormalizeToFraction(HeatSinking, (float)AlphaStrike / 3, AlphaStrike);
        }

        private int GetHeatSinkCapacity()
        {
            var stat = statCollection.HeatSinkCapacity();
            stat.Create();
            return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
        }

        private int GetHeatGenerated()
        {
            var defaultValue = mechDef.Inventory
                .Where(x => x.IsFunctionalORInstalling())
                .Select(x => x.Def as WeaponDef)
                .Where(x => x != null)
                .Sum(x => x.HeatGenerated);

            // TODO HeatDivisor support or not? nah we just don't support COIL, who needs that anyway...

            var stat = statCollection.HeatGenerated();
            stat.CreateWithDefault(defaultValue);
            var value = MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
            return PrecisionUtils.RoundUpToInt(value);
        }

        private float GetWeaponHeatMultiplier()
        {
            var stat = statCollection.WeaponHeatMultiplier();
            stat.Create();
            return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
        }

        private int GetJumpHeat()
        {
            var stat = statCollection.JumpHeat();
            stat.Create();
            MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
            return statCollection.GetJumpHeat(1);
        }

        private int GetMaxHeat()
        {
            var stat = statCollection.MaxHeat();
            stat.Create();
            return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
        }

        private int GetOverheat()
        {
            var stat = statCollection.OverheatLevel();
            stat.Create();
            return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
        }
    }
}