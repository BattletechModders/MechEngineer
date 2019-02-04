using System;
using System.Collections.Generic;
using System.IO;
using BattleTech;
using HBS.Util;

namespace MechEngineer
{
    public partial class BonusDescriptions
    {
        public static class SettingsResourcesTools
        {
            private class SettingsResource<T>
            {
#pragma warning disable 649
                public List<T> Settings;
#pragma warning restore 649
            }

            public static IEnumerable<T> Enumerate<T>(
                string resourceType,
                Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
            {
                if (customResources == null)
                {
                    Control.mod.Logger.Log($"customResources not found");
                    yield break;
                }

                if (!customResources.TryGetValue(resourceType, out var entries))
                {
                    Control.mod.Logger.Log($"{resourceType} not found");
                    yield break;
                }

                foreach (var entry in entries.Values)
                {
                    var settings = new SettingsResource<T>();
                    try
                    {
                        using (var reader = new StreamReader(entry.FilePath))
                        {
                            var json = reader.ReadToEnd();
                            JSONSerializationUtility.FromJSON(settings, json);
                        }
                    }
                    catch (Exception e)
                    {
                        Control.mod.Logger.LogError($"Couldn't read {entry.FilePath}", e);
                    }

                    if (settings.Settings == null)
                    {
                        Control.mod.Logger.LogWarning($"Settings is null in {entry.FilePath}");
                        continue;
                    }

                    foreach (var settingsEntry in settings.Settings)
                    {
                        yield return settingsEntry;
                    }
                }
            }
        }
    }
}