using System;
using JetBrains.Annotations;

namespace MechEngineer.Misc;

// annotate stuff used by third-parties
// avoids compiler unused complaints
// shows inverse dependencies
[MeansImplicitUse(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.WithMembers)]
internal class UsedByAttribute : Attribute
{
    public UsedByAttribute([UsedImplicitly] User user)
    {
        // we don't really care as its only used during compile time
    }
}

internal enum User
{
    // don't modify or refactor
    ModTek,
    Harmony,

    // avoid complaints about unused stuff
    FastJson,

    // try not to modify or refactor
    FieldRepairs, // FrostRaptor
    BattleValue, // bhtrail
    Abilifier // ajkroeg(fboob) / Cmission(Kmission)
}
