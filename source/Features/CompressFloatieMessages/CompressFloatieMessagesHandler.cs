using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BattleTech;
using Harmony;
using MechEngineer.Features.CompressFloatieMessages.Patches;
using MechEngineer.Misc;

namespace MechEngineer.Features.CompressFloatieMessages
{
    internal static class CompressFloatieMessagesHandler
    {
        internal static void SetupPatches()
        {
            FeatureUtils.SetupFeature(
                nameof(Features.CompressFloatieMessages),
                Control.settings.FeatureCompressFloatieMessagesEnabled,
                typeof(CombatHUDFloatieStack_AddFloatie_Patch)
            );
        }

        public static bool CompressFloatieMessages(FloatieMessage incoming, Queue<FloatieMessage> queue)
        {
            var incomingString = incoming.text.ToString();
            Control.mod.Logger.LogDebug($"Floatie {incomingString}");
            if (Control.settings.DebugDestroyedFloaties && !string.IsNullOrEmpty(incomingString) && incomingString.EndsWith("DESTROYED"))
            {
                Control.mod.Logger.LogError("COMPRESS DESTROYED " + new System.Diagnostics.StackTrace());
            }
            foreach (var message in queue)
            {
                // quick preliminary check
                if (!message.text.ToString().StartsWith(incomingString))
                {
                    continue;
                }

                var times = 1;
                
                // parse and remove multiplier from the end of the message
                var lastPart = message.text.m_parts.Last();
                var m = MultiplierRegex.Match(lastPart.text);
                if (m.Success)
                {
                    times = int.Parse(m.Groups[1].Value);
                    var parts = message.text.m_parts;
                    parts.RemoveAt(parts.Count - 1);
                }

                // actual check if the message contents are the same, so we can combine messages
                if (message.text.ToString() != incomingString)
                {
                    if (m.Success)
                    {
                        message.text.m_parts.Add(lastPart); // undo multiplier removal
                    }
                    continue;
                }

                // adding new multiplier
                message.text.Append(" x " + ++times);
                return true;
            }

            return false;
        }

        private static readonly Regex MultiplierRegex = new Regex("^ x (\\d+)$", RegexOptions.Compiled);
    }
}