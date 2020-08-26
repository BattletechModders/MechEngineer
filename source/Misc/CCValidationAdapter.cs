using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using Harmony;
using Localize;

namespace MechEngineer
{
    internal class CCValidationAdapter
    {
        private readonly IValidateMech validator;
        public CCValidationAdapter(IValidateMech validator)
        {
            this.validator = validator;
        }

        public void ValidateMech(Dictionary<MechValidationType, List<Text>> errorMessages, MechValidationLevel validationLevel, MechDef mechDef)
        {
            var errors = new Errors();
            validator.ValidateMech(mechDef, errors);
            errors.Populate(errorMessages);
        }

        public bool ValidateMechCanBeFielded(MechDef mechDef)
        {
            var errors = new Errors {FailOnFirstError = true};
            validator.ValidateMech(mechDef, errors);
            return !errors.Any();
        }

        public string ValidateDrop(MechLabItemSlotElement drop_item, MechDef mechDef1, List<InvItem> iteminventory2, List<IChange> changes)
        {
            var errors1 = new Errors();
            validator.ValidateMech(mechDef1, errors1);

            var mechDef2 = new MechDef(mechDef1);
            var inventory2 = iteminventory2.Select(x =>
            {
                var r = new MechComponentRef(x.item);
                Traverse.Create(r).Property(nameof(MechComponentRef.MountedLocation)).SetValue(x.location);
                return r;
            }).ToList();

            mechDef2.SetInventory(inventory2.ToArray());

            var errors2 = new Errors();
            validator.ValidateMech(mechDef2, errors2);

            var newErrors = errors2.Except(errors1);
            return newErrors.FirstOrDefault()?.Message ?? string.Empty;
        }
    }

    public class Errors: IEnumerable<Error>
    {
        internal readonly OrderedSet<Error> Messages = new OrderedSet<Error>();
        internal bool FailOnFirstError = false;

        internal bool Add(MechValidationType type, string message)
        {
            Control.Logger.Debug?.Log($"Add type={type} message={message}");
            Messages.Add(new Error(type, message));
            return FailOnFirstError;
        }

        internal void Populate(Dictionary<MechValidationType, List<Text>> errorMessages)
        {
            foreach (var error in this)
            {
                errorMessages[error.Type].Add(new Text(error.Message));
            }
        }

        public IEnumerator<Error> GetEnumerator()
        {
            return Messages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Error : IEquatable<Error>
    {
        internal Error(MechValidationType type, string message)
        {
            Type = type;
            Message = message;
        }

        internal MechValidationType Type { get; }
        internal string Message { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Error);
        }

        public bool Equals(Error other)
        {
            return other != null &&
                   Type == other.Type &&
                   Message == other.Message;
        }

        public override int GetHashCode()
        {
            var hashCode = 674393081;
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Message);
            return hashCode;
        }

        public override string ToString()
        {
            return $"{Type}: {Message}";
        }
    }
}
