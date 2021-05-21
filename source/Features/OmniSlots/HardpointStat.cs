using BattleTech;
using System;

namespace MechEngineer.Features.OmniSlots
{
    internal class HardpointStat
    {
        internal readonly WeaponCategoryValue Category;
        internal HardpointStat(WeaponCategoryValue category)
        {
            Category = category;
        }

        internal int VanillaUsage;
        internal int VanillaMax;
            
        internal int VanillaMaxOver => Math.Max(VanillaUsage - VanillaMax, 0);

        internal int DynamicMax => VanillaMax + VanillaMaxOver + OmniFree;

        internal int TheoreticalMax => VanillaMax + OmniMax;

        internal int OmniFree;
        internal int OmniMax;
        private bool OmniHas => OmniMax > 0;

        private string MaxString => OmniHas ? $"<color=#F79B26FF>{(DynamicMax > 0 ? DynamicMax : 0)}</color>" : $"{VanillaMax}";

        internal string HardpointString => VanillaUsage == 0 ? MaxString : $"{VanillaUsage}/{MaxString}";
        internal string HardpointStringWithSpace => VanillaUsage == 0 ? MaxString : $"{VanillaUsage} / {MaxString}";

        internal WeaponCategoryValue CategoryForLocationWidget => OmniHas || VanillaMax > 0 ? Category : WeaponCategoryEnumeration.GetNotSetValue();
    }
}