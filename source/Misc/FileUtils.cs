using System.IO;

namespace MechEngineer.Misc;

internal static class FileUtils
{
    internal static void SetReadonly(string path, bool ro)
    {
        if (!File.Exists(path))
        {
            return;
        }
        var attributes = File.GetAttributes(path);
        if (ro)
        {
            attributes |= FileAttributes.ReadOnly;
        }
        else
        {
            attributes &= ~FileAttributes.ReadOnly;
        }
        File.SetAttributes(path, attributes);
    }
}