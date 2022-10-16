using BattleTech;
using MechEngineer.Helper;

namespace MechEngineer.Features.ArmorMaximizer;

public class ArmorMaximizerSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Max Armor button works within CBT ratios.";

    public bool StripBeforeMax { get; set; } = true;
    public string StripBeforeMaxDescription => "Avoid dirty locations when going after Max";

	public ArmorLocation[] ArmorLocationsLockedByDefault { get; set; } = {};
	public string ArmorLocationsLockedByDefaultDescription => $"A list of armor locations locked by default. Possible values: {string.Join(",", LocationExtensions.ArmorLocationList)}";

	public ValueByModifier StepSize = new ValueByModifier
	{
        Alt = 1,
		Control = 999,
        Shift = 25,
	};

	public ValueByModifier StepPrecision = new ValueByModifier
	{
		Alt = 0.5f,
	};

	public class ValueByModifier
	{
		public float? Alt { get; set; }
		public float? Control { get; set; }
		public float? Shift { get; set; }
		public float? Default { get; set; }

		internal float? Get()
		{
			if (Alt != null && InputUtils.AltModifierPressed)
			{
				return Alt;
			}
			if (Control != null && InputUtils.ControlModifierPressed)
			{
				return Control;
			}
			if (Shift != null && InputUtils.ShiftModifierPressed)
			{
				return Shift;
			}
			return Default;
		}
	}
}