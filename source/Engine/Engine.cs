using System.Collections.Generic;
using BattleTech;

namespace MechEngineer
{
    internal class Engine
    {
        internal Engine(EngineCoreRef coreRef, EngineType type, List<MechComponentRef> parts)
        {
            CoreRef = coreRef;
            CoreDef = coreRef.CoreDef;
            Type = type;
            Parts = parts;
        }

        internal EngineCoreRef CoreRef { get; }
        internal EngineCoreDef CoreDef { get; set; }
        internal EngineType Type { get; }
        internal List<MechComponentRef> Parts { get; }

        internal int Rating => CoreDef.Rating;
        internal float EngineTonnage => (CoreDef.StandardEngineTonnage * Type.WeightMultiplier).RoundStandard();

        internal float TotalTonnage => CoreRef.HeatSinkTonnage + EngineTonnage + CoreDef.GyroTonnage;

        public float TotalTonnageChanges => TotalTonnage - CoreDef.Def.Tonnage;
    }
}