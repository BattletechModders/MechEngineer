using System.Collections.Generic;
using BattleTech;

namespace CustomComponents
{
    public class SimpleCustomComponentFactory<TCustomComponent> : CustomComponentFactory<TCustomComponent>
        where TCustomComponent : SimpleCustomComponent, new()
    {
        public override ICustomComponent Create(MechComponentDef target, Dictionary<string, object> values)
        {
            var obj = base.Create(target, values) as TCustomComponent;
            if (obj != null)
            {
                obj.Def = target;
            }
            return obj;
        }
    }
}