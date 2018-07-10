using System.Collections.Generic;
using BattleTech;
using UnityEngine;

namespace MechEngineer
{
    internal class Engine
    {
        internal Engine(EngineCoreRef coreRef, EngineType type, float externalHeatSinkTonnage)
        {
            CoreRef = coreRef;
            CoreDef = coreRef.CoreDef;
            Type = type;
            ExternalHeatSinkTonnage = externalHeatSinkTonnage;
        }

        internal EngineCoreRef CoreRef { get; }
        internal EngineCoreDef CoreDef { get; set; }
        internal EngineType Type { get; }
        internal float ExternalHeatSinkTonnage;

        internal float FreeExternalHeatSinkTonnage => Mathf.Min(ExternalHeatSinkTonnage, CoreDef.MaxFreeExternalHeatSinkTonnage);

        internal float EngineTonnage => (CoreDef.StandardEngineTonnage * Type.WeightMultiplier).RoundStandard();

        internal float HeatSinkTonnage => CoreRef.InternalHeatSinkTonnage - FreeExternalHeatSinkTonnage;

        internal float TotalTonnage => HeatSinkTonnage + EngineTonnage + CoreDef.GyroTonnage;

        public float TotalTonnageChanges => TotalTonnage - CoreDef.Def.Tonnage;
    }
}