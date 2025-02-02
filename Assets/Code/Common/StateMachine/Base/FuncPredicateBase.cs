using System;

namespace Platformer
{
    public class FuncPredicateBase : IPredicate
    {
        readonly Func<bool> _func;

        public FuncPredicateBase(Func<bool> func)
        {
            _func = func;
        }

        public bool Evaluate()
        {
            return _func.Invoke();
        }
    }
}