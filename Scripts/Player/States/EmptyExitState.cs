using Extensions.FSM;
using UnityEngine;

namespace Player.States
{
    public class EmptyExitState : IState
    {
        private readonly Color _exitStateColor;
        public EmptyExitState(Color exitStateColor)
        {
            _exitStateColor = exitStateColor;
        }
        public void OnEnter()
        {
        }

        public void Tick()
        {
        
        }

        public void OnExit()
        {

        }

        public Color GizmoState()
        {
            return _exitStateColor;
        }
    }
}
