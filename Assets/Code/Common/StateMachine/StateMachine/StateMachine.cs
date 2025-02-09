using System;
using System.Collections.Generic;

namespace Platformer
{
    public class StateMachine
    {
        StateNode current;
        Dictionary<Type, StateNode> nodes = new();
        HashSet<ITransition> anyTransition = new();

        public void Update() 
        {
            var transition = GetTransition();

            if(transition!= null)
                ChangeState(transition.To);

            current._state?.OnUpdate();
        }

        public void FixedUpdate() 
        {
            current._state?.OnFixedUpdate();
        }

        public void SetState(IState state)
        {
            current = nodes[state.GetType()];
            current._state?.OnEnter();
        }

        private void ChangeState(IState state)
        {
            if(state == current._state) return;

            var prevState = current._state;
            var nextState = nodes[state.GetType()]._state;

            prevState?.OnExit();
            nextState?.OnEnter();
            current = nodes[state.GetType()];
        }

        ITransition GetTransition()
        {
            foreach (var transition in anyTransition)
                if(transition.Condition.Evaluate()) return transition;

            foreach (var transition in current._transition)
                if(transition.Condition.Evaluate()) return transition;

            return null;
        }

        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to)._state, condition);
        }

        public void AddAnyTransition(IState to, IPredicate condition)
        {
            anyTransition.Add(new TransitionBase(GetOrAddNode(to)._state, condition));
        }

        StateNode GetOrAddNode(IState state)
        {
            var node = nodes.GetValueOrDefault(state.GetType());

            if(node == null)
            {
                node = new StateNode(state);
                nodes.Add(state.GetType(), node);
            }

            return node;

        }

        class StateNode{
            public IState _state;
            public HashSet<ITransition> _transition;

            public StateNode(IState state)
            {
                _state = state;
                _transition = new HashSet<ITransition>();
            }

            public void AddTransition(IState to, IPredicate condition)
            {
                _transition.Add(new TransitionBase(to, condition));
            }
        }
    }
}