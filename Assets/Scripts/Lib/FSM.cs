using System.Collections;
using UnityEngine;

namespace Lib
{
    public abstract class State<T> where T : MonoBehaviour
    {
        public T owner;
 
        protected State(T owner)
        {
            this.owner = owner;
        }

        public virtual void Update() {}
        public virtual void Enter() {}
        public virtual void Exit() {}

        public virtual IEnumerator OnExit()
        {
            yield return null;
        }
    }
    
    public class StateMachine<T> where T : MonoBehaviour
    {
        State<T> _state;
        public State<T> Current => _state;

        public StateMachine(State<T> initialState)
        {
            _state = initialState;
        }

        public void Start()
        {
            _state?.Enter();
        }

        public void ChangeState(State<T> newState)
        {
            _state?.owner.StartCoroutine(WaitForFinish(newState));
        }

        IEnumerator WaitForFinish(State<T> newState)
        {
            yield return _state.owner.StartCoroutine(_state.OnExit());
            _state?.Exit();
            _state = newState;
            _state.Enter();
        }

        public void Update()
        {
            _state?.Update();
        }
    }
}