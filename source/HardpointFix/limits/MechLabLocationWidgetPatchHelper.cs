using System.Linq;
using BattleTech;
using BattleTech.UI;
using UnityEngine.EventSystems;

namespace MechEngineer
{
    internal class MechLabLocationWidgetPatchHelper
    {
        private readonly MechLabLocationWidgetAdapter _adapter;

        internal MechLabLocationWidgetPatchHelper(MechLabLocationWidget widget)
        {
            _adapter = new MechLabLocationWidgetAdapter(widget);
        }

        internal bool MechLabLocationWidgetOnMechLabDrop(PointerEventData eventData)
        {
            if (!_adapter.MechLab.Initialized)
            {
                return true;
            }

            if (_adapter.MechLab.DragItem == null)
            {
                return true;
            }

            var dragItem = _adapter.MechLab.DragItem;
            var componentRef = dragItem.ComponentRef;

            if (componentRef == null || componentRef.ComponentDefType != ComponentType.Weapon)
            {
                return true;
            }

            if (Control.settings.HardpointFix.AllowDefaultLoadoutWeapons && ComponentIsPartOfDefaultLoadout(componentRef))
            {
                return true;
            }

            if (GetNotMappedPrefabNameCount(componentRef) > 0)
            {
                var dropErrorMessage = string.Format("Cannot add {0} to {1}: There are no available {2} hardpoints.",
                    componentRef.Def.Description.Name,
                    _adapter.LocationName.text,
                    componentRef.Def.PrefabIdentifier.ToUpper()
                );
                _adapter.MechLab.ShowDropErrorMessage(dropErrorMessage);
                _adapter.MechLab.OnDrop(eventData);
                return false;
            }

            return true;
        }

        private bool ComponentIsPartOfDefaultLoadout(MechComponentRef componentRef)
        {
            var chassisDef = _adapter.MechLab.activeMechDef.Chassis;
            var mechDefs = _adapter.MechLab.dataManager.MechDefs;
            foreach (var key in mechDefs.Keys)
            {
                var mechDef = mechDefs.Get(key);
                if (mechDef.Chassis != chassisDef)
                {
                    continue;
                }

                var location = _adapter.Loadout.Location;
                if (mechDef.Inventory.Any(c => c.MountedLocation == location && c.Def.PrefabIdentifier == componentRef.Def.PrefabIdentifier))
                {
                    //Control.mod.Logger.LogDebug(
                    //    "found component " + componentRef.Def.Description.Id
                    //             + " to be in use on mech " + mechDef.Description.Id
                    //             + " for location " + location);
                    return true;
                }
            }

            return false;
        }

        private int GetNotMappedPrefabNameCount(MechComponentRef newComponentRef)
        {
            var chassis = _adapter.MechLab.activeMechDef.Chassis;
            var location = _adapter.Loadout.Location;

            var componentRefs = _adapter.LocalInventory
                .Select(item => item.ComponentRef)
                .Where(c => c.Def.ComponentType == ComponentType.Weapon)
                .ToList();

            componentRefs.Add(newComponentRef);

            var calculator = new WeaponComponentPrefabCalculator(chassis, componentRefs, location);
            return calculator.NotMappedPrefabNameCount;
        }
    }
}