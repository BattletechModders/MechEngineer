using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;
using UnityEngine;

namespace MechEngineer.Features.Engine
{
    internal class Engine
    {
        internal Engine(
            CoolingDef coolingDef,
            EngineHeatBlockDef engineHeatBlockDef,
            EngineCoreDef coreDef,
            Weights.Weights weights,
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
        internal int ExternalHeatSinkFreeCount => Mathf.Min(ExternalHeatSinkCount, CoreDef.ExternalHeatSinksFreeMaxCount);
        internal int ExternalHeatSinkAdditionalCount => ExternalHeatSinkCount - ExternalHeatSinkFreeCount;

        /* dynamic stuff below */

        internal Weights.Weights Weights { get; set; }

        internal float ExternalHeatSinkFreeTonnage => ExternalHeatSinkFreeCount * EngineHeatSinkDef.Def.Tonnage;

        internal float GyroTonnage => (CoreDef.StandardGyroTonnage * Weights.GyroFactor).RoundUp();

        internal float EngineTonnage => (CoreDef.StandardEngineTonnage * Weights.EngineFactor).RoundUp();
 
        internal float HeatSinkTonnage => - ExternalHeatSinkFreeTonnage; // InternalHeatSinkTonnage

        internal float TotalTonnage => HeatSinkTonnage + EngineTonnage + GyroTonnage;

        internal EngineHeatSinkDef EngineHeatSinkDef { get; set; }

        internal float EngineHeatDissipation
        {
            get
            {
                var dissipation = EngineHeatSinkDef.Def.DissipationCapacity * ( CoreDef.InternalHeatSinks + HeatBlockDef.HeatSinkCount );
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
    }
}