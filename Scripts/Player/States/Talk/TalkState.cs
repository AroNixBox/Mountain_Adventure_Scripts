using Extensions.FSM;
using Interactable.NPC;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States.Talk
{
    public class TalkState : IState
    {
        private readonly PlayerReferenceManager _playerReferences;
        private INpc _npc;
        private readonly CharacterController _controller;
        private readonly GravityHandler _gravityHandler;
        
        public TalkState(PlayerReferenceManager playerReferences)
        {
            _playerReferences = playerReferences;
            _controller = playerReferences.Controller;
            _gravityHandler = playerReferences.GravityHandler;
        }
        public void OnEnter()
        {
            _npc = _playerReferences.CurrentNpc;
            
            _npc.Talk();
        }
        public void Tick()
        {
            _gravityHandler.HandleGravity();
            _controller.Move(new Vector3(0, _playerReferences.VerticalVelocity, 0) * Time.deltaTime);
        }
        public void OnExit()
        {

        }
        public Color GizmoState()
        {
            return new Color(1f, 0.549f, 0f);
        }
    }
}
