using Extensions.FSM;
using InputSystem;
using Mechanics.Driving;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States.Driving {
    // The DrivingState class represents the state of the player when they are driving a car.
    public class DrivingState : IState {
        private Car _car;
        private readonly StarterAssetsInputs _playerInput;
        private readonly CharacterController _characterController;
        private readonly PlayerReferenceManager _playerReferenceManager;

        public DrivingState(PlayerReferenceManager playerReferenceManager) {
            _characterController = playerReferenceManager.Controller;
            _playerReferenceManager = playerReferenceManager;
            _playerInput = playerReferenceManager.Input;
        }
        
        // It sets up the necessary parameters and starts the car engine.
        public void OnEnter() {
            // Get the car to enter and set the camera blend.
            _car = _playerReferenceManager.CarToEnter;
            _playerReferenceManager.Brain.m_DefaultBlend = _playerReferenceManager.EaseBlend;

            // Set the driving animation and adjust the camera to follow the car.
            _playerReferenceManager.Animator.SetBool(PlayerReferenceManager.ANIM_DRIVE, true);
            _playerReferenceManager.CarCamera.m_Follow = _car.cameraRoot;
            _playerReferenceManager.CarCamera.m_LookAt = _car.cameraRoot;
            _playerReferenceManager.CarCamera.Priority = _playerReferenceManager.CinemachineActivePriority;

            // Disable the character controller and move the player to the driver's seat.
            _characterController.enabled = false;
            _playerReferenceManager.transform.SetPositionAndRotation(_car.driverSeat.position, _car.driverSeat.rotation);
            _playerReferenceManager.transform.SetParent(_car.carParent.transform);

            // Start the car engine.
            _car.StartEngine(_playerInput);
        }

        public void Tick() {
            // No operation is performed here because the car is reading the input directly.
        }

        // It resets the player and camera settings to their default state.
        public void OnExit() {
            // Reset the camera blend and move the player out of the car.
            _playerReferenceManager.Brain.m_DefaultBlend = _playerReferenceManager.DefaultBlend;
            _playerReferenceManager.transform.position = _car.GetOutPosition;

            // Detach the player from the car and reset the camera priority.
            _playerReferenceManager.transform.SetParent(null);
            _playerReferenceManager.CarCamera.Priority = _playerReferenceManager.CameraInactivePriority;

            // Stop the driving animation and clear the car to enter.
            _playerReferenceManager.Animator.SetBool(PlayerReferenceManager.ANIM_DRIVE, false);
            _playerReferenceManager.CarToEnter = null;

            // Enable the character controller and detach the camera from the car.
            _characterController.enabled = true;
            _playerReferenceManager.CarCamera.m_Follow = null;
            _playerReferenceManager.CarCamera.m_LookAt = null;
        }
        public Color GizmoState() {
            return Color.red;
        }
    }
}