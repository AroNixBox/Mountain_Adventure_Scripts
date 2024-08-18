using System;
using System.Collections;
using System.Linq;
using InputSystem;
using Interactable;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace Mechanics.Driving {
    public class Car : MonoBehaviour, IInteractable {
        // Car related components
        [FoldoutGroup("Car Components")] 
        public GameObject carParent;
        [FoldoutGroup("Car Components")] 
        public Transform driverSeat;
        [FoldoutGroup("Car Components")]
        public Transform leftDoorTransform;
        [FoldoutGroup("Car Components")]
        [SerializeField] private Transform[] getOutPositions;
        [FoldoutGroup("Car Components")] public Transform cameraRoot;

        // Car physics parameters
        [FoldoutGroup("Car Physics")]
        [SerializeField] private float decelerationSpeed = 1f;
        [FoldoutGroup("Car Physics")]
        [SerializeField] private float motorForce, breakForce, maxSteerAngle;
        [FoldoutGroup("Car Physics")]
        [SerializeField] private float steerSpeed = 5f;
        [FoldoutGroup("Car Physics")]
        [SerializeField] private Wheel[] wheels;

        // Transmission related parameters
        [FoldoutGroup("Transmission")]
        [SerializeField] private int maxGears = 5;
        [FoldoutGroup("Transmission")]
        [SerializeField] private float minSpeedPerGear = 10f;
        [FoldoutGroup("Transmission")]
        [SerializeField] private float maxSpeedPerGear = 20f;
        [FoldoutGroup("Transmission")]
        [SerializeField] private AudioClip startEngineSound;
        [FoldoutGroup("Transmission")]
        [SerializeField] private AudioClip shiftSound;
        [FoldoutGroup("Transmission")]
        [SerializeField] private AudioClip driveSound;
        
        [FoldoutGroup("Trailer")]
        [SerializeField] private bool hasTrailer;
        
        [FoldoutGroup("Trailer")]
        [ShowIf("hasTrailer")]
        [SerializeField] private HingeJoint trailerHitch;

        [FoldoutGroup("Trailer")]
        [ShowIf("hasTrailer")]
        [SerializeField] private Wheel[] trailerWheels;
        
        // Private variables
        private Transmission _transmission;
        private float _currentSteerAngle, _currentbreakForce;
        private bool _isBreaking;
        private bool _isPlayerInCar;
        private AudioSource _carAudioSource;
        private Rigidbody _carRigidbody;
        private StarterAssetsInputs _playerInput;
        private float _maxSpeed;

        // Initialize the car
        private void Start() {
            _carRigidbody = carParent.GetComponent<Rigidbody>();
            _carAudioSource = carParent.GetComponent<AudioSource>();
            _maxSpeed = maxSpeedPerGear * maxGears;
            _transmission = new Transmission(maxGears, minSpeedPerGear, maxSpeedPerGear, shiftSound, driveSound, _carAudioSource, _maxSpeed);
            
            // If we have a trailer, because it is in the GameObject, we need to set the parent to null and connect the body to the car
            if(hasTrailer) {
                trailerHitch.transform.SetParent(null);
                trailerHitch.connectedBody = _carRigidbody;
            }
        }

        // Interact with the car
        public IEnumerator Interact(Transform position, Action onInteractFinished) {
            // TODO: Open Door!
            yield return null;
            onInteractFinished?.Invoke();
        }

        // Start the car engine
        public void StartEngine(StarterAssetsInputs playerInput) {
            AudioSource.PlayClipAtPoint(startEngineSound, carParent.transform.position);
            _playerInput = playerInput;
            _isPlayerInCar = true;
        }

        // Get the position to get out of the car
        public Vector3 GetOutPosition {
            get {
                _carAudioSource.Stop();
                _isPlayerInCar = false;

                foreach (var position in getOutPositions) {
                    if (Physics.Raycast(position.position, Vector3.down, out var hit, 2)) {
                        return hit.point;
                    }
                }

                // Fallback
                return carParent.transform.position + new Vector3(0, 10, 0);
            }
        }

        // Check if the player can get out of the car
        public bool CanGetOutOfCar() {
            return getOutPositions.Select(position => 
                Physics.Raycast(position.position, Vector3.down, 2)).FirstOrDefault();
        }

        // Get the position of the car door
        public Vector3 GetPosition() {
            return leftDoorTransform.position;
        }

        // Update the car physics
        private void FixedUpdate() {
            HandleMotorAndBrake();
            if (!_isPlayerInCar) return;

            HandleSteering();
            UpdateWheels();
            
            if (hasTrailer) {
                UpdateTrailerWheels();
            }

            float speed = _carRigidbody.velocity.magnitude;
            _transmission.ShiftGears(speed);
            _transmission.AdjustEngineSound(speed);

            CheckAndCorrectCarPosition();
        }

        // Check and correct the car position if it's flipped over
        private void CheckAndCorrectCarPosition() {
            float zRotation = carParent.transform.rotation.z;
            if (zRotation >= 70f || zRotation <= -70f) { 
                carParent.transform.rotation = Quaternion.Euler(0, 0, 0);
                _carRigidbody.AddForce(Vector3.up * 2, ForceMode.VelocityChange);
            }
        }

        // Handle the car motor and brake
        private void HandleMotorAndBrake() {
            foreach (var wheel in wheels) {
                var wheelCollider = wheel.wheelCollider;

                if (_isPlayerInCar) {
                    if (_playerInput.move.y != 0) {
                        wheelCollider.motorTorque = _playerInput.move.y * motorForce;
                        _currentbreakForce = _isBreaking ? breakForce : 0f;
                        _carRigidbody.drag = 0;
                    } else {
                        wheelCollider.motorTorque = 0;
                        _currentbreakForce = 0f;
                        _carRigidbody.drag = decelerationSpeed;
                    }
                } else {
                    wheelCollider.motorTorque = 0;
                    _currentbreakForce = breakForce;
                    _carRigidbody.drag = decelerationSpeed;
                }

                wheelCollider.brakeTorque = _currentbreakForce;
            }
        }

        // Handle the car steering
        private void HandleSteering() {
            foreach (var wheel in wheels) {
                if (wheel.wheelType == WheelType.Front) {
                    var wheelCollider = wheel.wheelCollider;
                    var targetSteerAngle = maxSteerAngle * _playerInput.move.x;
                    _currentSteerAngle = Mathf.LerpAngle(_currentSteerAngle, targetSteerAngle, Time.deltaTime * steerSpeed);
                    wheelCollider.steerAngle = _currentSteerAngle;
                }
            }
        }

        // Update the car wheels
        private void UpdateWheels() {
            foreach (var wheel in wheels) {
                UpdateSingleWheel(wheel.wheelCollider, wheel.wheelMesh.transform);
            }
        }

        // Update a single wheel
        private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform) {
            wheelCollider.GetWorldPose(out var pos, out var rot);
            wheelTransform.rotation = rot;
            wheelTransform.position = pos;
        }
        
        private void UpdateTrailerWheels() {
            foreach (var wheel in trailerWheels) {
                UpdateSingleWheel(wheel.wheelCollider, wheel.wheelMesh.transform);
            }
        }
    }

    // Wheel class
    [System.Serializable]
    public class Wheel {
        [FoldoutGroup("Wheel Components")]
        public MeshRenderer wheelMesh;
        [FoldoutGroup("Wheel Components")]
        public WheelCollider wheelCollider;
        [FoldoutGroup("Wheel Components")]
        public WheelType wheelType;
    }

    // Wheel type enum
    public enum WheelType {
        Front,
        Rear
    }
}