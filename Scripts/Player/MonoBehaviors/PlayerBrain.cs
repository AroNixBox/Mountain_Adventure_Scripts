using System;
using Extensions.FSM;
using Interactable.Carriable.Tool;
using Player.States;
using Player.States.Carry;
using Player.States.Driving;
using Player.States.Interact;
using Player.States.SeasonDial;
using Player.States.Talk;
using Player.States.Tool;
using UnityEngine;

namespace Player.MonoBehaviors
{
    public class PlayerBrain : MonoBehaviour
    {
        private StateMachine _stateMachine;
        private PlayerReferenceManager _playerReferenceManager;
    
        [SerializeField] private SuperTextMesh debugText;
        private void Awake()
        {
            _playerReferenceManager = GetComponent<PlayerReferenceManager>();
        }

        private void Start()
        {
            _stateMachine = new StateMachine();

            //Make it easier to add transitions
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
            void Any(IState to, Func<bool> condition) => _stateMachine.AddAnyTransition (to, condition);
            
            var idleState = new IdleState(_playerReferenceManager);
            var walkState = new WalkState(_playerReferenceManager);
            var runState = new RunState(_playerReferenceManager);
            var jumpState = new JumpState(_playerReferenceManager);
            var fallState = new FallState(_playerReferenceManager);
            
            var dialSubStates = new SeasonDial_Substates(_playerReferenceManager);
            var toolSubStates = new Tool_SubStates(_playerReferenceManager);
            
            var interactStartAnim = new InteractStartAnimation(_playerReferenceManager);
            var interactState = new InteractStart(_playerReferenceManager);
            
            var carrySubStates = new Carry_Substates(_playerReferenceManager);
            var talkSubStates = new Talk_SubStates(_playerReferenceManager);
            
            var drivingState = new DrivingState(_playerReferenceManager);
            var unequipState = new InteractStop(_playerReferenceManager);
            var interactStopAnimState = new InteractStopAnimation(_playerReferenceManager);

            At(idleState, walkState, () => IsGroundedAndMoving() && !IsSprinting());
            At(idleState, runState, () => IsGroundedAndMoving() && IsSprinting());
            At(idleState, jumpState, () => IsGrounded() && _playerReferenceManager.GravityHandler.CanJump() && _playerReferenceManager.Input.jump);
            At(idleState, fallState, () => !IsGrounded()  && IsFalling() && !_playerReferenceManager.GravityHandler.FallTimeOut());
            At(idleState, dialSubStates, () => IsGrounded() && _playerReferenceManager.Input.equipDial && _playerReferenceManager.EquippedItem == null);
            At(idleState, interactStartAnim, () => _playerReferenceManager.EquippedItem == null && !SetPossibleInteractableIsNull() && _playerReferenceManager.Input.interact);
            At(idleState, interactStopAnimState, () => InputInteract() && _playerReferenceManager.EquippedItem.CanPerformDrop());
            //TODO: Can add additional flags to not go into that state!
            At(idleState, toolSubStates, CanUseTool);
            
            At(walkState, idleState, IsGroundedAndNotMoving);
            At(walkState, runState, () => IsGroundedAndMoving() && IsSprinting());
            At(walkState, jumpState, () => IsGrounded() && _playerReferenceManager.GravityHandler.CanJump() &&  _playerReferenceManager.Input.jump);
            At(walkState, fallState, () => !IsGrounded()  && IsFalling() && !_playerReferenceManager.GravityHandler.FallTimeOut());
            At(walkState, dialSubStates, () => IsGrounded() && _playerReferenceManager.Input.equipDial && _playerReferenceManager.EquippedItem == null);
            At(walkState, interactStartAnim, () => _playerReferenceManager.EquippedItem == null && !SetPossibleInteractableIsNull() && _playerReferenceManager.Input.interact);
            At(walkState, interactStopAnimState, () => InputInteract() && _playerReferenceManager.EquippedItem.CanPerformDrop());
            //TODO: Can add additional flags to not go into that state!
            At(walkState, toolSubStates, CanUseTool);
            
            At(runState, idleState, IsGroundedAndNotMoving);
            At(runState, walkState, () => IsGroundedAndMoving() && !IsSprinting());
            At(runState, jumpState, () => IsGrounded() && _playerReferenceManager.GravityHandler.CanJump() && _playerReferenceManager.Input.jump);
            At(runState, fallState, () => !IsGrounded()  && IsFalling() && !_playerReferenceManager.GravityHandler.FallTimeOut());
            At(runState, dialSubStates, () => IsGrounded() && _playerReferenceManager.Input.equipDial && _playerReferenceManager.EquippedItem == null);
            At(runState, interactStartAnim, () => _playerReferenceManager.EquippedItem == null && !SetPossibleInteractableIsNull() && _playerReferenceManager.Input.interact);
            At(runState, interactStopAnimState, () => InputInteract() && _playerReferenceManager.EquippedItem.CanPerformDrop());
            //TODO: Can add additional flags to not go into that state!
            At(runState, toolSubStates, CanUseTool);
            
            At(fallState, idleState, IsGroundedAndNotMoving);
            At(fallState, walkState, () => IsGroundedAndMoving() && !IsSprinting());
            At(fallState, runState, () => IsGroundedAndMoving() && IsSprinting());
        
            At(jumpState, idleState, () => IsGroundedAndNotMoving() && !jumpState.IsPerformingJump());
            At(jumpState, walkState, () => IsGroundedAndMoving() && !IsSprinting() && !jumpState.IsPerformingJump());
            At(jumpState, runState, () => IsGroundedAndMoving() && IsSprinting() && !jumpState.IsPerformingJump());
            At(jumpState, fallState, () => !IsGrounded()  && IsFalling() && !jumpState.IsPerformingJump() && !_playerReferenceManager.GravityHandler.FallTimeOut());
            
            //Dont load this one with conditions, player will be idling anyways
            At(interactStartAnim, interactState, () => _playerReferenceManager.IsInteractionAnimationDone && _playerReferenceManager.CurrentInteractable != null);
            
            //TODO: This is a piece of shit
            //This goes back normal state when equipping an ITOOL
            At(interactState, idleState, () => interactState.IsInteractComplete() && _playerReferenceManager.EquippedItem != null && !IsCarriableButNotTool());
            At(interactState, carrySubStates, () => interactState.IsInteractComplete() && _playerReferenceManager.EquippedItem != null && IsCarriableButNotTool());
            At(interactState, talkSubStates, () => interactState.IsInteractComplete() && _playerReferenceManager.EquippedItem == null && _playerReferenceManager.CurrentNpc != null);
            At(interactState, drivingState, () => interactState.IsInteractComplete() && _playerReferenceManager.EquippedItem == null && _playerReferenceManager.CarToEnter != null);
            
            At(carrySubStates, interactStopAnimState, () => InputInteract() && IsGrounded() && _playerReferenceManager.EquippedItem.CanPerformDrop());
            
            At(interactStopAnimState, unequipState, () => _playerReferenceManager.IsInteractionAnimationDone 
                                                          && _playerReferenceManager.EquippedItem != null);
            At(unequipState, idleState, () => unequipState.IsUnequipComplete());
            
            // TODO:
            At(drivingState, idleState, () => _playerReferenceManager.Input.interact && _playerReferenceManager.CarToEnter.CanGetOutOfCar());
        
            //Dont load this one with conditions, otherwise player cant exit the dial SubStateMachine
            At(dialSubStates, idleState, () => dialSubStates.IsInExitState());
            At(toolSubStates, idleState, () => toolSubStates.IsInExitState());
            //TODO ADD STATE AND GIVE PLAYER REWARD!
            At(talkSubStates, idleState, () => talkSubStates.IsInExitState());
            
            _stateMachine.SetState(idleState);
            
            return;
            
            bool SetPossibleInteractableIsNull() => (_playerReferenceManager.PossibleInteractable = _playerReferenceManager.InteractionSensor.GetClosestInteractable()) == null;
            bool CanUseTool() => _playerReferenceManager.Input.action && _playerReferenceManager.EquippedItem is ITool;
            bool IsSprinting() => _playerReferenceManager.Input.sprint;
            bool IsFalling() => _playerReferenceManager.VerticalVelocity < 0;
            bool IsGrounded() => _playerReferenceManager.GravityHandler.IsGrounded;
            bool IsGroundedAndMoving() => _playerReferenceManager.GravityHandler.IsGrounded && _playerReferenceManager.Input.move != Vector2.zero;
            bool IsGroundedAndNotMoving() => _playerReferenceManager.GravityHandler.IsGrounded && _playerReferenceManager.Input.move == Vector2.zero;
            bool IsCarriableButNotTool() => _playerReferenceManager.EquippedItem is not ITool;
            bool InputInteract() => _playerReferenceManager.Input.interact && _playerReferenceManager.EquippedItem != null;
        }
        private void Update()
        {
            _stateMachine.Tick();

            if (debugText != null)
            {
                //Is _stateMachine.GetCurrentState() a SubStateMachine? If so, get the current state name from that SubStateMachine, otherwise get the current state name from the StateMachine
                debugText.text = "<c=rainbow><w=seasick>" + _stateMachine.GetCurrentStateName();
            }
        }
        private void OnDrawGizmos()
        {
            if (_stateMachine != null)
            {
                Gizmos.color = _stateMachine.GetGizmoColor();
                Vector3 gizmoPosition = transform.position + Vector3.up * 2f;
                Gizmos.DrawSphere(gizmoPosition, 0.1f);
            }
        }
    }
}