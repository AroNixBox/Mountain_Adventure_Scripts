using Extensions.FSM;
using UnityEngine;

namespace Interactable.NPC.States
{
    public class RewardState : IState
    {
        private NPCReferenceManager _npcReferenceManager;
        private Animator _animator;
        private bool _shouldLookAtEvent;
    
        public RewardState(NPCReferenceManager npcReferenceManager)
        {
            _npcReferenceManager = npcReferenceManager;
            _animator = _npcReferenceManager.Animator;
        }
        public void OnEnter()
        {
            _animator.SetTrigger(NPCReferenceManager.ANIM_REWARDPLAYER);

            if (_npcReferenceManager.Quest is PointQuest)
            {
                _shouldLookAtEvent = true;
                //Remove Points from Player if this is a Pointquest
                UIEvents.PassNumber.Get("RemovePoints").Invoke(_npcReferenceManager.Quest.GetRewardPoints(true));
            }
        }

        public void Tick()
        {
        
        }

        //This states exit is triggered by the animation event in the NPCAnimationEventListener
        public void OnExit()
        {
            if (_npcReferenceManager.Quest is PhysicalCarryQuest) {
                UIEvents.PassNumber.Get("RewardPoints").Invoke(_npcReferenceManager.Quest.GetRewardPoints());   
            }
        }
    
        public bool ShallFireEvent()
        {
            return _shouldLookAtEvent;
        }

        public Color GizmoState()
        {
            return Color.red;
        }
    }
}
