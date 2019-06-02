using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BattleTech;

namespace MechEngineer.Features.CompressFloatieMessages
{
    internal class CompressFloatieMessagesFeature : Feature
    {
        internal static CompressFloatieMessagesFeature Shared = new CompressFloatieMessagesFeature();

        internal override bool Enabled => settings?.Enabled ?? false;

        internal static Settings settings => Control.settings.CompressFloatieMessages;

        public class Settings
        {
            public bool Enabled = false;
            public bool DebugDestroyedFloaties = false;
        }

        public static bool CompressFloatieMessages(FloatieMessage incoming, Queue<FloatieMessage> queue)
        {
            var incomingString = incoming.text.ToString();
            Control.mod.Logger.LogDebug($"Floatie {incomingString}");
            if (settings.DebugDestroyedFloaties && !string.IsNullOrEmpty(incomingString) && incomingString.EndsWith("DESTROYED"))
            {
                Control.mod.Logger.LogDebug("DEBUG DESTROYED FLOATIE " + new System.Diagnostics.StackTrace());
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