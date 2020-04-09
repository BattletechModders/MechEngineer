namespace MechEngineer.Features.HardpointFix.prefab
{
    internal class Prefab
    {
        internal string Name { get; }
        internal int Index { get; }

        internal string Identifier { get; }
        internal int Group { get; }

        internal Prefab(int index, string name)
        {
            Name = name;
            Index = index;
            var parts = Name.Split('_');

            // chrPrfWeap_thunderbolt_righttorso_(ppc|gauss|..)_eh1
            Identifier = NormIdentifier(parts[3]);
            Group = int.Parse(name.Substring(name.Length - 1));
        }

        internal static string NormIdentifier(string identifier)
        {
            return identifier.ToLowerInvariant();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}