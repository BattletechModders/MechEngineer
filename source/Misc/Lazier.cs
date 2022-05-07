using System;

namespace MechEngineer.Misc;

public class Lazier<T> where T :  notnull, new()
{
    private readonly Func<T> initializer = () => new T();
    private T value = default!;

    private bool IsValueCreated { get; set; }

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