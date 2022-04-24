using BattleTech;

namespace MechEngineer.Features.PlaceholderEffects;

internal abstract class PlaceholderInterpolation
{
    protected PlaceholderInterpolation(MechComponent mechComponent)
    {
        MechComponent = mechComponent;
    }
    internal MechComponent MechComponent { get; }

    protected const string LocationPlaceholder = "{location}";

    private static string ComponentUIDPlaceholder => PlaceholderEffectsFeature.Shared.Settings.ComponentEffectStatisticPlaceholder;

    internal abstract string LocationId { get; }

    internal virtual string InterpolateEffectId(string id)
    {
        return id.Replace(ComponentUIDPlaceholder, MechComponent.uid);
    }
    internal abstract string InterpolateStatisticName(string id);
    internal abstract string InterpolateText(string text);

    private static bool HasPlaceholders(string text)
    {
        return text.Contains(LocationPlaceholder) || text.Contains(ComponentUIDPlaceholder);
    }

    internal static bool Create(string effectId, MechComponent mechComponent, out PlaceholderInterpolation naming)
    {
        if (!HasPlaceholders(effectId))
        {
            naming = null;
            return false;
        }

        if (mechComponent.parent is Mech)
        {
            naming = new MechPlaceholderInterpolation(mechComponent);
            return true;
        }

        if (mechComponent.parent is Vehicle)
        {
            naming = new VehiclePlaceholderInterpolation(mechComponent);
            return true;
        }

        naming = null;
        return false;
    }
}