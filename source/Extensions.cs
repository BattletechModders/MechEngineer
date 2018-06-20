using System;
using BattleTech;
using UnityEngine;

namespace MechEngineMod
{
    internal static class Extensions
    {
        // main engine + engine slots
        internal static bool IsEnginePart(this MechComponentDef componentDef)
        {
            return CheckComponentDef(componentDef, ComponentType.HeatSink, Control.settings.EnginePartPrefix);
        }

        // only main engine
        internal static bool IsMainEngine(this MechComponentDef componentDef)
        {
            return CheckComponentDef(componentDef, ComponentType.HeatSink, Control.settings.MainEnginePrefix);
        }

        // engine category to use for auto fixing chassis
        internal static bool IsAutoFixEngine(this MechComponentDef componentDef)
        {
            return CheckComponentDef(componentDef, ComponentType.HeatSink, Control.settings.AutoFixEnginePrefix);
        }

        // we want to know about center torso upgrade (gyros), since we reduce their size
        internal static bool IsGyro(this MechComponentDef componentDef)
        {
            return componentDef.AllowedLocations == ChassisLocations.CenterTorso
                   && componentDef.ComponentType == ComponentType.Upgrade
                   && componentDef.Description.Id.StartsWith(Control.settings.GearGryoPrefix);
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


        internal static EngineDef GetEngineDef(this MechComponentDef @this)
        {
            if (@this == null || !@this.IsMainEngine())
            {
                return null;
            }

            return new EngineDef(@this);
        }


        internal static EngineRef GetEngineRef(this MechComponentRef @this)
        {
            if (@this == null || @this.Def == null)
            {
                return null;
            }

            var engineDef = @this.Def.GetEngineDef();
            if (engineDef == null)
            {
                return null;
            }

            return new EngineRef(@this, engineDef);
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
