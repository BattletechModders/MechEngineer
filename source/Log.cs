using HBS.Logging;
using ModTek.Public;

namespace MechEngineer;

internal static class Log
{
    private const string Name = nameof(MechEngineer);
    internal static readonly NullableLogger Main = NullableLogger.GetLogger(Name, LogLevel.Debug);
}
