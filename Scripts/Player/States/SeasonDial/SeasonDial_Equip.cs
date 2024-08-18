using Extensions.FSM;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States.SeasonDial
{
    public class SeasonDial_Equip : IState
    {
        private readonly PlayerReferenceManager _playerReferences;
        private readonly Animator _animator;
        public SeasonDial_Equip(PlayerReferenceManager playerReferences)
        {
            _playerReferences = playerReferences;
            _animator = _playerReferences.Animator;
        }
        public void OnEnter()
        {
            _animator.SetBool(_playerReferences.ANIM_EQUIPCOMPASS, true);
        }

        public void Tick()
        {
            //Apply Gravity
            _playerReferences.GravityHandler.HandleGravity();
            _playerReferences.Controller.Move(new Vector3(0, _playerReferences.VerticalVelocity, 0) * Time.deltaTime);
        }

        public void OnExit()
        {
            
        }

        public Color GizmoState()
        {
            return new Color(0.678f, 0.847f, 0.902f);
        }
    }
}
