using Movement.Player.States;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States
{
    public class FallState : MovementState
    {
        
        private float _currentSpeed;
        private float _speedLerpTime = 0.2f;
        public FallState(PlayerReferenceManager playerReferenceManager) : base(playerReferenceManager)
        {

        }

        public override void OnEnter()
        {
            if (Animator != null)
            {
                Animator.SetBool(PlayerReferenceManager.ANIM_GROUNDED, PlayerReferenceManager.GravityHandler.IsGrounded);
                Animator.SetBool(PlayerReferenceManager.ANIM_FREEFALL, true);
            }
        }

        public override void Tick()
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, PlayerReferenceManager.GetCurrentMoveSpeed(), Time.deltaTime / _speedLerpTime);

            if (Animator != null)
            {
                Animator.SetFloat(PlayerReferenceManager.ANIM_SPEED, _currentSpeed);
            }
            
            Vector3 movement = CalculateMovement();
            PlayerReferenceManager.GravityHandler.HandleGravity();
            movement.y = PlayerReferenceManager.VerticalVelocity;
            Controller.Move(movement * Time.deltaTime);
        }
        
        public override void OnExit()
        {      
            if (Animator != null)
            {
                Animator.SetBool(PlayerReferenceManager.ANIM_FREEFALL, false);
                Animator.SetBool(PlayerReferenceManager.ANIM_GROUNDED, PlayerReferenceManager.GravityHandler.IsGrounded);
            }
        }

        public override Color GizmoState()
        {
            return new Color(0.8f, 0.7f, 0f);
        }
    }
}