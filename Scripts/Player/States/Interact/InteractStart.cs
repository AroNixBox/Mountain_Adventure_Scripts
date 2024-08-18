using Extensions.FSM;
using Interactable.Carriable;
using Interactable.Carriable.Tool;
using Interactable.NPC;
using Mechanics.Driving;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States.Interact
{
    public class InteractStart : IState
    {
        private readonly PlayerReferenceManager _playerReferences;
        private readonly Animator _animator;
        private bool _isInteractComplete;
        private float _currentSpeed;
        
        public InteractStart(PlayerReferenceManager playerReferences)
        {
            _playerReferences = playerReferences;
            _animator = playerReferences.Animator;
        }
        public void OnEnter()
        {
            //TODO: Save the Interactable from InteractAnimation in PlayerReferenceManager to use it here.
            //TODO: On Enter set a local variable to the Interactable from PlayerReferenceManager
            
            //TODO: Hide the Interact UI
            
            switch (_playerReferences.CurrentInteractable)
            {
                case ITool toolToPick:
                    _playerReferences.StartCoroutine(toolToPick.Interact(_playerReferences.CarriableEquipPosition, () => {
                        _playerReferences.EquippedItem = toolToPick;
                        _isInteractComplete = true;
                    }));
                    break;
                
                case ICarriable itemToPick:
                    _playerReferences.StartCoroutine(itemToPick.Interact(_playerReferences.CarriableEquipPosition, () => {
                        _playerReferences.EquippedItem = itemToPick;
                        _isInteractComplete = true;
                    }));
                    break;
                
                case INpc npcToTalk:
                    _playerReferences.StartCoroutine(npcToTalk.Interact(_playerReferences.PlayerHead, () => {
                        _playerReferences.CurrentNpc = npcToTalk;   
                        _isInteractComplete = true;
                    }));
                    break;
                
                case Car carToEnter:
                    _playerReferences.StartCoroutine(carToEnter.Interact(_playerReferences.CarriableEquipPosition, () => {
                        _playerReferences.CarToEnter = carToEnter;
                        _isInteractComplete = true;
                    }));
                    break;
                
                case { } normalInteractable:
                    _playerReferences.StartCoroutine(normalInteractable.Interact(_playerReferences.PlayerHead, () => {
                        _isInteractComplete = true;
                    }));
                    break;
            }
            
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
        
        public void OnExit()
        {
            //TODO: UnHide the Interact UI

            _playerReferences.CurrentInteractable = null;
            _isInteractComplete = false;
        }

        public bool IsInteractComplete()
        {
            return _isInteractComplete;
        }
        public Color GizmoState()
        {
            return new Color(1f, 0f, 0f, 0.5f);
        }
    }
}
