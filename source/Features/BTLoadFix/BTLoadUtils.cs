using System;
using System.Collections.Generic;
using System.Text;
using BattleTech;
using BattleTech.Data;

namespace MechEngineer
{
    class BTLoadUtils
    {
        internal static void PreloadComponents(DataManager manager)
        {
            var loadRequest = manager.CreateLoadRequest();
            loadRequest.AddAllOfTypeLoadRequest<HeatSinkDef>(BattleTechResourceType.HeatSinkDef, null);
            loadRequest.AddAllOfTypeLoadRequest<UpgradeDef>(BattleTechResourceType.UpgradeDef, null);
            loadRequest.AddAllOfTypeLoadRequest<WeaponDef>(BattleTechResourceType.WeaponDef, null);
            loadRequest.AddAllOfTypeLoadRequest<AmmunitionBoxDef>(BattleTechResourceType.AmmunitionBoxDef, null);
            loadRequest.ProcessRequests();
        }
    }
}
