using System.Collections.Generic;

namespace GFrame.StateMachine
{
    public interface IState
    {
        void Create(IStateContext context);
        void OnEnter();

        void OnExit();
    }

    public interface IStateContext
    {
        void ChangeState(int key);
    }

    public class StateMachine : IStateContext
    {
        private readonly Dictionary<int, IState> _stateDic = new Dictionary<int, IState>();
        private IState _currentState;

        public void RegisterState(int type, IState state)
        {
            if (_stateDic.TryGetValue(type, out _))
                return;
            _stateDic.Add(type, state);
            state.Create(this);
        }

        private IState GetState(int type)
        {
            return _stateDic.TryGetValue(type, out var state) ? state : null;
        }

        public void ChangeState(int key)
        {
            if (_currentState != null)
            {
                GameLog.Log($"[StateMachine] Exit {_currentState.GetType()}");
                _currentState.OnExit();
            }

            var state = GetState(key);
            if (state != null)
            {
                GameLog.Log($"[StateMachine] Enter {state.GetType()}");
                state?.OnEnter();
            }

            _currentState = state;
        }
    }

    public abstract class BaseState : IState
    {
        private IStateContext _context;

        void IState.Create(IStateContext context)
        {
            _context = context;
            OnCreate();
        }

        protected void ChangeState(int state)
        {
            _context?.ChangeState(state);
        }

        public abstract void OnEnter();

        public abstract void OnExit();

        protected virtual void OnCreate()
        {
        }
    }
}