using System;
using BattleTech.UI;
using UnityEngine;

namespace MechEngineer.Features.MechLabSlots;

// finds original layout locations (layout_details is being moved -> therefore optional)
internal class MechLabLayoutFinder
{
    internal Transform Representation => representation.Value;
    internal Transform ObjActions => objActions.Value;
    internal Transform ObjCancelConfirm => objCancelConfirm.Value;
    internal Transform ObjHelpBttn => objHelpBttn.Value;
    internal Transform ObjEcmBttn => objEcmBttn.Value;
    internal Transform ObjWarnings => objWarnings.Value;
    internal Transform ObjMech => objMech.Value;
    internal Transform CenterLine => centerLine.Value;
    internal Transform? LayoutDetails => layoutDetails.Value;
    internal Transform ObjGroupLeft => objGroupLeft.Value;
    internal Transform ObjMeta => objMeta.Value;
    internal Transform ObjStatus => objStatus.Value;
    internal Transform LayoutTonnage => layoutTonnage.Value;

    public MechLabLayoutFinder(MechLabPanel panelComponent)
    {
        var panel = panelComponent.transform;

        representation = new(() => panel.Find("Representation"));
        objActions = new(() => Representation.Find("OBJ_actions"));
        objCancelConfirm = new(() => Representation.Find("OBJ_cancelconfirm"));
        objHelpBttn = new(() => Representation.Find("OBJ_helpBttn"));
        objEcmBttn = new(() => Representation.Find("OBJ_ECMBttn"));
        objWarnings = new(() => Representation.Find("OBJ_warnings"));
        objMech = new(() => Representation.Find("OBJ_mech"));
        centerLine = new(() => ObjMech.Find("Centerline"));
        layoutDetails = new(() => CenterLine.Find("layout_details"));
        objGroupLeft = new(() => Representation.Find("OBJGROUP_LEFT"));
        objMeta = new(() => ObjGroupLeft.Find("OBJ_meta"));
        objStatus = new(() => ObjMeta.Find("OBJ_status"));
        layoutTonnage = new(() => ObjStatus.Find("layout_tonnage"));
    }

    private readonly Lazy<Transform> representation;
    private readonly Lazy<Transform> objActions;
    private readonly Lazy<Transform> objCancelConfirm;
    private readonly Lazy<Transform> objHelpBttn;
    private readonly Lazy<Transform> objEcmBttn;
    private readonly Lazy<Transform> objWarnings;
    private readonly Lazy<Transform> objMech;
    private readonly Lazy<Transform> centerLine;
    private readonly Lazy<Transform?> layoutDetails;
    private readonly Lazy<Transform> objGroupLeft;
    private readonly Lazy<Transform> objMeta;
    private readonly Lazy<Transform> objStatus;
    private readonly Lazy<Transform> layoutTonnage;
}
