using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States
{
    public class RunState : MovementState
    {
        private float _currentSpeed;
        private float _speedLerpTime = 0.2f;

        public RunState(PlayerReferenceManager playerReferenceManager) : base(playerReferenceManager)
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
            return new Color(0f, 0.4f, 0f);
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