using System;
using System.Collections.Generic;
using BattleTech;
using CustomComponents;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechComponent), "DamageComponent")]
    public static class MechComponent_DamageComponent_Patch
    {
        public static bool Prefix(MechComponent __instance, CombatGameState ___combat, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            try
            {
                if (__instance.mechComponentRef.Is<Flags>(out var f) && f.IsSet("ignore_damage"))
                {
                    MechCheckForCritPatch.Message = null;
                    return false;
                }

                if (!CirticalHitStatesHandler.Shared.ProcessWeaponHit(__instance, ___combat, hitInfo, damageLevel, applyEffects, MechCheckForCritPatch.MessageAdditions))
                {
                    MechCheckForCritPatch.Message = null;
                    return false;
                }

                if (!EngineCrits.ProcessWeaponHit(__instance, ___combat, hitInfo, damageLevel, applyEffects, MechCheckForCritPatch.MessageAdditions))
                {
                    MechCheckForCritPatch.Message = null;
                    return false;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            return true;
        }
    }

    public class MessageAddition
    {
        public string Text { get; set; }
        public FloatieMessage.MessageNature Nature { get; set; }
    }

    [HarmonyPatch(typeof(Mech), "CheckForCrit")]
    public static class MechCheckForCritPatch
    {
        static MechCheckForCritPatch()
        {
            MessageAdditions = new List<MessageAddition>();
        }

        public static MessageCenterMessage Message { get; set; }
        public static List<MessageAddition> MessageAdditions { get; set; }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Method(typeof(MessageCenter), "PublishMessage"),
                AccessTools.Method(typeof(MechCheckForCritPatch), "PublishMessage")
            );
        }

        public static void PublishMessage(this MessageCenter @this, MessageCenterMessage message)
        {
            Message = message;
        }

        public static void Postfix(Mech __instance)
        {
            try
            {
                if (Message != null)
                {
                    __instance.Combat.MessageCenter.PublishMessage(Message);
                    Message = null;
                }

                foreach (var addition in MessageAdditions)
                {
                    var message = new AddSequenceToStackMessage(new ShowActorInfoSequence(__instance, addition.Text, addition.Nature, true));
                    __instance.Combat.MessageCenter.PublishMessage(message);
                }

                MessageAdditions.Clear();
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}