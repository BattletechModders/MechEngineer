using BattleTech;

namespace MechEngineer.Features.AutoFix;

internal class WeaponDefChange
{
    public WeaponSubType Type;
    public ValueChange<int> SlotChange;
    public ValueChange<float> TonnageChange;
}