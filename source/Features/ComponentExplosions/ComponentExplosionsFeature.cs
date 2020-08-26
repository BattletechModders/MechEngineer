using System.Linq;
using BattleTech;
using CustomComponents;
using UnityEngine;

namespace MechEngineer.Features.ComponentExplosions
{
    internal class ComponentExplosionsFeature: Feature<ComponentExplosionsSettings>
    {
        internal static ComponentExplosionsFeature Shared = new ComponentExplosionsFeature();

        internal override ComponentExplosionsSettings Settings => Control.settings.ComponentExplosions;

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

            var actor = component.parent;

            var ammoCount = 0;
            if (component is AmmunitionBox box)
            {
                var adapter = new MechComponentAdapter(box);
                ammoCount = adapter.statCollection.GetValue<int>("CurrentAmmo");
            }
            else if (component is Weapon w2)
            {
                ammoCount = Mathf.Max(w2.InternalAmmo, Mathf.Min(w2.ShotsWhenFired, w2.CurrentAmmo));
            }
            
            var attackSequence = actor.Combat.AttackDirector.GetAttackSequence(hitInfo.attackSequenceId);

            var heatDamage = exp.HeatDamage + ammoCount * exp.HeatDamagePerAmmo;
            Control.Logger.Debug?.Log($"heatDamage={heatDamage}");
            if (!Mathf.Approximately(heatDamage, 0))
            {
                actor.AddExternalHeat("AMMO EXPLOSION HEAT", (int)heatDamage);
                attackSequence?.FlagAttackDidHeatDamage(actor.GUID);
            }

            { // only applies for mechs, vehicles don't have stability
                if (actor is Mech mech)
                {
                    var stabilityDamage = exp.StabilityDamage + ammoCount * exp.StabilityDamagePerAmmo;
                    Control.Logger.Debug?.Log($"stabilityDamage={stabilityDamage}");
                    if (!Mathf.Approximately(stabilityDamage, 0))
                    {
                        mech.AddAbsoluteInstability(stabilityDamage, StabilityChangeSource.Effect, hitInfo.targetId);
                    }
                }
            }

            var explosionDamage = exp.ExplosionDamage + ammoCount * exp.ExplosionDamagePerAmmo;
            Control.Logger.Debug?.Log($"explosionDamage={explosionDamage}");

            if (Mathf.Approximately(explosionDamage, 0))
            {
                return;
            }

            IsInternalExplosion = true;
            try
            {
                attackSequence?.FlagAttackCausedAmmoExplosion(actor.GUID);

                actor.PublishFloatieMessage($"{component.Name} EXPLOSION");
                if (actor.Combat.Constants.PilotingConstants.InjuryFromAmmoExplosion)
                {
                    var pilot = actor.GetPilot();
                    pilot?.SetNeedsInjury(InjuryReason.AmmoExplosion);
                }

                if (actor is Mech mech)
                {
                    // this is very hacky as this is an invalid weapon
                    var weapon = new Weapon(mech, actor.Combat, component.mechComponentRef, component.uid);

                    mech.DamageLocation(component.Location, hitInfo, (ArmorLocation) component.Location, weapon, 0, explosionDamage, 0, AttackImpactQuality.Solid, DamageType.AmmoExplosion);
                }
                else if (actor is Vehicle vehicle)
                {
                    // this is very hacky as this is an invalid weapon
                    var weapon = new Weapon(vehicle, actor.Combat, component.vehicleComponentRef, component.uid);

                    vehicle.DamageLocation(hitInfo, component.Location, (VehicleChassisLocations)component.Location, weapon, 0, explosionDamage, AttackImpactQuality.Solid);
                }
                else if (actor is Turret turret)
                {
                    // this is very hacky as this is an invalid weapon
                    var weapon = new Weapon(turret, actor.Combat, component.turretComponentRef, component.uid);

                    turret.DamageLocation(hitInfo, (BuildingLocation)component.Location, weapon, 0, explosionDamage);
                }
            }
            finally
            {
                IsInternalExplosion = false;
            }
        }

        internal static bool IsInternalExplosion;

        internal CASEComponent GetCASEProperties(AbstractActor actor, int location)
        {
            return actor.allComponents
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
