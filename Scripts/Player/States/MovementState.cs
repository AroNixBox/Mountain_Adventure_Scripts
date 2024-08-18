using Extensions.FSM;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States
{
    public abstract class MovementState : IState
    {
        protected readonly PlayerReferenceManager PlayerReferenceManager;
        protected readonly Animator Animator;
        protected readonly CharacterController Controller;

        private float _speed;
        private float _targetRotation;
        private float _rotationVelocity;

        protected MovementState(PlayerReferenceManager playerReferenceManager)
        {
            PlayerReferenceManager = playerReferenceManager;
            Animator = playerReferenceManager.Animator;
            Controller = playerReferenceManager.Controller;
        }

        public abstract void OnEnter();
        public abstract void Tick();
        public abstract void OnExit();
        public abstract Color GizmoState();

        protected Vector3 CalculateMovement()
        {
            float targetSpeed = PlayerReferenceManager.Input.sprint ? PlayerReferenceManager.SprintSpeed : PlayerReferenceManager.MoveSpeed;
            float inputMagnitude = PlayerReferenceManager.Input.analogMovement ? PlayerReferenceManager.Input.move.magnitude : 1f;

            if (PlayerReferenceManager.Input.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
            }

            Vector3 velocity = Controller.velocity;
            
            float currentHorizontalSpeed = new Vector3(velocity.x, 0.0f, velocity.z).magnitude;

            float speedOffset = 0.1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * PlayerReferenceManager.SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            Vector3 inputDirection = new Vector3(PlayerReferenceManager.Input.move.x, 0.0f, PlayerReferenceManager.Input.move.y).normalized;

            if (PlayerReferenceManager.Input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + PlayerReferenceManager.MainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(Controller.transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, PlayerReferenceManager.RotationSmoothTime);
                Controller.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            return targetDirection.normalized * _speed;
        }
    }
}