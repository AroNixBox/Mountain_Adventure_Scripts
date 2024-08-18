using Extensions.FSM;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States.Tool {
    public class Tool_MiniGamePopup : IState {
        private readonly PlayerReferenceManager _playerReferences;
        
        private readonly CharacterController _controller;
        private readonly GravityHandler _gravityHandler;
        private Animator _animator;
        
        private float _currentSpeed;
        private readonly float _speedLerpTime = 0.2f;
        public Tool_MiniGamePopup(PlayerReferenceManager playerReferenceManager) {
            _playerReferences = playerReferenceManager;
            _controller = playerReferenceManager.Controller;
            _gravityHandler = playerReferenceManager.GravityHandler;
            _animator = playerReferenceManager.Animator;
        }
        public void OnEnter() {
            // Open Popup
            _playerReferences.FishingInfoPopup.SetActive(true);
        }

        public void Tick() {
            //Gravity
            _gravityHandler.HandleGravity();
            _controller.Move(new Vector3(0, _playerReferences.VerticalVelocity, 0) * Time.deltaTime);

            // Lerp the speed towards 0
            _currentSpeed = Mathf.Lerp(_currentSpeed, 0f, Time.deltaTime / _speedLerpTime);

            if (_animator != null) {
                _animator.SetFloat(_playerReferences.ANIM_SPEED, _currentSpeed);
                _animator.SetBool(_playerReferences.ANIM_GROUNDED, _gravityHandler.IsGrounded);
            }
        }

        public void OnExit() {
            // Close Popup
            _playerReferences.FishingInfoPopup.SetActive(false);
            
            // In the case we aborted the Minigame, we dont go into the Minigame State, so we need to manually reset the Hook
            if(_playerReferences.CurrentFishingHook != null && _playerReferences.CurrentFishingHook.MoveHookCoroutine == null) {
                _playerReferences.CurrentFishingHook = null;
            }
        }

        public Color GizmoState() {
            return new Color(0.80f, 0.43f, 0.80f);
        }
    }
}