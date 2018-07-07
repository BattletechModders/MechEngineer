using BattleTech;

namespace CustomComponents
{
    public static class Extensions
    {
        public static T GetComponent<T>(this MechComponentDef def) where T: class, ICustomComponent
        {
            return Database.GetCustomComponent(def, typeof(T)) as T;
        }
    }
}