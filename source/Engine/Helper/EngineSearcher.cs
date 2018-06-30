using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace MechEngineer
{
    internal class EngineSearcher
    {
        internal static Result SearchInventory(IEnumerable<MechComponentRef> componentRefs)
        {
            var result = new Result();
            foreach (var componentRef in componentRefs)
            {
                var componentDef = componentRef.Def;

                if (!componentDef.IsEnginePart())
                {
                    continue;
                }

                result.Parts.Add(componentRef);

                if (result.CoreRef == null && componentDef.IsEngineCore())
                {
                    result.CoreRef = componentRef.GetEngineCoreRef();
                    continue;
                }

                if (result.TypeDef == null)
                {
                    var engineType = Control.settings.EngineTypes.FirstOrDefault(c => componentDef.Description.Id == c.ComponentTypeID);
                    if (engineType != null)
                    {
                        result.TypeDef = new EngineTypeDef(componentDef, engineType);
                    }
                }
            }

            return result;
        }

        internal class Result
        {
            internal EngineCoreRef CoreRef;
            internal List<MechComponentRef> Parts = new List<MechComponentRef>();
            internal EngineTypeDef TypeDef;
        }
    }
}