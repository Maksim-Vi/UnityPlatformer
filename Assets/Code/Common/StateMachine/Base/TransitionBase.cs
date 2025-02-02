using System;

namespace Platformer
{
    public class TransitionBase : ITransition
    {
        public IState To {get;}

        public IPredicate Condition {get;}

        public TransitionBase(IState to, IPredicate condition)
        {
            To = to;
            Condition = condition;
        }
    }
}