using System;
using Extensions.FSM;
using Movement.Player.States;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States.Carry
{
    public class Carry_Substates : ISubStateMachine
    {
        private readonly StateMachine _stateMachine;
        private readonly IState _startState;
        private readonly PlayerReferenceManager _playerReferences;
        
        public Carry_Substates(PlayerReferenceManager playerReferences)
        {
            _playerReferences = playerReferences;
            
            _stateMachine = new StateMachine();
            
            //Make it easier to add transitions
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
            void Any(IState to, Func<bool> condition) => _stateMachine.AddAnyTransition (to, condition);

            var carryIdle = new IdleState(_playerReferences);
            var carryWalk = new WalkState(_playerReferences);
            var carryRun = new RunState(_playerReferences);
            var fallState = new FallState(_playerReferences);
            
            At(carryIdle, carryWalk, () => IsGroundedAndMoving() && !IsSprinting());
            At(carryIdle, carryRun, () => IsGroundedAndMoving() && IsSprinting());
            At(carryIdle, fallState, () => !IsGrounded()  && IsFalling());
            
            At(carryWalk, carryIdle, IsGroundedAndNotMoving);
            At(carryWalk, carryRun, () => IsGroundedAndMoving() && IsSprinting());
            At(carryWalk, fallState, () => !IsGrounded()  && IsFalling());
            
            At(carryRun, carryIdle, IsGroundedAndNotMoving);
            At(carryRun, carryWalk, () => IsGroundedAndMoving() && !IsSprinting());
            At(carryRun, fallState, () => !IsGrounded() && IsFalling());
            
            At(fallState, carryIdle, IsGroundedAndNotMoving);
            At(fallState, carryWalk, () => IsGroundedAndMoving() && !IsSprinting());
            At(fallState, carryRun, () => IsGroundedAndMoving() && IsSprinting());
            
            _startState = carryIdle;
            return;

            bool IsSprinting() => _playerReferences.Input.sprint;
            bool IsFalling() => _playerReferences.VerticalVelocity < 0;
            bool IsGrounded() => _playerReferences.GravityHandler.IsGrounded;
            bool IsGroundedAndMoving() => _playerReferences.GravityHandler.IsGrounded && _playerReferences.Input.move != Vector2.zero;
            bool IsGroundedAndNotMoving() => _playerReferences.GravityHandler.IsGrounded && _playerReferences.Input.move == Vector2.zero;
        }
        public void OnEnter()
        {
            _stateMachine.SetState(_startState);
        }

        public void Tick()
        {
            _stateMachine.Tick();
        }

        public void OnExit()
        {
            
        }

        public Color GizmoState()
        {
            return _stateMachine.GetCurrentState().GizmoState();
        }

        public string GetCurrentStateName()
        {
            return  "Carry_SubStates_" + _stateMachine.GetCurrentStateName();
        }
    }
}
