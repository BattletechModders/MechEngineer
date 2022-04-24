using System;
using CustomComponents;
using HBS.Collections;

namespace MechEngineer.Features.AutoFix;

internal class AutoFixerFeature : Feature<AutoFixerSettings>
{
    internal static readonly AutoFixerFeature Shared = new();

    internal override AutoFixerSettings Settings => Control.settings.AutoFixer;

    internal static AutoFixerSettings settings => Shared.Settings;

    internal override void SetupFeatureLoaded()
    {
        Registry.RegisterPreProcessor(CockpitHandler.Shared);
        Registry.RegisterPreProcessor(SensorsAHandler.Shared);
        Registry.RegisterPreProcessor(SensorsBHandler.Shared);
        Registry.RegisterPreProcessor(GyroHandler.Shared);
        Registry.RegisterPreProcessor(LegActuatorHandler.Shared);

        CustomComponents.AutoFixer.Shared.RegisterMechFixer(AutoFixer.Shared.AutoFix);
    }

    public AutoFixerFeature()
    {
        IgnoreAutofixTagsLazy = new Lazy<TagSet>(() => new TagSet(Settings.IgnoreAutofixTags));
    }
    internal readonly Lazy<TagSet> IgnoreAutofixTagsLazy;
    internal TagSet IgnoreAutofixTags => IgnoreAutofixTagsLazy.Value;
}