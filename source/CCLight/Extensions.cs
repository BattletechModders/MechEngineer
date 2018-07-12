using BattleTech;

namespace CustomComponents
{
    public static class Extensions
    {
        public static T GetComponent<T>(this MechComponentDef def) where T: class, ICustomComponent
        {
            return Database.GetCustomComponent<T>(def);
        }

        public static bool Is<T>(this MechComponentDef def, out T res) where T: class, ICustomComponent
        {
            res = Database.GetCustomComponent<T>(def);
            return res != null;
        }
    }
}