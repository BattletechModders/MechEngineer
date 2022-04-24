namespace MechEngineer.Features.DebugSaveMechToFile;

internal class DebugSaveMechToFileFeature : Feature<DebugSaveMechToFileSettings>
{
    internal static readonly DebugSaveMechToFileFeature Shared = new();

    internal override DebugSaveMechToFileSettings Settings => Control.settings.DebugSaveMechToFile;
}