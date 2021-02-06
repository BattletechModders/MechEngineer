namespace MechEngineer.Features.DamageIgnore
{
    internal class DamageIgnoreFeature : Feature<DamageIgnoreSettings>
    {
        internal const string Namespace = nameof(MechEngineer) + "." + nameof(Features) + "." + nameof(DamageIgnore);

        internal static DamageIgnoreFeature Shared = new();

        internal override DamageIgnoreSettings Settings => Control.settings.DamageIgnore;
    }
}