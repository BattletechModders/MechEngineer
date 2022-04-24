using System;

namespace MechEngineer.Misc;

public class MELazy<T> where T : new()
{
    private readonly Func<T> initializer;
    private T value;

    public MELazy(Func<T> initializer)
    {
        this.initializer = initializer ?? throw new ArgumentNullException(nameof(initializer));
    }

    public MELazy() : this(() => new T())
    {
    }

    public bool IsValueCreated { get; private set; }

    public T Value
    {
        get
        {
            if (!IsValueCreated)
            {
                value = initializer();
                IsValueCreated = true;
            }
            return value;
        }
    }
}