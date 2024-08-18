using Extensions.FSM;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States.Interact
{
    public class InteractStop : IState
    {
        private readonly PlayerReferenceManager _playerReferences;
        private bool _isUnequipComplete;
        private float _currentSpeed;
        private Animator _animator;
        public InteractStop(PlayerReferenceManager playerReferenceManager) {
            _playerReferences = playerReferenceManager;
            _animator = playerReferenceManager.Animator;
        }
        public void OnEnter()
        {
            //Wait for the item to tell the player to drop then wait for the player to play the drop animation, then action is invoked
            _playerReferences.StartCoroutine(_playerReferences.EquippedItem.Drop(() =>
            {
                _isUnequipComplete = true;
                _playerReferences.EquippedItem = null;
            }));
            
            _currentSpeed = _playerReferences.GetCurrentMoveSpeed();
        }

        public void Tick()
        {
            _playerReferences.GravityHandler.HandleGravity();
            _playerReferences.Controller.Move(new Vector3(0, _playerReferences.VerticalVelocity, 0) * Time.deltaTime);
            
            // Lerp the speed towards 0
            _currentSpeed = Mathf.Lerp(_currentSpeed, 0f, Time.deltaTime / 0.2f);

            if (_animator != null)
            {
                _animator.SetFloat(_playerReferences.ANIM_SPEED, _currentSpeed);
                _animator.SetBool(_playerReferences.ANIM_GROUNDED, _playerReferences.GravityHandler.IsGrounded);
            }
        }
        public bool IsUnequipComplete() {
            return _isUnequipComplete;
        }
        public void OnExit() {
            _isUnequipComplete = false;
        }
        
        public Color GizmoState() {
            return new Color(1f, 0.2f, 0.2f);
        }
    }
}
