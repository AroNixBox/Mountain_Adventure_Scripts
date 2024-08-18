using Extensions.FSM;
using InputSystem;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States.Tool
{
    public class Tool_MiniGame : IState
    {
        private readonly PlayerReferenceManager _playerReferences;
        private readonly StarterAssetsInputs  _input;
        private readonly float _initialMouseInputSensitivityMultiplier;
        private float _currentMouseInputSpeedMultiplier;
        private readonly Rigidbody _minigameCircleRigidbody;
        private readonly float _controllerSpeedMultiplier;
        private readonly Animator _animator;
        private bool _isControllerConnected; // Cache the result of the controller check
    
        private readonly CharacterController _controller;
        private readonly GravityHandler _gravityHandler;

        private float _currentSpeed;
        private readonly float _speedLerpTime = 0.2f;
        public Tool_MiniGame(PlayerReferenceManager playerReferenceManager) {
            _playerReferences = playerReferenceManager;
            _input = _playerReferences.Input;
            _initialMouseInputSensitivityMultiplier = _playerReferences.FishingMouseInputSensitivityMultiplier;
            _minigameCircleRigidbody = _playerReferences.MinigameCircleRigidbody;
            _animator = _playerReferences.Animator;
            _controller = playerReferenceManager.Controller;
            _gravityHandler = playerReferenceManager.GravityHandler;
            _controllerSpeedMultiplier = playerReferenceManager.FishingControllerInputMultiplier;
        }
        public void OnEnter() {
            _currentSpeed = _playerReferences.GetCurrentMoveSpeed();
            
            _isControllerConnected = _playerReferences.IsControllerConnected();
            
            // Every time we enter a Minigame we reevaluate the Mouse Input Sensitivity, based on Controller Connected due to discrepancy in Input Speeds
            _currentMouseInputSpeedMultiplier = _isControllerConnected ? _initialMouseInputSensitivityMultiplier / _controllerSpeedMultiplier : _initialMouseInputSensitivityMultiplier;
            
            // Start the Minigame Coroutine
            _playerReferences.StartCoroutine(_playerReferences.CurrentFishingHook.MoveHookCoroutine);
        }

        public void Tick() {
            Vector2 lookInput = _input.look;
            Vector3 movement = new Vector3(-lookInput.x, 0, lookInput.y) * (_currentMouseInputSpeedMultiplier * Time.deltaTime);
            _minigameCircleRigidbody.MovePosition(_minigameCircleRigidbody.position + movement);

            //Gravity
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

        public void OnExit() {
            //Reset MinigameFlag
            _currentMouseInputSpeedMultiplier = _initialMouseInputSensitivityMultiplier;
            
            // Null the Hook, because we don't want to keep the reference to the hook
            _playerReferences.CurrentFishingHook = null;
        }

        public Color GizmoState() {
            return new Color(0.87f, 0.63f, 0.87f);
        }
    }
}
