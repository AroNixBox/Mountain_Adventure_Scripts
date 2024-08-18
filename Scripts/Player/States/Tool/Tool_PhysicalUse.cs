using Extensions.FSM;
using Interactable.Carriable.Tool;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States.Tool
{
    public class Tool_PhysicalUse : IState
    {
        private readonly PlayerReferenceManager _playerReferences;
        private readonly Animator _animator;
    
        private readonly CharacterController _controller;
        private readonly GravityHandler _gravityHandler;

        private float _currentSpeed;
        private readonly float _speedLerpTime = 0.2f;
        public Tool_PhysicalUse(PlayerReferenceManager playerReferenceManager)
        {
            _playerReferences = playerReferenceManager;
            _animator = _playerReferences.Animator;
            _controller = playerReferenceManager.Controller;
            _gravityHandler = playerReferenceManager.GravityHandler;
        }
        public void OnEnter()
        {
            _currentSpeed = _playerReferences.GetCurrentMoveSpeed();

            if (_playerReferences.EquippedItem is ITool fishingRod)
            {
                fishingRod.PerformAction();
            }
            else
            {
                Debug.LogError("Player does not have a Fishing Rod equipped! Check Conditions, TYPE MISMATCH!");
            }
        }
        public void Tick()
        {
            _gravityHandler.HandleGravity();
            _controller.Move(new Vector3(0, _playerReferences.VerticalVelocity, 0) * Time.deltaTime);

            // Lerp the speed towards 0
            _currentSpeed = Mathf.Lerp(_currentSpeed, 0f, Time.deltaTime / _speedLerpTime);

            if (_animator != null)
            {
                _animator.SetFloat(_playerReferences.ANIM_SPEED, _currentSpeed);
                _animator.SetBool(_playerReferences.ANIM_GROUNDED, _gravityHandler.IsGrounded);
            }
        }
        public void OnExit()
        {
            //Reset the flags
            _playerReferences.IsActionCompleted = false;
            _playerReferences.ToolUseAnimEvent = false;
        }
        public Color GizmoState()
        {
            return new Color(1.0f, 0.4f, 0.7f);
        }
    }
}
