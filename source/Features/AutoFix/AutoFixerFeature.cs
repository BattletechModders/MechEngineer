using System;
using HBS.Collections;

namespace MechEngineer.Features.AutoFix;

internal class AutoFixerFeature : Feature<AutoFixerSettings>
{
    internal static readonly AutoFixerFeature Shared = new();

    internal override AutoFixerSettings Settings => Control.Settings.AutoFixer;

    internal static AutoFixerSettings settings => Shared.Settings;

    protected override void SetupFeatureLoaded()
    {
        CustomComponents.AutoFixer.Shared.RegisterMechFixer(AutoFixer.Shared.AutoFix);
    }

    private readonly Lazy<TagSet> IgnoreAutofixTagsLazy = new(() => new(settings.IgnoreAutofixTags));
    internal TagSet IgnoreAutofixTags => IgnoreAutofixTagsLazy.Value;
}