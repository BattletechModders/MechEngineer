namespace MechEngineer.Features.HardpointFix;

internal class HardpointFixSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Optimizes the way installed weapons are shown on a mech model.";

    public string[] FallbackPrefabsForComponentDefIds = {"Weapon_Mortar_MechMortar"};
    public string FallbackPrefabsForComponentDefIdsDescription => "A list of components that always should be assigned a weapon prefab to.";

    // from: /data/weapon$ grep -R "PrefabIdentifier" . | cut -d\" -f 4 | sort | uniq
    // to: /data/hardpoints$ grep -R "chrPrfWeap" . | cut -d_ -f 5 | sort | uniq
    // default mapping = prefabid -> lower case prefab id (e.g. Flamer -> flamer, PPC -> ppc)
    public WeaponPrefabMapping[] WeaponPrefabMappings =
    {
        new()
        {
            PrefabIdentifier = "AC2",
            HardpointCandidates = new[] {"ac2", "uac2", "rac2", "lbx2", "ac", "lbx"}
        },
        new()
        {
            PrefabIdentifier = "AC5",
            HardpointCandidates = new[] {"ac5", "uac5", "rac5", "lbx5", "ac", "lbx"}
        },
        new()
        {
            PrefabIdentifier = "AC10",
            HardpointCandidates = new[] {"ac10", "uac10", "lbx10", "ac", "lbx"}
        },
        new()
        {
            PrefabIdentifier = "AC20",
            HardpointCandidates = new[] {"ac20", "uac20", "ac", "lbx"}
        },
        new()
        {
            PrefabIdentifier = "lbx10", // used by LBX 2-20
            HardpointCandidates = new[] {"lbx10", "lbx20", "lbx5", "lbx2", "ac10", "ac", "lbx"}
        },
        new()
        {
            PrefabIdentifier = "uac5", // used by UAC 2-20
            HardpointCandidates = new[] {"uac5", "uac20", "uac10", "uac2", "ac5", "ac20", "ac10", "ac2", "rac5", "lbx5", "ac", "lbx"}
        },
        new()
        { /* requested by LtShade */
            PrefabIdentifier = "artillery",
            HardpointCandidates = new[] {"artillery", "ac20", "uac20", "lbx20", "ac", "lbx"}
        },
        new()
        { /* requested by bloodydoves */
            PrefabIdentifier = "mortar",
            HardpointCandidates = new[] {"mortar", "ac20", "uac20", "lbx20", "ac", "lbx"}
        },
        new()
        {
            PrefabIdentifier = "lrm5",
            HardpointCandidates = new[] {"lrm5", "lrm10", "lrm15", "lrm20", "srm20"}
        },
        new()
        {
            PrefabIdentifier = "lrm10",
            HardpointCandidates = new[] {"lrm10", "lrm15", "lrm20", "srm20", "lrm5", "rl10"}
        },
        new()
        {
            PrefabIdentifier = "lrm15",
            HardpointCandidates = new[] {"lrm15", "lrm20", "srm20", "lrm10", "lrm5", "rl20", "rl15"}
        },
        new()
        {
            PrefabIdentifier = "lrm20",
            HardpointCandidates = new[] {"lrm20", "srm20", "lrm15", "lrm10", "lrm5", "rl20", "rl15", "rl10"}
        },
        new()
        {
            PrefabIdentifier = "MachineGun",
            HardpointCandidates = new[] {"machinegun", "mg", "lmg", "hmg"}
        },
        new()
        {
            PrefabIdentifier = "srm2",
            HardpointCandidates = new[] {"srm2", "srm4", "srm6"}
        },
        new()
        {
            PrefabIdentifier = "srm4",
            HardpointCandidates = new[] {"srm4", "srm6", "srm2"}
        },
        new()
        {
            PrefabIdentifier = "srm6",
            HardpointCandidates = new[] {"srm6", "srm4", "srm2"}
        },
        new()
        {
            PrefabIdentifier = "PPC",
            HardpointCandidates = new[] {"PPC", "ppc", "hppc", "lppc", "snppc"}
        },
        new()
        {
            PrefabIdentifier = "Gauss",
            HardpointCandidates = new[] {"gauss", "hgauss", "lgauss"}
        }
    };

    public class WeaponPrefabMapping
    {
        public string PrefabIdentifier = null!;
        public string[] HardpointCandidates = null!;
    }
}