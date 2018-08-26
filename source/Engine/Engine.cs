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
            Weights = weights;
            FreeExternalHeatSinkCount = MatchingCount(externalHeatSinks, CoreRef.HeatSinkDef.Def);
        }

        public static float MatchingCount(IEnumerable<MechComponentRef> heatSinks, HeatSinkDef heatSinkDef)
        {
            return heatSinks.Select(r => r.Def).Count(d => d == heatSinkDef);
        }

        internal EngineCoreRef CoreRef { get; set; }
        internal float FreeExternalHeatSinkCount { get; }

        /* dynamic stuff below */

        internal Weights Weights { get; set; }

        internal EngineCoreDef CoreDef => CoreRef.CoreDef;

        internal float FreeExternalHeatSinkTonnage
            => Mathf.Min(FreeExternalHeatSinkCount, CoreDef.MaxFreeExternalHeatSinks)
            * CoreRef.HeatSinkDef.Def.Tonnage;

        internal float GyroTonnage => (CoreDef.StandardGyroTonnage * Weights.GyroFactor).RoundUp();

        internal float EngineTonnage => (CoreDef.StandardEngineTonnage * Weights.EngineFactor).RoundUp();
 
        internal float HeatSinkTonnage => CoreRef.InternalHeatSinkTonnage - FreeExternalHeatSinkTonnage;

        internal float TotalTonnage => HeatSinkTonnage + EngineTonnage + GyroTonnage;

        internal float TotalTonnageChanges => TotalTonnage - CoreDef.Def.Tonnage;
    }
}