using System;
using Extensions.FSM;
using Movement.Player.States;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States.SeasonDial
{
    public class SeasonDial_Substates : ISubStateMachine
    {
        private readonly StateMachine _stateMachine;
        private readonly PlayerReferenceManager _playerReferences;
        public bool IsUsingSeasonDial { get; private set; }
        private readonly IState _startState;
        private Animator _animator;
        
        private float _currentSpeed;
        public SeasonDial_Substates(PlayerReferenceManager playerReferences)
        {
            _playerReferences = playerReferences;
            _animator = _playerReferences.Animator;
            
            _stateMachine = new StateMachine();

            //Make it easier to add transitions
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
            void Any(IState to, Func<bool> condition) => _stateMachine.AddAnyTransition (to, condition);
            
            var equipState = new SeasonDial_Equip(_playerReferences);
            var toggleState = new SeasonDial_Toggle(_playerReferences);
            var unequipState = new SeasonDial_Unequip(_playerReferences);
            var seasonChangeState = new SeasonDial_SeasonChange(_playerReferences);
            var exitState = new EmptyExitState(new Color(0f, 0f, 0.282f));
            
            At(equipState, toggleState, () => _playerReferences.PulledOutDial);
            
            At(toggleState, unequipState, () => _playerReferences.Input.equipDial);
            At(unequipState, seasonChangeState, () => _playerReferences.PutAwayDial && toggleState.IsDifferentSeasonLockedIn());
            
            At(unequipState, exitState, () => _playerReferences.PutAwayDial && !toggleState.IsDifferentSeasonLockedIn());
            At(seasonChangeState, exitState, () => _playerReferences.IsRitualFinished);
            
            //Exit Condition: PutAwayDial
            _startState = equipState;
        }
        public void OnEnter()
        {
            // Lerp the speed towards 0
            _currentSpeed = _playerReferences.GetCurrentMoveSpeed();
            _stateMachine.SetState(_startState);
            IsUsingSeasonDial = true;
        }

        public void Tick()
        {
            // Lerp the speed towards 0
            _currentSpeed = Mathf.Lerp(_currentSpeed, 0f, Time.deltaTime / 0.2f);

            if (_animator != null)
            {
                _animator.SetFloat(_playerReferences.ANIM_SPEED, _currentSpeed);
                _animator.SetBool(_playerReferences.ANIM_GROUNDED, _playerReferences.GravityHandler.IsGrounded);
            }
            
            _stateMachine.Tick();
        }

        public void OnExit()
        {
            //Reset Flags
            //PutAway
            _playerReferences.PutAwayDial = false;
            IsUsingSeasonDial = false;
        }
        public bool IsInExitState()
        {
            return _stateMachine.GetCurrentState() is EmptyExitState;
        }
        public Color GizmoState()
        {
            return _stateMachine.GetCurrentState().GizmoState();
        }

        public string GetCurrentStateName()
        {
            return _stateMachine.GetCurrentState().ToString();
        }
    }
}
