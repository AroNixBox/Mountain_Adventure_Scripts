using Extensions.FSM;
using UnityEngine;

namespace Interactable.NPC.States
{
    public class TalkIdleState : IState
    {
        private NPCReferenceManager _npcReferenceManager;
        private SuperTextMesh _dialogueTMP;
        public TalkIdleState(NPCReferenceManager npcReferenceManager)
        {
            _npcReferenceManager = npcReferenceManager;
            _dialogueTMP = _npcReferenceManager.DialogueTMP;
        }
        public void OnEnter()
        {
            //TODO Trigger Talk Idle Animation
        }

        public void Tick()
        {
            
        }

        public void OnExit()
        {
            
        }

        public Color GizmoState()
        {
            return new Color(0.8f, 0.7f, 0f);
        }
    }
}
