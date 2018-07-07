using System.Collections.Generic;
using BattleTech;

namespace CustomComponents
{
    public interface ICustomComponentFactory
    {
        ICustomComponent Create(MechComponentDef target, Dictionary<string, object> values);
    }
}