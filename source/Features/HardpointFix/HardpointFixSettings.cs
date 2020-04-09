namespace MechEngineer.Features.HardpointFix
{
    internal class HardpointFixSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Optimizes the way installed weapons are shown on a mech model.";

        public bool TraceLogDebugMappings { get; set; } = false;
        public string TraceLogDebugMappingsDescription => "Trace all GetComponentBlankNames and GetComponentPrefabName calls. Otherwise only some of them are logged.";

        // TODO add set to 4 slots per chassis location autofix variant
        // TODO make enum so we have: set to 4, set to encountered prefabs, disabled
        public bool AutoFixChassisDefWeaponHardpointCounts = false;
        public string AutoFixChassisDefWeaponHardpointCountsDescription = "Changes chassis hardpoints based on configured prefabs.";

        public bool EnforceHardpointLimits = false;
        public string EnforceHardpointLimitsDescription = "Enforces hardpoint limits in mechlab only allowing configured prefabs.";

        public bool AllowDefaultLoadoutWeapons = false;
        public string AllowDefaultLoadoutWeaponsDescription = "Ignore limits that would prevent mounting default loadouts.";

        public string[] FallbackPrefabsForComponentDefIds = { "Weapon_Mortar_MechMortar" };
        public string FallbackPrefabsForComponentDefIdsDescription = "A list of components that always should be assigned a weapon prefab to.";

        // from: /data/weapon$ grep -R "PrefabIdentifier" . | cut -d\" -f 4 | sort | uniq
        // to: /data/hardpoints$ grep -R "chrPrfWeap" . | cut -d_ -f 5 | sort | uniq
        // default mapping = prefabid -> lower case prefab id (e.g. Flamer -> flamer, PPC -> ppc)
        public WeaponPrefabMapping[] WeaponPrefabMappings = {
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "AC2",
                HardpointCandidates = new[] {"ac2", "uac2", "rac2", "lbx2", "ac", "lbx"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "AC5",
                HardpointCandidates = new[] {"ac5", "uac5", "rac5", "lbx5", "ac", "lbx"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "AC10",
                HardpointCandidates = new[] {"ac10", "uac10", "lbx10", "ac", "lbx"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "AC20",
                HardpointCandidates = new[] {"ac20", "uac20", "ac", "lbx"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "lbx10", // used by LBX 2-20
                HardpointCandidates = new[] {"lbx10", "lbx20", "lbx5", "lbx2", "ac10", "ac", "lbx"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "uac5", // used by UAC 2-20
                HardpointCandidates = new[] {"uac5", "uac20", "uac10", "uac2", "ac5", "ac20", "ac10", "ac2", "rac5", "lbx5", "ac", "lbx"}
            },
            new WeaponPrefabMapping
            { /* requested by LtShade */
                PrefabIdentifier= "artillery",
                HardpointCandidates = new[] {"artillery", "ac20", "uac20", "lbx20", "ac", "lbx"}
            },
            new WeaponPrefabMapping
            { /* requested by bloodydoves */
                PrefabIdentifier= "mortar",
                HardpointCandidates = new[] {"mortar", "ac20", "uac20", "lbx20", "ac", "lbx"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "lrm5",
                HardpointCandidates = new[] {"lrm5", "lrm10", "lrm15", "lrm20", "srm20"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "lrm10",
                HardpointCandidates = new[] {"lrm10", "lrm15", "lrm20", "srm20", "lrm5", "rl10"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "lrm15",
                HardpointCandidates = new[] {"lrm15", "lrm20", "srm20", "lrm10", "lrm5", "rl20", "rl15"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "lrm20",
                HardpointCandidates = new[] {"lrm20", "srm20", "lrm15", "lrm10", "lrm5", "rl20", "rl15", "rl10"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "MachineGun",
                HardpointCandidates = new[] {"machinegun", "mg", "lmg", "hmg"}
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
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "PPC",
                HardpointCandidates = new[] {"PPC", "ppc", "hppc", "lppc", "snppc"}
            },
            new WeaponPrefabMapping
            {
                PrefabIdentifier= "Gauss",
                HardpointCandidates = new[] {"gauss", "hgauss", "lgauss"}
            },
        };

        public class WeaponPrefabMapping
        {
            public string PrefabIdentifier;
            public string[] HardpointCandidates;
        }
    }
}