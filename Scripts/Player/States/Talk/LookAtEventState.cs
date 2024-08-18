using System;
using Extensions.FSM;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States.Talk
{
    public class LookAtEventState : IState
    {
        private readonly PlayerReferenceManager _playerReferences;
        private readonly CharacterController _controller;
        private readonly GravityHandler _gravityHandler;
        private DateTime _time;
        private float _lookAtEventTime;

        public bool CanExit{ get; private set; }

        public LookAtEventState(PlayerReferenceManager playerReferences, float lookAtEventTime)
        {
            _playerReferences = playerReferences;
            _controller = playerReferences.Controller;
            _gravityHandler = playerReferences.GravityHandler;
            _lookAtEventTime = lookAtEventTime;
        }
        public void OnEnter()
        {
            _time = DateTime.Now.AddSeconds(_lookAtEventTime);
            CanExit = false;
        }
        public void Tick()
        {
            if (DateTime.Now >= _time)
            {
                CanExit = true;
            }
            
            _gravityHandler.HandleGravity();
            _controller.Move(new Vector3(0, _playerReferences.VerticalVelocity, 0) * Time.deltaTime);
        }

        public void OnExit()
        {
            _playerReferences.CurrentNpc = null;
        }

        public Color GizmoState()
        {
            return new Color(1f, 0.647f, 0f);
        }
    }
}