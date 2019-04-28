using BattleTech;
using BattleTech.Data;

namespace MechEngineer.Features.BattleTechLoadFix
{
    internal class BattleTechLoadFixFeature : Feature
    {
        internal static BattleTechLoadFixFeature Shared = new BattleTechLoadFixFeature();

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
