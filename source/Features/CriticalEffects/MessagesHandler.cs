using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BattleTech;
using CustomComponents;
using Localize;

namespace MechEngineer
{
    public static class MessagesHandler
    {
        public static bool CompressFloatieMessages(FloatieMessage incoming, Queue<FloatieMessage> queue)
        {
            var localizedText = incoming.text.ToString();
            Control.mod.Logger.LogDebug("Floatie " + DateTime.Now.ToString("hh.mm.ss.ffffff") + " " + localizedText);
            foreach (var message in queue)
            {
                if (message.text.ToString().StartsWith(localizedText))
                {
                    var m = MultiplierRegex.Match(message.text.m_parts.Last().text);
                    var times = 1;
                    if (m.Success)
                    {
                        times = int.Parse(m.Groups[1].Value);
                        var parts = message.text.m_parts;
                        parts.RemoveAt(parts.Count - 1);
                    }
                    message.text.Append(" x " + ++times);
                }

                return true;
            }

            return false;
        }
        private static readonly Regex MultiplierRegex = new Regex("^ x (\\d+)$", RegexOptions.Compiled);

        public static void PublishComponentState(MechComponent mechComponent)
        {
            if (mechComponent.DamageLevel == ComponentDamageLevel.Penalized)
            {
                var critMessage = new Text("{0} CRIT", mechComponent.UIName);
                if (mechComponent.componentDef.Is<CriticalEffects>(out var ce))
                {
                    if (ce.CritFloatieMessage != null)
                    {
                        if (ce.CritFloatieMessage == "")
                        {
                            return;
                        }
                        critMessage = new Text(ce.CritFloatieMessage);
                    }
                }
                mechComponent.PublishMessage(
                    critMessage,
                    FloatieMessage.MessageNature.CriticalHit
                );
            }
            else if (mechComponent.DamageLevel == ComponentDamageLevel.Destroyed)
            {
                // dont show destroyed if a mech is known to be incapacitated
                var actor = mechComponent.parent;
                if (actor.IsDead || actor.IsFlaggedForDeath)
                {
                    return;
                }

                // dont show destroyed if a whole section was destroyed, counters spam
                //if (actor is Mech mech)
                //{
                //    var location = mechComponent.mechComponentRef.MountedLocation;
                //    var mechLocationDestroyed = mech.IsLocationDestroyed(location);
                //    if (mechLocationDestroyed)
                //    {
                //        return;
                //    }
                //}

                var destroyedMessage = new Text("{0} DESTROYED", mechComponent.UIName);
                if (mechComponent.componentDef.Is<CriticalEffects>(out var ce))
                {
                    if (ce.DestroyedFloatieMessage != null)
                    {
                        if (ce.DestroyedFloatieMessage == "")
                        {
                            return;
                        }
                        destroyedMessage = new Text(ce.DestroyedFloatieMessage);
                    }
                }
                mechComponent.PublishMessage(
                    destroyedMessage,
                    FloatieMessage.MessageNature.ComponentDestroyed
                );
            }
        }

        private static void PublishMessage(this MechComponent component, Text message, FloatieMessage.MessageNature nature)
        {
            var actor = component.parent;
            if (actor == null)
            {
                return;
            }

            var stackMessage = new AddSequenceToStackMessage(new ShowActorInfoSequence(actor, message, nature, true));
            actor.Combat?.MessageCenter?.PublishMessage(stackMessage);
        }
    }
}
