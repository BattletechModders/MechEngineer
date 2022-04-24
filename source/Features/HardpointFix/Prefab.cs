namespace MechEngineer.Features.HardpointFix;

internal class Prefab
{
    internal string Name { get; }
    internal string Identifier { get; }

    internal Prefab(string name)
    {
        Name = name;
        var parts = Name.Split('_');

        // chrPrfWeap_thunderbolt_righttorso_(ppc|gauss|..)_eh1
        Identifier = NormIdentifier(parts[3]);
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