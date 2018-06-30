using System.Collections.Generic;
using BattleTech;

namespace MechEngineer
{
    internal class Engine
    {
        internal Engine(EngineCoreRef coreRef, EngineTypeDef typeDef, List<MechComponentRef> parts)
        {
            CoreRef = coreRef;
            TypeDef = typeDef;
            Parts = parts;
        }

        internal EngineCoreRef CoreRef { get; private set; }
        internal EngineTypeDef TypeDef { get; private set; }
        internal List<MechComponentRef> Parts { get; private set; }

        internal float EngineTonnage
        {
            get { return (CoreRef.CoreDef.StandardEngineTonnage * (TypeDef == null ? 1.0f : TypeDef.Type.WeightMultiplier)).RoundStandard(); }
        }

        internal float Tonnage
        {
            get { return CoreRef.HeatSinkTonnage + EngineTonnage + CoreRef.CoreDef.GyroTonnage; }
        }

        public float TonnageChanges
        {
            get { return -CoreRef.CoreDef.Def.Tonnage + Tonnage; }
        }
    }
}