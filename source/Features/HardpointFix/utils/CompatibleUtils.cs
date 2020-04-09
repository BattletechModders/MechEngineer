using System;
using System.Collections.Generic;
using System.Linq;

namespace MechEngineer.Features.HardpointFix.utils
{
    internal class CompatibleUtils
    {
        private static readonly Dictionary<string, string[]> cachedCompatibleTerms = new Dictionary<string, string[]>();

        internal static string[] GetCompatiblePrefabTerms(string prefabIdentifier)
        {
            if (!cachedCompatibleTerms.TryGetValue(prefabIdentifier, out var compatibleTerms))
            {
                compatibleTerms = Control.settings.HardpointFix.WeaponPrefabMappings
                    .Where(x => string.Equals(x.PrefabIdentifier, prefabIdentifier, StringComparison.CurrentCultureIgnoreCase))
                    .Select(x => x.HardpointCandidates)
                    .SingleOrDefault();

                if (compatibleTerms == null)
                {
                    compatibleTerms = new[] { prefabIdentifier };
                }

                cachedCompatibleTerms[prefabIdentifier] = compatibleTerms;
            }

            return compatibleTerms;
        }
    }
}
