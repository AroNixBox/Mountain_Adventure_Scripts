using Extensions.FSM;
using Interactable.Carriable;
using Interactable.NPC.States;
using Movement.Player.States;

namespace Interactable.NPC
{
    public interface INpc : IInteractable
    {
        public bool HasRewardedPlayer();
        public DialogueState GetDialogueState();
        public bool HasFinishedTalking();
        void Talk();
        public bool IsStateOfType<T>() where T : IState;
    }
}