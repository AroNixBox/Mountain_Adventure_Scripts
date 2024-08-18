using Extensions.FSM;
using UnityEngine;

namespace Interactable.NPC.States
{
    public class IdleState : IState
    {
        private NPCReferenceManager _npcReferenceManager;
        private SuperTextMesh _dialogueTMP;
        private bool _isInitial = true;
        public IdleState (NPCReferenceManager npcReferenceManager)
        {
            _npcReferenceManager = npcReferenceManager;
            _dialogueTMP = _npcReferenceManager.DialogueTMP;
        }
        public void OnEnter()
        {
            //Fallback, reset the input...
            _npcReferenceManager.Talk = false;
            
            if (!_isInitial)
            {
                _dialogueTMP.gameObject.SetActive(false);
                _npcReferenceManager.ResetDialogueCamera();
            }
            else
            {
                _isInitial = false;
            }
            //Trigger Idle Animation
        }

        public void Tick()
        {
            //Apply Gravity?
        }

        public void OnExit()
        {
            
        }

        public Color GizmoState()
        {
            return new Color(0.2f, 0.8f, 0.2f);
        }
    }
}
