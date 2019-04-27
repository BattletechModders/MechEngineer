using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BattleTech;
using Harmony;
using UnityEngine;

namespace MechEngineer
{
    internal class NewSaveFolderHandlers
    {
        internal static NewSaveFolderHandlers Shared = new NewSaveFolderHandlers();

        private static string PathByKey(string key)
        {
            return Path.Combine(SavesPath, key + ".pref");
        }
        internal static string PathByPlatform(bool usePlatform)
        {
            return Path.Combine(SavesPath, usePlatform ? "cloud" : "local");
        }

        internal static string SavesPath => Path.Combine(ModsPath, "Saves");

        internal static string ModsPath // copied from ModTek sources
        {
            get
            {
                var manifestDirectory = Path.GetDirectoryName(VersionManifestUtilities.MANIFEST_FILEPATH);
                if (manifestDirectory == null)
                {
                    throw new InvalidOperationException();
                }

                return Path.GetFullPath(
                    Path.Combine(manifestDirectory,
                        Path.Combine(
                            Path.Combine(
                                Path.Combine("..", "..")
                                ,
                                ".."
                            ),
                            "Mods")
                    )
                );
            }
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .MethodReplacer(
                    AccessTools.Method(typeof(PlayerPrefs), nameof(PlayerPrefs.HasKey)),
                    AccessTools.Method(typeof(NewSaveFolderHandlers), nameof(HasKey))
                )
                .MethodReplacer(
                    AccessTools.Method(typeof(PlayerPrefs), nameof(PlayerPrefs.GetString), new []{typeof(string), typeof(string)}),
                    AccessTools.Method(typeof(NewSaveFolderHandlers), nameof(GetString))
                )
                .MethodReplacer(
                    AccessTools.Method(typeof(PlayerPrefs), nameof(PlayerPrefs.DeleteKey)),
                    AccessTools.Method(typeof(NewSaveFolderHandlers), nameof(DeleteKey))
                )
                .MethodReplacer(
                    AccessTools.Method(typeof(PlayerPrefs), nameof(PlayerPrefs.SetString)),
                    AccessTools.Method(typeof(NewSaveFolderHandlers), nameof(SetString))
                );
        }

        internal static bool HasKey(string key)
        {
            return File.Exists(PathByKey(key));
        }

        internal static string GetString(string key, string defaultValue)
        {
            try
            {
                if (HasKey(key))
                {
                    return File.ReadAllText(PathByKey(key));
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return defaultValue;
        }

        internal static void DeleteKey(string key)
        {
            try
            {
                if (HasKey(key))
                {
                    File.Delete(PathByKey(key));
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        internal static void SetString(string key, string value)
        {
            try
            {
                var path = PathByKey(key);
                var dir = Path.GetDirectoryName(path);
                Directory.CreateDirectory(dir ?? throw new InvalidOperationException());
                File.WriteAllText(path, value, Encoding.UTF8);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        } 
    }
}