namespace MechEngineer.Features.ArmorMaximizer.Maximizer;

internal class ChassisLocationState
{
    internal int Max { get; }
    internal int Assigned { get; set; }

    internal ChassisLocationState(int max, int assigned)
    {
        Max = max;
        Assigned = assigned;
    }

    internal int Remaining => Max - Assigned;
    internal bool IsFull => Assigned >= Max;

    public override string ToString()
    {
        return $"[Max={Max} Assigned={Assigned}]";
    }
}
