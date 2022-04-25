using BattleTech;

namespace MechEngineer.Features.ArmorMaximizer;

class ArmorState
{
    internal readonly bool CanMaxArmor;
    internal readonly float MaxArmorPoints;
    internal readonly float CurrentArmorPoints;
    internal readonly float AvailableArmorPoints;
    internal readonly float H_MaxAP;
    internal readonly float H_AssignedAP;
    internal readonly float CT_MaxAP;
    internal readonly float CT_AssignedAP;
    internal readonly float LT_MaxAP;
    internal readonly float LT_AssignedAP;
    internal readonly float RT_MaxAP;
    internal readonly float RT_AssignedAP;
    internal readonly float LA_MaxAP;
    internal readonly float LA_AssignedAP;
    internal readonly float RA_MaxAP;
    internal readonly float RA_AssignedAP;
    internal readonly float LL_MaxAP;
    internal readonly float LL_AssignedAP;
    internal readonly float RL_MaxAP;
    internal readonly float RL_AssignedAP;
    internal ArmorState(MechDef mechDef)
    {
        CanMaxArmor = mechDef.CanMaxArmor();
        MaxArmorPoints = mechDef.MaxArmorPoints(); 
        CurrentArmorPoints = mechDef.CurrentArmorPoints();
        AvailableArmorPoints = mechDef.AvailableAP();
        H_MaxAP = mechDef.CalcMaxAPbyLocation(mechDef.Head, mechDef.Chassis.Head);
        H_AssignedAP = mechDef.AssignAPbyLocation(mechDef.Head, mechDef.Chassis.Head);
        CT_MaxAP = mechDef.CalcMaxAPbyLocation(mechDef.CenterTorso, mechDef.Chassis.CenterTorso);
        CT_AssignedAP = mechDef.AssignAPbyLocation(mechDef.CenterTorso, mechDef.Chassis.CenterTorso);
        LT_MaxAP = mechDef.CalcMaxAPbyLocation(mechDef.LeftTorso, mechDef.Chassis.LeftTorso);
        LT_AssignedAP = mechDef.AssignAPbyLocation(mechDef.LeftTorso, mechDef.Chassis.LeftTorso);
        RT_MaxAP = mechDef.CalcMaxAPbyLocation(mechDef.RightTorso, mechDef.Chassis.RightTorso);
        RT_AssignedAP = mechDef.AssignAPbyLocation(mechDef.RightTorso, mechDef.Chassis.RightTorso);
        LA_MaxAP = mechDef.CalcMaxAPbyLocation(mechDef.LeftArm, mechDef.Chassis.LeftArm);
        LA_AssignedAP = mechDef.AssignAPbyLocation(mechDef.LeftArm, mechDef.Chassis.LeftArm);
        RA_MaxAP = mechDef.CalcMaxAPbyLocation(mechDef.RightArm, mechDef.Chassis.RightArm);
        RA_AssignedAP = mechDef.AssignAPbyLocation(mechDef.RightArm, mechDef.Chassis.RightArm);
        LL_MaxAP = mechDef.CalcMaxAPbyLocation(mechDef.LeftLeg, mechDef.Chassis.LeftLeg);
        LL_AssignedAP = mechDef.AssignAPbyLocation(mechDef.LeftLeg, mechDef.Chassis.LeftLeg);
        RL_MaxAP = mechDef.CalcMaxAPbyLocation(mechDef.RightLeg, mechDef.Chassis.RightLeg);
        RL_AssignedAP = mechDef.AssignAPbyLocation(mechDef.RightLeg, mechDef.Chassis.RightLeg);
    }
}