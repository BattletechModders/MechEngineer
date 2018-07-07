using BattleTech;
using fastJSON;

namespace CustomComponents
{
    public class SimpleCustomComponent : ICustomComponent
    {
        [JsonIgnore]
        public MechComponentDef Def { get; internal set; }
    }
}