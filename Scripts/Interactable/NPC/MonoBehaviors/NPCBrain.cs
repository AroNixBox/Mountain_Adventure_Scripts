using System;
using System.Collections;
using Extensions.FSM;
using Interactable.Carriable;
using Interactable.NPC.States;
using Movement.Player.States;
using UnityEngine;

namespace Interactable.NPC
{
    public class NPCBrain : MonoBehaviour, INpc
    {
        [SerializeField] private SuperTextMesh debugText;
        private StateMachine _stateMachine;
        private NPCReferenceManager _npcReferenceManager;
        
        private void Awake()
        {
            _npcReferenceManager = GetComponent<NPCReferenceManager>();
        }

        private void Start()
        {
            _stateMachine = new StateMachine();
            
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
            void Any(IState to, Func<bool> condition) => _stateMachine.AddAnyTransition (to, condition);
            
            var idleState = new IdleState(_npcReferenceManager);
            var turnToPlayerState = new TurnState(_npcReferenceManager, GameReferenceCarrier.Instance.PlayerHead);
            var talkState = new TalkState(_npcReferenceManager);
            var talkIdleState = new TalkIdleState(_npcReferenceManager);
            var rewardState = new RewardState(_npcReferenceManager);
            var turnToEventState = new TurnState(_npcReferenceManager, _npcReferenceManager.GetFirstEventListener(), true);
            var eventState = new LookAtEventState(_npcReferenceManager, NPCReferenceManager.ANIM_POINT, _npcReferenceManager.DialogueSO.NpcEventSound,_npcReferenceManager.LookAtEventWaitTime);
            
            At(idleState, turnToPlayerState, () => _npcReferenceManager.Interact);
            At(turnToPlayerState, talkState, () => _npcReferenceManager.Talk && turnToPlayerState.HasFinishedTurning());
            At(talkState, talkIdleState, () => _npcReferenceManager.HasFinishedPrintingTextToScreen);
            At(talkIdleState, talkState, () => _npcReferenceManager.Talk && _npcReferenceManager.CurrentDialogueState == DialogueState.Talk);
            At(talkIdleState, rewardState, () => _npcReferenceManager.CurrentDialogueState is DialogueState.RewardPlayer or DialogueState.LookAtEvent);
            //TODO: Add Talk as Condition
            At(talkIdleState, idleState, () => _npcReferenceManager.Talk && _npcReferenceManager.CurrentDialogueState is DialogueState.DialogueFinished or DialogueState.QuestFinished);
            At(rewardState, idleState, () => _npcReferenceManager.HasRewardedPlayer && !rewardState.ShallFireEvent());
            At(rewardState, turnToEventState, () => _npcReferenceManager.HasRewardedPlayer && rewardState.ShallFireEvent());
            At(turnToEventState, eventState, () => turnToEventState.HasFinishedTurning());
            At(eventState, idleState, () => eventState.CanExit());
            
            _stateMachine.SetState(idleState);
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
        public void Talk()
        {
            _npcReferenceManager.Talk = true;
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

        public bool IsStateOfType<T>() where T : IState
        {
            return _stateMachine.GetCurrentState() is T;
        }
        public IEnumerator Interact(Transform position, Action onInteractFinished)
        {
            var wait = new WaitForSeconds(0.5f);
            
            _npcReferenceManager.Interact = true;
            
            while (_npcReferenceManager.Interact)
            {
                yield return wait;
            }
            
            onInteractFinished.Invoke();
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }
        public bool HasFinishedTalking()
        {
            return _npcReferenceManager.HasFinishedPrintingTextToScreen;
        }
        public DialogueState GetDialogueState()
        {
            return _npcReferenceManager.CurrentDialogueState;
        }
        public bool HasRewardedPlayer()
        {
            return _npcReferenceManager.HasRewardedPlayer;
        }
    }
}
