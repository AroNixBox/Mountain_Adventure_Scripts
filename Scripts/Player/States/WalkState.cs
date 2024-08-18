using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States
{
    public class WalkState : MovementState
    {
        private float _currentSpeed;
        private float _speedLerpTime = 0.2f;

        public WalkState(PlayerReferenceManager playerReferenceManager) : base(playerReferenceManager)
        {

        }

        public override void OnEnter()
        {
            _currentSpeed = PlayerReferenceManager.GetCurrentMoveSpeed();
            Animator.SetBool(PlayerReferenceManager.ANIM_GROUNDED, PlayerReferenceManager.GravityHandler.IsGrounded);
        }

        public override void Tick()
        {
            Move();
        }

        public override void OnExit()
        {
            
        }

        public override Color GizmoState()
        {
            return new Color(0.6f, 0.15f, 0.53f);
        }

        private void Move()
        {
            Vector3 movement = CalculateMovement();
            PlayerReferenceManager.GravityHandler.HandleGravity();

            // Lerp the speed towards the current move speed
            _currentSpeed = Mathf.Lerp(_currentSpeed, PlayerReferenceManager.GetCurrentMoveSpeed(), Time.deltaTime / _speedLerpTime);

            PlayerReferenceManager.Controller.Move(movement * Time.deltaTime + new Vector3(0, PlayerReferenceManager.VerticalVelocity, 0) * Time.deltaTime);

            if (Animator != null)
            {
                Animator.SetFloat(PlayerReferenceManager.ANIM_SPEED, _currentSpeed);
            }
        }
    }
}