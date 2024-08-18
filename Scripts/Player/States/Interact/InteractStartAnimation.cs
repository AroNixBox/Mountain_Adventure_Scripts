using Extensions.FSM;
using Interactable.Carriable;
using Interactable.Carriable.Tool;
using Mechanics.Driving;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States.Interact
{
    public class InteractStartAnimation : IState
    {
        private readonly PlayerReferenceManager _playerReferences;
        private readonly Animator _animator;
        private readonly CharacterController _controller;
        private readonly GravityHandler _gravityHandler;
        
        private float _currentSpeed;
        private readonly float _speedLerpTime = 0.2f;

        //TODO: Put this in PlayerReferenceManager and AnimatioEventHandler and let it set by an Animation event when the Animation is done.
        private bool _isInteractAnimationComplete;
        private static readonly int PickUp = Animator.StringToHash("PickUp");
        private static readonly int IKGrab = Animator.StringToHash("IKGrab");

        public InteractStartAnimation(PlayerReferenceManager playerReferences)
        {
            _playerReferences = playerReferences;
            _animator = _playerReferences.Animator;
            _controller = _playerReferences.Controller;
            _gravityHandler = _playerReferences.GravityHandler;
        }
        public void OnEnter()
        {
            //Disable the Interaction Sensor UI
            UIEvents.PassTextToUI.Get("Interact").Invoke(string.Empty);
            
            _playerReferences.CurrentInteractable = _playerReferences.PossibleInteractable;
            
            switch (_playerReferences.PossibleInteractable)
            {
                case ITool toolToPick:
                    _playerReferences.RightHandIKTarget.transform.position = toolToPick.GetPosition();
                    _animator.SetTrigger(IKGrab);
                    break;
                
                case ICarriable itemToPick:
                    // [_playerReferences.IsInteractionAnimationDone] Is set by Animation Event
                    _animator.SetTrigger(PickUp);
                    //TODO: Half the Movement Speed
                    
                    break;
                
                case Car carToEnter:
                    _playerReferences.RightHandIKTarget.transform.position = carToEnter.leftDoorTransform.position;
                    _animator.SetTrigger(IKGrab);
                    break;
                
                case { } normalInteractable:
                    _playerReferences.RightHandIKTarget.transform.position = normalInteractable.GetPosition();
                    _animator.SetTrigger(IKGrab);
                    break;
            }
            
            _currentSpeed = _playerReferences.GetCurrentMoveSpeed();
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
            _playerReferences.IsInteractionAnimationDone = false;
        }

        public Color GizmoState()
        {
            return new Color(0.86f, 0.08f, 0.24f);
        }
    }
}
