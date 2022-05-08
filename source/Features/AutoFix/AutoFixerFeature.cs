using System;
using CustomComponents;
using HBS.Collections;

namespace MechEngineer.Features.AutoFix;

internal class AutoFixerFeature : Feature<AutoFixerSettings>
{
    internal static readonly AutoFixerFeature Shared = new();

    internal override AutoFixerSettings Settings => Control.Settings.AutoFixer;

    internal static AutoFixerSettings settings => Shared.Settings;

    protected override void SetupFeatureLoaded()
    {
        Registry.RegisterPreProcessor(CockpitHandler.Shared);
        Registry.RegisterPreProcessor(SensorsAHandler.Shared);
        Registry.RegisterPreProcessor(SensorsBHandler.Shared);
        Registry.RegisterPreProcessor(GyroHandler.Shared);
        Registry.RegisterPreProcessor(LegActuatorHandler.Shared);

        CustomComponents.AutoFixer.Shared.RegisterMechFixer(AutoFixer.Shared.AutoFix);
    }

    private readonly Lazy<TagSet> IgnoreAutofixTagsLazy = new Lazy<TagSet>(() => new TagSet(settings.IgnoreAutofixTags));
    internal TagSet IgnoreAutofixTags => IgnoreAutofixTagsLazy.Value;
}