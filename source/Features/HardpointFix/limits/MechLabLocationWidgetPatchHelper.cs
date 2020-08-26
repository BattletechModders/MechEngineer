using System.Linq;
using BattleTech;
using BattleTech.UI;
using MechEngineer.Features.HardpointFix.utils;
using UnityEngine.EventSystems;

namespace MechEngineer.Features.HardpointFix.limits
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

            if (HasUnmappedPrefabs(componentRef))
            {
                var dropErrorMessage = $"Cannot add {componentRef.Def.Description.Name} to {_adapter.LocationName.text}: There are no available {componentRef.Def.PrefabIdentifier.ToUpper()} hardpoints.";
                _adapter.MechLab.ShowDropErrorMessage(new Localize.Text(dropErrorMessage));
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
                    Control.Logger.Debug?.Log($"found component {componentRef.Def.Description.Id} to be in use on mech {mechDef.Description.Id} for location {location}");
                    return true;
                }
            }

            return false;
        }

        private bool HasUnmappedPrefabs(MechComponentRef newComponentRef)
        {
            var chassis = _adapter.MechLab.activeMechDef.Chassis;
            var location = _adapter.Loadout.Location;

            var componentRefs = _adapter.LocalInventory
                .Select(item => item.ComponentRef)
                .Where(c => c.Def.ComponentType == ComponentType.Weapon)
                .ToList();

            componentRefs.Add(newComponentRef);

            var calculator = new WeaponComponentPrefabCalculator(chassis, componentRefs, location);
            return calculator.MappedComponentRefCount != componentRefs.Count;
        }
    }
}