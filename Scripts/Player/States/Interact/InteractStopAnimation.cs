using System.Collections;
using Extensions.FSM;
using Interactable.Carriable.Tool;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States.Interact
{
    public class InteractStopAnimation : IState
    {
        private readonly PlayerReferenceManager _playerReferences;
        private Coroutine _rotateCoroutine;
        private readonly Animator _animator;
        private float _currentSpeed;
        
        private float rotationSpeed = 180f; // Degrees per second
        private static readonly int Drop = Animator.StringToHash("Drop");
        private static readonly int IKThrow = Animator.StringToHash("IKThrow");

        public InteractStopAnimation(PlayerReferenceManager playerReferences)
        {
            _playerReferences = playerReferences;
            _animator = playerReferences.Animator;
        }
        public void OnEnter()
        {
            //TODO: Save the Interactable from InteractAnimation in PlayerReferenceManager to use it here.
            //TODO: On Enter set a local variable to the Interactable from PlayerReferenceManager
            
            //TODO: Hide the Interact UI
            
            switch (_playerReferences.EquippedItem)
            {
                case ITool toolToPick:
                    // Set the target rotation
                    var targetPositionToLookAt = toolToPick.InitialPosition;
                    // Create a new target position that has the same height (y-value) as the player's current position
                    Vector3 targetPositionWithSameHeight = new Vector3(targetPositionToLookAt.x, _playerReferences.transform.position.y, targetPositionToLookAt.z);
                    // Calculate the target rotation
                    Quaternion targetRotation = Quaternion.LookRotation(targetPositionWithSameHeight - _playerReferences.transform.position, Vector3.up);


                    // Start the rotation coroutine
                    _rotateCoroutine = _playerReferences.StartCoroutine(RotateTowardsTarget(targetRotation));
                    break;
                
                case { } itemToPick:
                    _animator.SetTrigger(Drop);
                    break;
            }
            
            _currentSpeed = _playerReferences.GetCurrentMoveSpeed();
        }
        private IEnumerator RotateTowardsTarget(Quaternion targetRotation) {
            while (Quaternion.Angle(_playerReferences.transform.rotation, targetRotation) > 0.01f) {
                _playerReferences.transform.rotation = Quaternion.RotateTowards(_playerReferences.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                yield return null;
            }

            _animator.SetTrigger(IKThrow);

            if (_rotateCoroutine != null)
            {
                _playerReferences.StopCoroutine(_rotateCoroutine);
                _rotateCoroutine = null;
            }
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
        
        public void OnExit()
        {
            _playerReferences.CurrentInteractable = null;
            //TODO: Null this local Interactable
            _playerReferences.IsInteractionAnimationDone = false;
        }
        
        public Color GizmoState()
        {
            return new Color(1f, 0f, 0f, 0.5f);
        }
    }
}
