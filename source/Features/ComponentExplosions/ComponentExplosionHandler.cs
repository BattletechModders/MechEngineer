using System.Linq;
using BattleTech;
using CustomComponents;
using Harmony;
using UnityEngine;

namespace MechEngineer
{
    internal class ComponentExplosionHandler
    {
        internal static ComponentExplosionHandler Shared = new ComponentExplosionHandler();

        internal void CheckForExplosion(MechComponent component, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            if (!applyEffects)
            {
                return;
            }
            
            if (damageLevel != ComponentDamageLevel.Destroyed)
            {
                return;
            }

            if (!(component.componentDef.Is<ComponentExplosion>(out var exp)))
            {
                return;
            }
            
            var explosionDamage = exp.ExplosionDamage;

            if (component is AmmunitionBox box)
            {
                var adapter = new MechComponentAdapter(component);
                var ammoCount = adapter.statCollection.GetValue<int>("CurrentAmmo");
                explosionDamage += ammoCount * exp.ExplosionDamagePerAmmo;
            }

            if (component is Weapon w2)
            {
                var ammoCount = w2.InternalAmmo;
                explosionDamage += ammoCount * exp.ExplosionDamagePerAmmo;
            }

            //Control.mod.Logger.LogDebug($"explosionDamage={explosionDamage}");

            if (Mathf.Approximately(explosionDamage, 0))
            {
                return;
            }

            if (!(component.parent is Mech mech))
            {
                return;
            }
            
            Mech_DamageLocation_Patch.IsInternalExplosion = true;
            try
            {
                var attackSequence = mech.Combat.AttackDirector.GetAttackSequence(hitInfo.attackSequenceId);
                attackSequence?.FlagAttackCausedAmmoExplosion();

                mech.PublishFloatieMessage($"{component.Name} EXPLOSION");
                if (mech.Combat.Constants.PilotingConstants.InjuryFromAmmoExplosion)
                {
                    var pilot = mech.GetPilot();
                    pilot?.SetNeedsInjury(InjuryReason.AmmoExplosion);
                }
            
                // this is very hacky as this is an invalid weapon
                var weapon = new Weapon(mech, mech.Combat, component.mechComponentRef, component.uid);

                // bool DamageLocation(int originalHitLoc, WeaponHitInfo hitInfo, ArmorLocation aLoc, Weapon weapon, float totalDamage, int hitIndex, AttackImpactQuality impactQuality)
                var args = new object[] {component.Location, hitInfo, (ArmorLocation) component.Location, weapon, explosionDamage, 0, AttackImpactQuality.Solid};
                Traverse.Create(mech).Method("DamageLocation", args).GetValue();
            }
            finally
            {
                Mech_DamageLocation_Patch.IsInternalExplosion = false;
            }
        }

        internal CASEComponent GetCASEProperties(Mech mech, int location)
        {
            return mech.allComponents
                .Where(c => c.DamageLevel == ComponentDamageLevel.Functional)
                .Select(componentRef => new { componentRef, CASE = componentRef.componentDef.GetComponent<CASEComponent>() } )
                .Where(t => t.CASE != null)
                .Where(t => t.CASE.AllLocations || t.componentRef.Location == location)
                .Select(t => t.CASE)
                .OrderBy(CASE => CASE.AllLocations) // localized CASE always overrides global CASE
                .FirstOrDefault();
        }
    }

    public class MechComponentAdapter : Adapter<MechComponent>
    {
        public MechComponentAdapter(MechComponent instance) : base(instance)
        {
        }

        public StatCollection statCollection => traverse.Field("statCollection").GetValue<StatCollection>();
    }
}
