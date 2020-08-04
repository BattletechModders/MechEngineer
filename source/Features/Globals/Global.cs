using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.Globals
{
    internal static class Global
    {
        internal static MechDef ActiveMechDef => ActiveMechDefFromLab ?? SelectedMechDefFromBay;

        #region MechLabPanel

        internal static MechDef ActiveMechDefFromLab => ActiveMechLabPanel?.CreateMechDef();
        internal static readonly WeakReference MechLabPanelReference = new WeakReference(null);
        internal static MechLabPanel ActiveMechLabPanel
        {
            set => MechLabPanelReference.Target = value;
            get => MechLabPanelReference.Target as MechLabPanel;
        }

        #endregion

        #region MechBayPanel

        internal static MechDef SelectedMechDefFromBay => 
            ActiveMechBayPanel == null ? null :
                Traverse.Create(ActiveMechBayPanel).Field("selectedMech").GetValue<MechBayMechUnitElement>()?.MechDef;
        internal static readonly WeakReference MechBayPanelReference = new WeakReference(null);
        internal static MechBayPanel ActiveMechBayPanel
        {
            set => MechBayPanelReference.Target = value;
            get => MechBayPanelReference.Target as MechBayPanel;
        }

        #endregion
    }
}