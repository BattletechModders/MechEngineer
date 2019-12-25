using BattleTech;
using MechEngineer.Features.MoveMultiplierStat;

namespace MechEngineer.Features.Engines.Helper
{
    internal class MechDefMovementStatistics
    {
        internal float WalkMovementPoint { get; }
        internal float WalkSpeed { get; }
        internal float RunSpeed { get; }
        internal float JumpDistance { get; }

        internal MechDefMovementStatistics(MechDef mechDef)
        {
            this.mechDef = mechDef;
            
            movement = mechDef.GetEngine()?.CoreDef.GetMovement(mechDef.Chassis.Tonnage);
            if (movement == null)
            {
                return;
            }
            
            WalkMovementPoint = movement.WalkMovementPoint;
            MoveMultiplier = GetMoveMultiplier();
            BaseWalkSpeed = GetWalkSpeed();
            BaseRunSpeed = GetRunSpeed();
            WalkSpeed = BaseWalkSpeed * MoveMultiplier;
            RunSpeed = BaseRunSpeed * MoveMultiplier;

            JumpCapacity = GetJumpCapacity();
            BaseJumpDistance = EngineMovement.ConvertMPToGameDistance(JumpCapacity);
            JumpDistanceMultiplier = GetJumpDistanceMultiplier();
            JumpDistance = BaseJumpDistance * JumpDistanceMultiplier;
        }
        
        private float MoveMultiplier { get; }
        private float BaseWalkSpeed { get; }
        private float BaseRunSpeed { get; }
        private float JumpDistanceMultiplier { get; }
        private float JumpCapacity { get; }
        private float BaseJumpDistance { get; }

        private readonly MechDef mechDef;
        private readonly EngineMovement movement;
        private readonly StatCollection statCollection = new StatCollection();

        internal float GetStatisticRating()
        {
            // only sprint, no walk and no jump
            var constants = UnityGameInstance.BattleTechGame.MechStatisticsConstants;
            var relativeSpeed = (RunSpeed - constants.MinSprintFactor) / constants.MaxSprintFactor;
            return relativeSpeed;
        }

        private float GetMoveMultiplier()
        {
            var stat = statCollection.MoveMultiplier();
            stat.Create();
            return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
        }
        
        private float GetWalkSpeed()
        {
            var stat = statCollection.WalkSpeed();
            stat.CreateWithDefault(movement.WalkSpeed);
            return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
        }

        private float GetRunSpeed()
        {
            var stat = statCollection.RunSpeed();
            stat.CreateWithDefault(movement.RunSpeed);
            return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
        }

        private float GetJumpDistanceMultiplier()
        {
            var stat = statCollection.JumpDistanceMultiplier();
            stat.Create();
            return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
        }

        private float GetJumpCapacity()
        {
            var stat = statCollection.JumpCapacity();
            stat.Create();
            return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
        }
    }
}