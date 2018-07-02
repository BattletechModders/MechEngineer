using CustomComponents;

namespace MechEngineer
{
    [Custom("EngineSideDef")]
    public class EngineSideDef : CustomHeatSinkDef<EngineSideDef>, IEnginePart
    {
        // only used to allow identification via IEnginePart during Crit calculations
    }
}
