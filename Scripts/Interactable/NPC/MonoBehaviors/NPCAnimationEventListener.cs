using UnityEngine;

namespace Interactable.NPC
{
    public class NPCAnimationEventListener : MonoBehaviour
    {
        private NPCReferenceManager _npcReferenceManager;

        private void Awake()
        {
            _npcReferenceManager = GetComponent<NPCReferenceManager>();
        }

        public void OnRewardPlayerAnimationFinished()
        {
            _npcReferenceManager.HasRewardedPlayer = true;
            //TODO: Transition to next State, has said last Line
        }
    }
}
