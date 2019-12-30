using BattleTech;
using MechEngineer.Features.Engines.StaticHandler;
using MechEngineer.Features.OverrideTonnage;
using System.Linq;
using UnityEngine;

namespace MechEngineer.Features.Engines.Helper
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

            DissipationCapacity = GetDissipationCapacity();
            HeatSinkCapacity = GetHeatSinkCapacity();
            HeatSinking = (int)(DissipationCapacity + HeatSinkCapacity * MechStatisticsRules.Combat.Heat.GlobalHeatSinkMultiplier);
            AlphaStrike = GetHeatGenerated();
            JumpHeat = GetJumpHeat();
            MaxHeat = GetMaxHeat();
            Overheat = GetOverheat();
        }
        
        private Engine Engine { get; }
        private float DissipationCapacity { get; }
        private float HeatSinkCapacity { get; }

        private readonly MechDef mechDef;
        private readonly StatCollection statCollection = new StatCollection();

        internal float GetStatisticRating()
        {
            // only weapons and heatsinking
            var min = (float)AlphaStrike / 3;
            var max = AlphaStrike - min;
            return AlphaStrike < 1 ? 0 : (HeatSinking - min) / max;
        }

        private float GetDissipationCapacity()
        {
            var dissipation = mechDef.Inventory
                .Where(x => x.IsFunctionalORInstalling())
                .Select(x => x.Def as HeatSinkDef)
                .Where(x => x != null)
                .Sum(x => x.DissipationCapacity);
            
            if (Engine != null)
            {
                dissipation += Engine.EngineHeatDissipation;
            }
            
            return dissipation;
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

            var stat = statCollection.HeatGenerated();
            stat.CreateWithDefault(defaultValue);
            var value = MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
            return PrecisionUtils.RoundUpToInt(value);
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