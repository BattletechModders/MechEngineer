using BattleTech;
using System;

namespace MechEngineer.Features.OmniSlots
{
    internal class HardpointStat
    {
        internal readonly WeaponCategory Category;
        internal HardpointStat(WeaponCategory category)
        {
            Category = category;
        }

        internal int VanillaUsage;
        internal int VanillaMax;
        internal bool VanillaHas => VanillaMax > 0;
            
        internal int VanillaMaxOver => Math.Max(VanillaUsage - VanillaMax, 0);

        internal int DynamicMax => VanillaMax + VanillaMaxOver + OmniFree;
        internal int DynamicFree => DynamicMax - VanillaUsage;

        internal int OmniFree;
        internal int OmniMax;
        internal bool OmniHas => OmniMax > 0;

        internal string MaxString => OmniHas ? $"<color=#F79B26FF>{(DynamicMax > 0 ? DynamicMax : 0)}</color>" : $"{VanillaMax}";

        public string HardpointString => $"{VanillaUsage}/{MaxString}";
        public string HardpointTotalString => OmniHas ? (VanillaHas ? $"<color=#F79B26FF>{VanillaMax}+{OmniMax}</color>" : $"<color=#F79B26FF>{OmniMax}</color>") : $"{VanillaMax}";

        public WeaponCategory CategoryForLocationWidget => OmniHas || VanillaMax > 0 ? Category : WeaponCategory.NotSet;
    }
}