using Movement.Player.States;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States
{
    public class JumpState : MovementState
    {
        //TODO: Add GroundedTimeout where the ground can not be checked!!!
        private float _performingJumpThresholdDelta;
        private float _currentSpeed;
        private float _speedLerpTime = 0.2f;
        
        public JumpState(PlayerReferenceManager playerReferenceManager) : base(playerReferenceManager)
        {

        }

        public override void OnEnter()
        {
            _currentSpeed = PlayerReferenceManager.GetCurrentMoveSpeed();
            
            //Starts the jump timeout
            _performingJumpThresholdDelta = PlayerReferenceManager.PerformingJumpThreshold;
            
            PlayerReferenceManager.VerticalVelocity = Mathf.Sqrt(PlayerReferenceManager.JumpHeight * -2f * PlayerReferenceManager.GravityHandler.Gravity);

            if (Animator != null)
            {
                Animator.SetBool(PlayerReferenceManager.ANIM_JUMP, true);
                Animator.SetBool(PlayerReferenceManager.ANIM_GROUNDED, PlayerReferenceManager.GravityHandler.IsGrounded);
            }
        }

        public override void Tick()
        {
            if (_performingJumpThresholdDelta >= 0.0f)
            {
                _performingJumpThresholdDelta -= Time.deltaTime;
            }
            
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
        public bool IsPerformingJump()
        {
            return _performingJumpThresholdDelta > 0.0f;
        }
        public override void OnExit()
        {
            //Reset the JumpTimeout on Exit, to allow new jump after minimum time threshold
            PlayerReferenceManager.GravityHandler.JumpTimeoutDelta = PlayerReferenceManager.JumpTimeOut;
            
            if (Animator != null)
            {
                Animator.SetBool(PlayerReferenceManager.ANIM_JUMP, false);
                Animator.SetBool(PlayerReferenceManager.ANIM_GROUNDED, PlayerReferenceManager.GravityHandler.IsGrounded);
            }
        }

        public override Color GizmoState()
        {
            return new Color(1f, 0.8f, 0f);
        }
    }
}