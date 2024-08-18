using InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.MonoBehaviors
{
    public class PlayerCameraController : MonoBehaviour
    {
        //The follow target set in the Cinemachine Virtual Camera that the camera will follow
        [field: SerializeField] public GameObject CinemachineCameraTarget { get; private set; }

        [Tooltip("How far in degrees can you move the camera up")]
        [field: SerializeField] public float TopClamp { get; private set; } = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        [field: SerializeField] public float BottomClamp { get; private set; } = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        [field: SerializeField] public float CameraAngleOverride { get; private set; }

        [Tooltip("For locking the camera position on all axis")]
        [field: SerializeField] public bool LockCameraPosition { get; set; }
    
        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private const float _threshold = 0.01f;
    
        private PlayerReferenceManager _playerReferenceManager;
        private StarterAssetsInputs _input;
    
    
#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif

        private void Awake()
        {
            _playerReferenceManager = GetComponent<PlayerReferenceManager>();
        }

        private void Start()
        {
            _input = _playerReferenceManager.Input;
        
#if ENABLE_INPUT_SYSTEM 
            _playerInput = _playerReferenceManager.PlayerInput;
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

        
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        }

        private void LateUpdate()
        {
            CameraRotation();
        }
        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }
        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
            return false;
#endif
            }
        }
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}
