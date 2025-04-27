using System;

namespace IngameScript
{
    public abstract class BaseTestClass
    {
        public abstract Action[] Tests { get; }

        public virtual void Setup() { }
    }
}
