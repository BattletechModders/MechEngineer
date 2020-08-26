using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.OverrideTonnage;
using UnityEngine;

namespace MechEngineer.Features.Engines
{
    internal class Engine
    {
        internal static Engine GetEngine(ChassisDef chassisDef, IEnumerable<MechComponentRef> componentRefs)
        {
            var result = EngineSearcher.SearchInventory(componentRefs);
            if (result.CoolingDef == null || result.CoreDef == null || result.HeatBlockDef == null)
            {
                return null;
            }

            if (chassisDef.ChassisTags.Contains("ProtoMech")) // TODO hardcoded, we want to introduce more options than just this (probably some chassis custom)
            {
                return new ProtoMechEngine(result);
            }

            return new Engine(result);
        }

        protected Engine(EngineSearcher.Result result) : this(result.CoolingDef, result.HeatBlockDef, result.CoreDef, result.Weights, result.HeatSinks)
        {
        }

        // should be private but used during autofixer, rename EngineSearcher.Result to .Builder and apply new semantics
        internal Engine(
            CoolingDef coolingDef,
            EngineHeatBlockDef engineEngineHeatBlockDef,
            EngineCoreDef coreDef,
            Weights weights,
            List<MechComponentRef> heatSinksExternal)
        {
            HeatSinksExternal = heatSinksExternal;
            EngineHeatBlockDef = engineEngineHeatBlockDef;
            CoreDef = coreDef;
            Weights = weights;
            CoolingDef = coolingDef; // last as it also CalculateStats()
        }

        public static int MatchingCount(IEnumerable<MechComponentRef> heatSinks, HeatSinkDef heatSinkDef)
        {
            return heatSinks.Select(r => r.Def).Count(d => d == heatSinkDef);
        }

        private CoolingDef _coolingDef;
        internal CoolingDef CoolingDef
        {
            get => _coolingDef;
            set
            {
                _coolingDef = value;
                var id = _coolingDef.HeatSinkDefId;
                var def = UnityGameInstance.BattleTechGame.DataManager.HeatSinkDefs.Get(id);
                MechHeatSinkDef = def.GetComponent<EngineHeatSinkDef>();
                CalculateStats();
            }
        }

        internal void CalculateStats()
        {
            HeatSinkExternalCount = MatchingCount(HeatSinksExternal, MechHeatSinkDef.Def);
            Control.Logger.Debug?.Log($"HeatSinkExternalFreeCount={HeatSinkExternalFreeCount} MechHeatSinkDef.Def.Tonnage={MechHeatSinkDef.Def.Tonnage}");
        }

        private List<MechComponentRef> HeatSinksExternal { get; }
        private int HeatSinkExternalCount { get; set; }
        
        internal EngineCoreDef CoreDef { get; set; }
        internal Weights Weights { get; set; }
        internal EngineHeatBlockDef EngineHeatBlockDef { get; set; } // amount of internal heat sinks
        internal EngineHeatSinkDef MechHeatSinkDef { get; set; } // type of internal heat sinks and compatible external heat sinks

        internal float EngineHeatDissipation
        {
            get
            {
                var dissipation = MechHeatSinkDef.Def.DissipationCapacity * ( HeatSinkInternalFreeMaxCount + EngineHeatBlockDef.HeatSinkCount );
                dissipation += CoreDef.Def.DissipationCapacity;
                dissipation += CoolingDef.Def.DissipationCapacity;
                return dissipation;
            }
        }

        #region heat sink counting

        internal int HeatSinkExternalFreeCount => Mathf.Min(HeatSinkExternalCount, HeatSinkExternalFreeMaxCount);
        internal int HeatSinkExternalAdditionalCount => HeatSinkExternalCount - HeatSinkExternalFreeCount;

        private int HeatSinkTotalCount => HeatSinkInternalCount + HeatSinkExternalCount;
        internal int HeatSinkInternalCount => HeatSinkInternalFreeMaxCount + EngineHeatBlockDef.HeatSinkCount;

        private int HeatSinksFreeMaxCount => EngineFeature.settings.MinimumHeatSinksOnMech;
        private int HeatSinksInternalMaxCount => CoreDef.Rating / 25;

        protected virtual int HeatSinkTotalMinCount => HeatSinksFreeMaxCount;

        internal virtual int HeatSinkInternalFreeMaxCount => Mathf.Min(HeatSinksFreeMaxCount, HeatSinksInternalMaxCount);
        internal virtual int HeatSinkInternalAdditionalMaxCount => Mathf.Max(0, HeatSinksInternalMaxCount - HeatSinksFreeMaxCount);
        internal virtual int HeatSinkExternalFreeMaxCount => HeatSinksFreeMaxCount - HeatSinkInternalFreeMaxCount;

        internal bool IsMissingHeatSinks(out int min, out int count)
        {
            min = HeatSinkTotalMinCount;
            count = HeatSinkTotalCount;
            return count < min;
        }

        #endregion

        #region weights

        internal float HeatSinkExternalFreeTonnage => HeatSinkExternalFreeCount * MechHeatSinkDef.Def.Tonnage;
        internal float GyroTonnage => PrecisionUtils.RoundUp(StandardGyroTonnage * Weights.GyroFactor, WeightPrecision);
        internal float EngineTonnage => PrecisionUtils.RoundUp(StandardEngineTonnage * Weights.EngineFactor, WeightPrecision);
        internal float HeatSinkTonnage => - HeatSinkExternalFreeTonnage;
        internal float TotalTonnage => HeatSinkTonnage + EngineTonnage + GyroTonnage;

        internal virtual float StandardGyroTonnage => PrecisionUtils.RoundUp(CoreDef.Rating / 100f, 1f);
        internal virtual float StandardEngineTonnage => CoreDef.Def.Tonnage - StandardGyroTonnage;

        internal virtual float WeightPrecision => OverrideTonnageFeature.settings.TonnageStandardPrecision;

        #endregion
    }

    internal class ProtoMechEngine : Engine
    {
        internal ProtoMechEngine(EngineSearcher.Result result) : base(result)
        {
        }

        protected override int HeatSinkTotalMinCount => 0;

        internal override int HeatSinkInternalFreeMaxCount => 0;
        internal override int HeatSinkInternalAdditionalMaxCount => 0;
        internal override int HeatSinkExternalFreeMaxCount => 0;

        internal override float StandardGyroTonnage => 0;
        internal override float StandardEngineTonnage => CoreDef.Def.Tonnage;

        internal override float WeightPrecision => OverrideTonnageFeature.settings.KilogramStandardPrecision;
    }
}