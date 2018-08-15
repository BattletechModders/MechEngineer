using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace MechEngineer
{
    public class TagRestrictionsHandler : IValidateMech
    {
        internal static TagRestrictionsHandler Shared = new TagRestrictionsHandler();
        internal CCValidationAdapter CCValidation;

        public void Add(TagRestrictions restrictions)
        {
            Restrictions.Add(restrictions.Tag, restrictions);
        }

        private Dictionary<string, TagRestrictions> Restrictions { get; } = new Dictionary<string, TagRestrictions>();

        private TagRestrictionsHandler()
        {
            CCValidation = new CCValidationAdapter(this);
        }

        public void ValidateMech(MechDef mechDef, Errors errors)
        {
            var tags = new HashSet<string>();
            tags.UnionWith(mechDef.Chassis.ChassisTags);
            foreach (var componentTags in mechDef.Inventory.Select(r => r.Def.ComponentTags))
            {
                tags.UnionWith(componentTags);
            }

            foreach (var tag in tags)
            {
                foreach (var incompatibleTag in IncompatibleTags(tag))
                {
                    if (tags.Contains(incompatibleTag))
                    {
                        if (errors.Add(MechValidationType.InvalidInventorySlots, $"Can't use {tag} with {incompatibleTag}"))
                        {
                            return;
                        }
                    }
                }
            }
        }

        private IEnumerable<string> IncompatibleTags(string tag)
        {
            if (!Restrictions.TryGetValue(tag, out var restriction))
            {
                yield break;
            }

            foreach (var incompatibleTag in restriction.IncompatibleTags)
            {
                yield return incompatibleTag;
            }

            foreach (var subTag in restriction.MoreTagRestrictionsFrom)
            {
                foreach (var incompatibleTag in IncompatibleTags(subTag))
                {
                    yield return incompatibleTag;
                }
            }
        }
    }
}