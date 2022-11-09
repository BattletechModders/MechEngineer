#nullable disable
using System;
using BattleTech;
using BattleTech.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MechEngineer.Features.MechLabSlots.FastMechStorage;

internal class FakeItem : IMechLabDraggableItem
{
    internal FakeItem(MechDef def)
    {
        MechDef = def;
        ChassisDef = def?.Chassis;
    }
    internal FakeItem(ChassisDef def)
    {
        ChassisDef = def;
    }

    internal bool HasId(string id)
    {
        if (MechDef != null && MechDef?.Description.Id == id)
        {
            return true;
        }
        return ChassisDef.Description.Id == id;
    }
    internal bool HasGUID(string GUID)
    {
        return MechDef != null && MechDef.GUID == GUID;
    }

    // interface implementations
    public MechDef MechDef { get; set; }
    public ChassisDef ChassisDef { get; set; }

    // these are NotImplementedException
    public bool RemoveFromParent(bool validate) => throw new NotImplementedException();
    public void RepairComponent(bool validate) => throw new NotImplementedException();
    public void SetDraggable(bool isDraggable) => throw new NotImplementedException();
    public GameObject GameObject => throw new NotImplementedException();
    public IMechLabDropTarget DropParent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public MechLabDropTargetType OriginalDropParentType => throw new NotImplementedException();
    public CanvasGroup CanvasGroup => throw new NotImplementedException();
    public bool HandledDrop { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool AllowDrag { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public void OnEndDrag(PointerEventData eventData) => throw new NotImplementedException();
    public void OnBeginDrag(PointerEventData eventData) => throw new NotImplementedException();
    public MechComponentRef ComponentRef => throw new NotImplementedException();
    public ChassisLocations MountedLocation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public MechLabDraggableItemType ItemType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}