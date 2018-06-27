using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using UnityEngine;

namespace MechEngineer
{
    internal static class Extensions
    {
        // main engine + engine slots
        internal static bool IsEnginePart(this MechComponentDef componentDef)
        {
            return CheckComponentDef(componentDef, ComponentType.HeatSink, Control.settings.EnginePartPrefix);
        }

        // engine slots
        internal static bool IsEngineSlots(this MechComponentDef componentDef)
        {
            return CheckComponentDef(componentDef, ComponentType.HeatSink, Control.settings.EngineSlotPrefix);
        }

        // engine center slots
        internal static bool IsEngineCenterSlots(this MechComponentDef componentDef)
        {
            return componentDef.AllowedLocations == ChassisLocations.CenterTorso && IsEngineSlots(componentDef);
        }

        // only main engine
        internal static bool IsEngineCore(this MechComponentDef componentDef)
        {
            return CheckComponentDef(componentDef, ComponentType.HeatSink, Control.settings.EngineCorePrefix);
        }

        // we want to know about center torso upgrade (gyros), since we reduce their size
        internal static bool IsGyro(this MechComponentDef componentDef)
        {
            return componentDef.AllowedLocations == ChassisLocations.CenterTorso
                   && componentDef.ComponentType == ComponentType.Upgrade
                   && componentDef.Description.Id.StartsWith(Control.settings.GearGryoPrefix);
        }

        // we want to know about leg upgrades, since we reduce their size
        internal static bool IsLegUpgrade(this MechComponentDef componentDef)
        {
            return componentDef.OnlyAllowedIn(ChassisLocations.Legs)
                   && componentDef.ComponentType == ComponentType.Upgrade;
        }

        internal static bool OnlyAllowedIn(this MechComponentDef componentDef, ChassisLocations locations)
        {
            return (componentDef.AllowedLocations & locations) != 0 // can be inserted in locations
                   && (componentDef.AllowedLocations & locations) == 0; // can't be inserted anywhere outside of locations
        }

        // we want to know about center torso upgrade (gyros), since we reduce their size
        internal static bool IsCockpit(this MechComponentDef componentDef)
        {
            return componentDef.AllowedLocations == ChassisLocations.Head
                   && componentDef.ComponentType == ComponentType.Upgrade
                   && componentDef.Description.Id.StartsWith(Control.settings.GearCockpitPrefix);
        }

        // endo steel has some calculations behind it
        internal static bool IsStructure(this MechComponentDef componentDef)
        {
            return CheckComponentDef(componentDef, ComponentType.Upgrade, Control.settings.StructurePrefix);
        }

        // ferros fibrous has some calculations behind it
        internal static bool IsArmor(this MechComponentDef componentDef)
        {
            return CheckComponentDef(componentDef, ComponentType.Upgrade, Control.settings.ArmorPrefix);
        }
        
        private static bool CheckComponentDef(MechComponentDef def, ComponentType type, string prefix)
        {
            if (def.ComponentType != type)
            {
                return false;
            }

            if (def.Description == null || def.Description.Id == null)
            {
                return false;
            }

            return def.Description.Id.StartsWith(prefix);
        }

        private static bool CheckChassisDef(ChassisDef def, string prefix)
        {

            if (def.Description == null || def.Description.Id == null)
            {
                return false;
            }

            return def.Description.Id.StartsWith(prefix);
        }

        internal static bool IsDouble(this HeatSinkDef def)
        {
            return def.Description.Id == Control.settings.GearHeatSinkDouble;
        }

        internal static bool IsSingle(this HeatSinkDef def)
        {
            return def.Description.Id == Control.settings.GearHeatSinkStandard;
        }

        internal static bool IsDHSKit(this HeatSinkDef def)
        {
            return def.Description.Id == Control.settings.EngineKitDHS;
        }


        internal static EngineCoreDef GetEngineCoreDef(this MechComponentDef @this)
        {
            if (@this == null || !@this.IsEngineCore())
            {
                return null;
            }

            return new EngineCoreDef(@this);
        }

        internal static EngineCoreRef GetEngineCoreRef(this MechDef @this)
        {
            return GetEngineCoreRef(@this.Inventory);
        }

        internal static EngineCoreRef GetEngineCoreRef(this IEnumerable<MechComponentRef> componentRefs)
        {
            EngineCoreDef engineDef = null;
            MechComponentRef enginComponentRef = null;
            EngineType engineType = null;
            foreach (var componentRef in componentRefs)
            {
                var componentDef = componentRef.Def;

                if (!componentDef.IsEnginePart())
                {
                    continue;
                }

                if (engineDef == null)
                {
                    engineDef = componentDef.GetEngineCoreDef();
                    if (engineDef != null)
                    {
                        enginComponentRef = componentRef;
                    }
                }

                if (engineType == null)
                {
                    engineType = Control.settings.EngineTypes.FirstOrDefault(c => componentDef.Description.Id == c.ComponentTypeID);
                }

                if (engineDef != null && engineType != null)
                {
                    break;
                }
            }

            if (engineDef == null)
            {
                return null;
            }

            return new EngineCoreRef(enginComponentRef, engineDef, engineType);
        }

        internal static EngineCoreRef GetEngineCoreRef(this MechComponentRef @this, EngineType engineType)
        {
            if (@this == null || @this.Def == null)
            {
                return null;
            }

            var engineDef = @this.Def.GetEngineCoreDef();
            if (engineDef == null)
            {
                return null;
            }

            return new EngineCoreRef(@this, engineDef, engineType);
        }

        internal static void PerformOperation(this StatCollection collection, Statistic statistic, StatisticEffectData data)
        {
            var type = Type.GetType(data.modType);
            var variant = new Variant(type);
            variant.SetValue(data.modValue);
            variant.statName = data.statName;
            collection.PerformOperation(statistic, data.operation, variant);
        }

        internal static float RoundStandard(this float @this)
        {
            if (Control.settings.FractionalAccounting)
            {
                // kg rounding
                return Mathf.Round(@this * 1000) / 1000;
            }
            else
            {
                // half ton rounding
                return Mathf.Round(@this * 2) / 2;
            }
        }

        internal static float RoundBy5(this float @this)
        {
            return Mathf.Round(@this / 5) * 5;
        }
    }
}
