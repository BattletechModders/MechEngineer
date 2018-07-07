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

        internal EngineCoreRef CoreRef { get; private set; }
        internal EngineCoreDef CoreDef { get; set; }
        internal EngineType Type { get; private set; }
        internal List<MechComponentRef> Parts { get; private set; }

        internal float EngineTonnage
        {
            get { return (CoreDef.StandardEngineTonnage * Type.WeightMultiplier).RoundStandard(); }
        }

        internal float Tonnage
        {
            get { return CoreRef.HeatSinkTonnage + EngineTonnage + CoreDef.GyroTonnage; }
        }

        public float TonnageChanges
        {
            get
            {
                //Control.mod.Logger.LogDebug(string.Format("get_Tonnage={0} Core.Tonnage={1}", Tonnage, Core.Def.Tonnage));
                return Tonnage - CoreDef.Def.Tonnage;
            }
        }
    }
}