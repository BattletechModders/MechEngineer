using System.Collections.Generic;
using System.Linq;
using BattleTech;
using UnityEngine;

namespace MechEngineer
{
    internal class Engine
    {
        internal Engine(EngineCoreRef coreRef, Weights weights, IEnumerable<MechComponentRef> externalHeatSinks)
        {
            CoreRef = coreRef;
            CoreDef = coreRef.CoreDef;
            Weights = weights;
            FreeExternalHeatSinkCount = MatchingCount(externalHeatSinks, CoreRef.HeatSinkDef.HeatSinkDef);
        }

        public static float MatchingCount(IEnumerable<MechComponentRef> heatSinks, HeatSinkDef heatSinkDef)
        {
            return heatSinks.Select(r => r.Def).Count(d => d == heatSinkDef);
        }

        internal EngineCoreRef CoreRef { get; }
        internal float FreeExternalHeatSinkCount { get; }

        /* dynamic stuff below */

        internal Weights Weights { get; set; }

        internal EngineCoreDef CoreDef { get; set; }

        internal float FreeExternalHeatSinkTonnage
            => Mathf.Min(FreeExternalHeatSinkCount, CoreDef.MaxFreeExternalHeatSinks)
            * CoreRef.HeatSinkDef.Def.Tonnage;

        internal float GyroTonnage => (CoreDef.StandardGyroTonnage * Weights.GyroFactor).RoundStandard();

        internal float EngineTonnage => (CoreDef.StandardEngineTonnage * Weights.EngineFactor).RoundStandard();
 
        internal float HeatSinkTonnage => CoreRef.InternalHeatSinkTonnage - FreeExternalHeatSinkTonnage;

        internal float TotalTonnage => HeatSinkTonnage + EngineTonnage + GyroTonnage;

        internal float TotalTonnageChanges => TotalTonnage - CoreDef.Def.Tonnage;
    }
}