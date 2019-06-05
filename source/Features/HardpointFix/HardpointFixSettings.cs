namespace MechEngineer.Features.HardpointFix
{
    internal class HardpointFixSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Modifies the way installed weapons are shown on a mech model.";

        // TODO add set to 4 slots per chassis location autofix variant
        // TODO make enum so we have: set to 4, set to encountered prefabs, disabled
        public bool AutoFixChassisDefWeaponHardpointCounts = false; // true = hardpoint counts derived from prefab hardpoints
        public bool EnforceHardpointLimits = false; // true = use prefab hardpoints
        public bool AllowDefaultLoadoutWeapons = false;

        // from: /data/weapon$ grep -R "PrefabIdentifier" . | cut -d\" -f 4 | sort | uniq
        // to: /data/hardpoints$ grep -R "chrPrfWeap" . | cut -d_ -f 5 | sort | uniq
        // default mapping = prefabid -> lower case prefab id (e.g. Flamer -> flamer, PPC -> ppc)
        public WeaponPrefabMapping[] WeaponPrefabMappings = {
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "AC2",
                HardpointCandidates = new[] {"ac2", "ac", "lbx"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "AC5",
                HardpointCandidates = new[] {"ac5", "uac5", "ac", "lbx"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "AC10",
                HardpointCandidates = new[] {"ac10", "lbx10", "ac", "lbx"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "AC20",
                HardpointCandidates = new[] {"ac20", "ac", "lbx"}
            },
            new WeaponPrefabMapping
            { /* requested by LtShade */
                PrefabIdentifier= "artillery",
                HardpointCandidates = new[] {"artillery", "ac20", "ac", "lbx"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "lrm5",
                HardpointCandidates = new[] {"lrm5", "lrm10", "lrm15", "lrm20", "srm20"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "lrm10",
                HardpointCandidates = new[] {"lrm10", "lrm15", "lrm20", "srm20", "lrm5"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "lrm15",
                HardpointCandidates = new[] {"lrm15", "lrm20", "srm20", "lrm10", "lrm5"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "lrm20",
                HardpointCandidates = new[] {"lrm20", "srm20", "lrm15", "lrm10", "lrm5"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "MachineGun",
                HardpointCandidates = new[] {"machinegun", "mg"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "srm2",
                HardpointCandidates = new[] {"srm2", "srm4", "srm6"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "srm4",
                HardpointCandidates = new[] {"srm4", "srm6", "srm2"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "srm6",
                HardpointCandidates = new[] {"srm6", "srm4", "srm2"}
            }
        };

        public class WeaponPrefabMapping
        {
            public string PrefabIdentifier;
            public string[] HardpointCandidates;
        }
    }
}