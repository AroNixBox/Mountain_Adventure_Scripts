using System;
using Extensions.FSM;
using Interactable.NPC.States;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States.Talk
{
    public class Talk_SubStates : ISubStateMachine
    {
        private StateMachine _stateMachine;
        private IState _startState;
        private PlayerReferenceManager _playerReferenceManager;
        public Talk_SubStates(PlayerReferenceManager playerReferenceManager)
        {
            _playerReferenceManager = playerReferenceManager;
            
            _stateMachine = new StateMachine();
            
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
            void Any(IState to, Func<bool> condition) => _stateMachine.AddAnyTransition (to, condition);
        
            var talkAnimState = new TalkAnimState(_playerReferenceManager, _playerReferenceManager.ANIM_TALK, _playerReferenceManager.TalkSounds);
            var animThankState = new TalkAnimState(_playerReferenceManager, _playerReferenceManager.ANIM_THANK, _playerReferenceManager.ThankfulSounds);
            var talkState = new TalkState(_playerReferenceManager);
            var rewardState = new RewardState(_playerReferenceManager, _playerReferenceManager.ANIM_REWARD, _playerReferenceManager.PlayerRewardSounds);
            var lookAtEventState = new LookAtEventState(_playerReferenceManager, _playerReferenceManager.LookAtEventTime);
            var exitState = new EmptyExitState(new Color(1f, 0.400f, 0f));
            
            At(talkAnimState, talkState, () => _playerReferenceManager.IsPlayerTalkAnimationDone);
            
            At(talkState, talkAnimState, () => InputAndHasFinishedSpeaking() &&
                                               _playerReferenceManager.CurrentNpc.GetDialogueState() == DialogueState.Talk &&
                                               _playerReferenceManager.CurrentNpc.IsStateOfType<TalkIdleState>());
            
            //NPC Didn't reward Player but dialogue ended
            At(talkState, animThankState, () => InputAndHasFinishedSpeaking()
                                                && _playerReferenceManager.CurrentNpc.GetDialogueState() is DialogueState.DialogueFinished or DialogueState.QuestFinished);
                                                  
            //NPC Finished Rewarding Player
            At(talkState, rewardState, () =>  _playerReferenceManager.CurrentNpc.HasFinishedTalking() 
                                              && _playerReferenceManager.CurrentNpc.GetDialogueState() is DialogueState.RewardPlayer
                                              //HasRewarded Player is Only to wait for the AnimEvent to finish
                                              && _playerReferenceManager.CurrentNpc.HasRewardedPlayer());
            
            At(talkState, lookAtEventState, () =>  _playerReferenceManager.CurrentNpc.HasFinishedTalking() 
                                                   && _playerReferenceManager.CurrentNpc.GetDialogueState() is DialogueState.LookAtEvent
                                                   //HasRewarded Player is Only to wait for the AnimEvent to finish
                                                   && _playerReferenceManager.CurrentNpc.HasRewardedPlayer());
            
            At(lookAtEventState, exitState, () => lookAtEventState.CanExit);
            At(rewardState, exitState, () => _playerReferenceManager.IsPlayerTalkAnimationDone);
            At(animThankState, exitState, () => _playerReferenceManager.IsPlayerTalkAnimationDone);
            
             _startState = talkAnimState;

            return;
            bool InputAndHasFinishedSpeaking() => _playerReferenceManager.Input.interact && _playerReferenceManager.CurrentNpc.HasFinishedTalking();
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
            return "Talk_SubStates_" + _stateMachine.GetCurrentStateName();
        }
    }
}
