using System.Collections.Generic;
using System.Linq;
using BattleTech;
using UnityEngine;

namespace MechEngineer
{
    internal class Engine
    {
        internal Engine(EngineCoreRef coreRef, EngineType type, IEnumerable<MechComponentRef> externalHeatSinks)
        {
            CoreRef = coreRef;
            CoreDef = coreRef.CoreDef;
            Type = type;
            FreeExternalHeatSinkCount = MatchingCount(externalHeatSinks, CoreDef);
        }

        public static float MatchingCount(IEnumerable<MechComponentRef> heatSinks, EngineCoreDef coreRef)
        {
            return heatSinks.Select(c => c.Def).Count(d => d == coreRef.HeatSinkDef);
        }

        internal EngineCoreRef CoreRef { get; }
        internal EngineCoreDef CoreDef { get; set; }
        internal EngineType Type { get; }
        internal float FreeExternalHeatSinkCount { get; }
        
        internal float FreeExternalHeatSinkTonnage
            => Mathf.Min(FreeExternalHeatSinkCount, CoreDef.MaxFreeExternalHeatSinks)
            * CoreRef.HeatSinkDef.Def.Tonnage;

        internal float EngineTonnage => (CoreDef.StandardEngineTonnage * Type.WeightMultiplier).RoundStandard();

        internal float HeatSinkTonnage => CoreRef.InternalHeatSinkTonnage - FreeExternalHeatSinkTonnage;

        internal float TotalTonnage => HeatSinkTonnage + EngineTonnage + CoreDef.GyroTonnage;

        public float TotalTonnageChanges => TotalTonnage - CoreDef.Def.Tonnage;
    }
}