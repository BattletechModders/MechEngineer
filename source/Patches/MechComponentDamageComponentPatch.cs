using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechComponent), "DamageComponent")]
    public static class MechComponentDamageComponentPatch
    {
        // crit engine reduces speed
        // destroyed engine destroys CT
        public static bool Prefix(MechComponent __instance, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            try
            {
                if (!EngineCrits.ProcessWeaponHit(__instance, hitInfo, damageLevel, applyEffects, MechCheckForCritPatch.MessageAdditions))
                {
                    MechCheckForCritPatch.Message = null;
                    return false;
                }

                if (!ArmorStructure.ProcessWeaponHit(__instance, hitInfo, damageLevel, applyEffects))
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
        public static MessageCenterMessage Message { get; set; }
        public static List<MessageAddition> MessageAdditions { get; set; }

        static MechCheckForCritPatch()
        {
            MessageAdditions = new List<MessageAddition>();
        }

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