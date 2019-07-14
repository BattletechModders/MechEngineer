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
            EngineHeatBlockDef engineHeatBlockDef,
            EngineCoreDef coreDef,
            Weights weights,
            List<MechComponentRef> externalHeatSinks)
        {
            ExternalHeatSinks = externalHeatSinks;
            HeatBlockDef = engineHeatBlockDef;
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
                EngineHeatSinkDef = def.GetComponent<EngineHeatSinkDef>();
                CalculateStats();
            }
        }
        
        internal EngineHeatBlockDef HeatBlockDef { get; set; }
        internal EngineCoreDef CoreDef { get; set; }

        internal void CalculateStats()
        {
            ExternalHeatSinkCount = MatchingCount(ExternalHeatSinks, EngineHeatSinkDef.Def);
            //Control.mod.Logger.LogDebug($"ExternalHeatSinkCount={ExternalHeatSinkCount} ExternalHeatSinks.Count={ExternalHeatSinks.Count} EngineHeatSinkDef.Id={EngineHeatSinkDef.Def.Description.Id}");
        }

        internal List<MechComponentRef> ExternalHeatSinks { get; }
        internal int ExternalHeatSinkCount { get; private set; }
        internal int ExternalHeatSinkFreeCount => Mathf.Min(ExternalHeatSinkCount, ExternalHeatSinkFreeMaxCount);
        internal int ExternalHeatSinkAdditionalCount => ExternalHeatSinkCount - ExternalHeatSinkFreeCount;

        internal int TotalHeatSinkCount => InternalHeatSinkCount + ExternalHeatSinkCount;

        /* dynamic stuff below */

        internal Weights Weights { get; set; }

        internal float ExternalHeatSinkFreeTonnage => ExternalHeatSinkFreeCount * EngineHeatSinkDef.Def.Tonnage;

        internal float GyroTonnage => PrecisionUtils.RoundUp(StandardGyroTonnage * Weights.GyroFactor, WeightPrecision);

        internal float EngineTonnage => PrecisionUtils.RoundUp(StandardEngineTonnage * Weights.EngineFactor, WeightPrecision);

        internal float HeatSinkTonnage => - ExternalHeatSinkFreeTonnage; // InternalHeatSinkTonnage

        internal float TotalTonnage => HeatSinkTonnage + EngineTonnage + GyroTonnage;

        internal EngineHeatSinkDef EngineHeatSinkDef { get; set; }

        internal float EngineHeatDissipation
        {
            get
            {
                var dissipation = EngineHeatSinkDef.Def.DissipationCapacity * ( InternalHeatSinkCount + HeatBlockDef.HeatSinkCount );
                dissipation += CoreDef.Def.DissipationCapacity;
                dissipation += CoolingDef.Def.DissipationCapacity;

                //Control.mod.Logger.LogDebug("GetHeatDissipation rating=" + engineDef.Rating + " minHeatSinks=" + minHeatSinks + " additionalHeatSinks=" + engineProps.AdditionalHeatSinkCount + " dissipation=" + dissipation);

                return dissipation;
            }
        }

        internal EngineHeatSinkDef GetInternalEngineHeatSinkTypes()
        {
            return EngineHeatSinkDef;
        }

        private int FreeHeatSinks => EngineFeature.settings.MinimumHeatSinksOnMech;
        private int InternalHeatSinksMaxCount => CoreDef.Rating / 25;

        internal virtual int TotalHeatSinkMinCount => FreeHeatSinks;

        internal virtual int InternalHeatSinkCount => Mathf.Min(FreeHeatSinks, InternalHeatSinksMaxCount);
        internal virtual int InternalHeatSinkAdditionalMaxCount => Mathf.Max(0, InternalHeatSinksMaxCount - FreeHeatSinks);
        internal virtual int ExternalHeatSinkFreeMaxCount => FreeHeatSinks - InternalHeatSinkCount;

        internal virtual float StandardGyroTonnage => PrecisionUtils.RoundUp(CoreDef.Rating / 100f, 1f);
        internal virtual float StandardEngineTonnage => CoreDef.Def.Tonnage - StandardGyroTonnage;

        internal virtual float WeightPrecision => OverrideTonnageFeature.settings.TonnageStandardPrecision;
    }

    internal class ProtoMechEngine : Engine
    {
        internal ProtoMechEngine(EngineSearcher.Result result) : base(result)
        {
        }

        internal override int TotalHeatSinkMinCount => 0;

        internal override int InternalHeatSinkCount => 0;
        internal override int InternalHeatSinkAdditionalMaxCount => 0;
        internal override int ExternalHeatSinkFreeMaxCount => 0;

        internal override float StandardGyroTonnage => 0;
        internal override float StandardEngineTonnage => CoreDef.Def.Tonnage;

        internal override float WeightPrecision => OverrideTonnageFeature.settings.KilogramStandardPrecision;
    }
}