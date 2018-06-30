using System.Collections.Generic;
using BattleTech;

namespace MechEngineer
{
    internal class Engine
    {
        internal Engine(EngineCoreRef coreRef, EngineTypeDef typeDef, List<MechComponentRef> parts)
        {
            CoreRef = coreRef;
            CoreDef = coreRef.CoreDef;
            TypeDef = typeDef;
            Parts = parts;
        }

        internal EngineCoreRef CoreRef { get; private set; }
        internal EngineCoreDef CoreDef { get; set; }
        internal EngineTypeDef TypeDef { get; private set; }
        internal List<MechComponentRef> Parts { get; private set; }

        internal float EngineTonnage
        {
            get { return (CoreDef.StandardEngineTonnage * TypeDef.Type.WeightMultiplier).RoundStandard(); }
        }

        internal float Tonnage
        {
            get { return CoreRef.HeatSinkTonnage + EngineTonnage + CoreDef.GyroTonnage; }
        }

        public float TonnageChanges
        {
            get
            {
                //Control.mod.Logger.LogDebug(string.Format("get_Tonnage={0} CoreDef.Tonnage={1}", Tonnage, CoreDef.Def.Tonnage));
                return Tonnage - CoreDef.Def.Tonnage;
            }
        }
    }
}