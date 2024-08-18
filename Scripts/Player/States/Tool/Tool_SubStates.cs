using System;
using Extensions.FSM;
using Interactable.Carriable.Tool;
using Movement.Player.States;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States.Tool
{
    public class Tool_SubStates : ISubStateMachine
    {
        private readonly StateMachine _stateMachine;
        private readonly IState _startState;
    
        public Tool_SubStates(PlayerReferenceManager playerReferences)
        {
            _stateMachine = new StateMachine();

            //Make it easier to add transitions
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
            void Any(IState to, Func<bool> condition) => _stateMachine.AddAnyTransition (to, condition);
        
            var animUseTool = new Tool_AnimUse(playerReferences);
            //THROW HOOK IS NOT THE ANIMATION, BUT RATHER ONLY THE PHYSICAL IMPULSE TRIGGERED BY THE ANIMATION
             var physicalUseTool = new Tool_PhysicalUse(playerReferences);
            //GET HOOK IS NOT THE ANIMATION, BUT THE ACTUAL MOVEMENT TRIGGERED BY THE ANIMATION EVENT IN THE GETHOOKANIM
            var animStopUseTool = new Tool_AnimStopUse(playerReferences);
            var physicalStopUseTool = new Tool_PhysicalStopUse(playerReferences);
            
            //IF MINIGAME
            var toolPopupMiniGame = new Tool_MiniGamePopup(playerReferences);
            var toolMiniGame = new Tool_MiniGame(playerReferences);
            var toolWonMiniGame = new Tool_AnimWonMinigame(playerReferences);
            
            var exitState = new EmptyExitState(new Color(0.25f, 0.0f, 0.25f));

        
            At(animUseTool, physicalUseTool, () => playerReferences.ToolUseAnimEvent);
            //Player caught nothing because he pulls the rod out himself
            At(physicalUseTool, animStopUseTool, () => playerReferences.CurrentFishingHook == null && playerReferences.Input.action 
                && playerReferences.IsActionCompleted && playerReferences.EquippedItem is ITool tool && tool.GetStopActionHash() != 0);
        
            At(physicalUseTool, toolPopupMiniGame, () => playerReferences.CurrentFishingHook != null && !playerReferences.Input.action 
                && playerReferences.IsActionCompleted && playerReferences.EquippedItem is ITool tool && tool.GetWonMinigameActionHash() != 0);
            
            At(physicalUseTool, exitState, () => playerReferences.IsActionCompleted && playerReferences.EquippedItem is ITool tool && tool.GetStopActionHash() == 0);
            
            At(animStopUseTool, physicalStopUseTool, () => playerReferences.ToolStopUseAnimEvent);
        
            // Additional checks, if the player has aborted the Minigame in the second the popup state is entered
            At(toolPopupMiniGame, toolMiniGame, () => playerReferences.Input.action 
                                                      && playerReferences.CurrentFishingHook != null 
                                                      && playerReferences.CurrentFishingHook.MoveHookCoroutine != null);
            
            At(toolPopupMiniGame, animStopUseTool, () => playerReferences.CurrentFishingHook == null 
                                                      || playerReferences.CurrentFishingHook.MoveHookCoroutine == null);
            //
            At(toolMiniGame, toolWonMiniGame, () => playerReferences.WonMiniGame);
            At(toolMiniGame, animStopUseTool, () => playerReferences.LostMiniGame);
        
            At(toolWonMiniGame, physicalStopUseTool, () => playerReferences.ToolStopUseAnimEvent);
            At(physicalStopUseTool, exitState, () => playerReferences.IsActionCompleted);
        
            _startState = animUseTool;
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
        public bool IsInExitState()
        {
            return _stateMachine.GetCurrentState() is EmptyExitState;
        }
        public Color GizmoState()
        {
            return _stateMachine.GetCurrentState().GizmoState();
        }
        public string GetCurrentStateName()
        {
            return  "Tool_SubStates_" + _stateMachine.GetCurrentStateName();
        }
    }
}
