using System;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer.Features.Globals;

// ReSharper disable Unity.NoNullPropagation
internal static class Global
{
    internal static MechDef? ActiveMechDef => ActiveMechDefFromLab ?? SelectedMechDefFromBay;

    private static MechDef? ActiveMechDefFromLab => ActiveMechLabPanel?.CreateMechDef();
    private static readonly WeakReference MechLabPanelReference = new(null);
    internal static MechLabPanel? ActiveMechLabPanel
    {
        set => MechLabPanelReference.Target = value;
        private get => MechLabPanelReference.Target as MechLabPanel;
    }

    private static MechDef? SelectedMechDefFromBay => ActiveMechBayPanel?.selectedMech?.MechDef;
    private static readonly WeakReference MechBayPanelReference = new(null);
    internal static MechBayPanel? ActiveMechBayPanel
    {
        set => MechBayPanelReference.Target = value;
        private get => MechBayPanelReference.Target as MechBayPanel;
    }
}
