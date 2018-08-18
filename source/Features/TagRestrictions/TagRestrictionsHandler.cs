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
            var tagsUINames = new Dictionary<string, string>();
            string NameForTag(string tag)
            {
                if (!tagsUINames.TryGetValue(tag, out var UIName))
                {
                    UIName = tag;
                }

                return UIName;
            }

            var tags = new HashSet<string>();
            {
                var chassis = mechDef.Chassis;
                if (chassis.ChassisTags != null)
                {
                    tags.UnionWith(chassis.ChassisTags);
                }
                if (Control.settings.TagRestrictionsUseDescriptionIds)
                {
                    tags.Add(chassis.Description.Id);
                    tagsUINames.Add(chassis.Description.Id, chassis.Description.UIName);
                }
            }
            foreach (var def in mechDef.Inventory.Select(r => r.Def))
            {
                if (def.ComponentTags != null)
                {
                    tags.UnionWith(def.ComponentTags);
                }
                if (Control.settings.TagRestrictionsUseDescriptionIds)
                {
                    tags.Add(def.Description.Id);
                    tagsUINames.Add(def.Description.Id, def.Description.UIName);
                }
            }

            foreach (var tag in tags)
            {
                foreach (var incompatibleTag in IncompatibleTags(tag))
                {
                    if (!tags.Contains(incompatibleTag))
                    {
                        continue;
                    }

                    var tagName = NameForTag(tag);
                    var incompatibleTagName = NameForTag(incompatibleTag);
                    if (errors.Add(MechValidationType.InvalidInventorySlots, $"Can't use {tagName} with {incompatibleTagName}"))
                    {
                        return;
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

            if (restriction.IncompatibleTags == null)
            {
                yield break;
            }

            foreach (var incompatibleTag in restriction.IncompatibleTags)
            {
                yield return incompatibleTag;
            }

            if (restriction.MoreTagRestrictionsFrom == null)
            {
                yield break;
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