using Extensions.FSM;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States
{
    public class IdleState : IState
    {
        private readonly PlayerReferenceManager _playerReferenceManager;
        private readonly Animator _animator;
        private readonly CharacterController _controller;
        private readonly GravityHandler _gravityHandler;

        private float _currentSpeed;
        private readonly float _speedLerpTime = 0.2f;

        public IdleState(PlayerReferenceManager playerReferenceManager)
        {
            _playerReferenceManager = playerReferenceManager;
            _animator = playerReferenceManager.Animator;
            _controller = playerReferenceManager.Controller;
            _gravityHandler = playerReferenceManager.GravityHandler;
        }

        public void OnEnter()
        {
            _currentSpeed = _playerReferenceManager.GetCurrentMoveSpeed();
        }

        public void Tick()
        {
            _gravityHandler.HandleGravity();
            _controller.Move(new Vector3(0, _playerReferenceManager.VerticalVelocity, 0) * Time.deltaTime);

            // Lerp the speed towards 0
            _currentSpeed = Mathf.Lerp(_currentSpeed, 0f, Time.deltaTime / _speedLerpTime);

            if (_animator != null)
            {
                _animator.SetFloat(_playerReferenceManager.ANIM_SPEED, _currentSpeed);
                _animator.SetBool(_playerReferenceManager.ANIM_GROUNDED, _gravityHandler.IsGrounded);
            }
        }

        public void OnExit()
        {
            
        }

        public Color GizmoState()
        {
            return new Color(0.2f, 0.8f, 0.2f);
        }
    }
}