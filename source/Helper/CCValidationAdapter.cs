using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using Localize;
using MechEngineer.Misc;

namespace MechEngineer.Helper;

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

    public string ValidateDrop(MechLabItemSlotElement drop_item, List<InvItem> new_inventory)
    {
        var mechDef = MechLabHelper.CurrentMechLab.ActiveMech;

        var errors1 = new Errors();
        validator.ValidateMech(mechDef, errors1);

        var mechDef2 = new MechDef(mechDef);
        var inventory2 = new_inventory.Select(x =>
        {
            var r = new MechComponentRef(x.Item);
            r.MountedLocation = x.Location;
            return r;
        }).ToList();

        mechDef2.SetInventory(inventory2.ToArray());

        var errors2 = new Errors();
        validator.ValidateMech(mechDef2, errors2);

        var newErrors = errors2.Except(errors1);
        return newErrors.FirstOrDefault()?.Message ?? string.Empty;
    }
}

public class Errors : IEnumerable<Error>
{
    internal readonly OrderedSet<Error> Messages = new();
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