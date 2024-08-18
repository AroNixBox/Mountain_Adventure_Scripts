using System;
using System.Collections;
using Cinemachine;
using InputSystem;
using Interactable;
using Interactable.Carriable;
using Interactable.NPC;
using Mechanics.Driving;
using Mechanics.Fishing.Minigame;
using Mechanics.SeasonSwitch;
using Mechanics.SeasonSwitch.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.MonoBehaviors
{
    public class PlayerReferenceManager : MonoBehaviour
    {
        #region Monobehavior
        
        public PlayerInput PlayerInput { get; private set; }
        public AudioSource AudioSource { get; private set; }
        public Animator Animator { get; private set; }
        public CharacterController Controller { get; private set; }
        public StarterAssetsInputs Input { get; private set; }
        public GravityHandler GravityHandler { get; private set; }
        public InteractionSensor InteractionSensor { get; private set; }
        public PlayerRewardHandler PlayerRewardHandler { get; private set; }
        
        #endregion

        #region Movement

        //Speed of the Character in m/s
        [field: SerializeField] public float MoveSpeed { get; private set; } = 2.0f;
        
        //Speed of the Character while sprinting in m/s
        [field: SerializeField] public float SprintSpeed { get; private set; } = 5.335f;
        
        //RotationSmoothTime of the Character
        [field: SerializeField] public float RotationSmoothTime { get; private set; } = 0.12f;
        
        //Acceleration and Deceleration of the Character
        [field: SerializeField] public float SpeedChangeRate { get; private set; } = 10.0f;
        
        //Height of the Jump in m
        [field: SerializeField] public float JumpHeight { get; private set; } = 1.2f;
        //Timeout after jump is performed, cant exit out of Jump if JumpTimeout is not 0, so basically Threshold,
        //Jump at least need to last this amount of Time
        [field: SerializeField] public float PerformingJumpThreshold { get; private set; } = 0.50f;
        
        //After Grounded, time until next Jump can be perfromed again
        //Because we use a spherecast, if we are "grounded", we are not directly on the ground
        [field: SerializeField] public float JumpTimeOut { get; private set; } = 0.50f;
        public float VerticalVelocity { get; set; }

        #endregion

        #region Camera
        public Camera MainCamera { get; private set; }
        
        public CinemachineBlendDefinition EaseBlend { get; } = new (
            CinemachineBlendDefinition.Style.EaseInOut, 2f);
        
        public CinemachineBrain Brain { get; private set; }
        public CinemachineBlendDefinition DefaultBlend { get; private set; }
        [field: SerializeField] public CinemachineVirtualCamera CarCamera { get; private set; }
        [field: SerializeField] public int CinemachineActivePriority { get; private set; }= 20;
        public int CameraInactivePriority => 0;

        #endregion
        
        #region SeasonDial
        
        //Animation Events that are triggered when the Element Dial is pulled out or put back
        public bool PulledOutDial { get; set; }
        public bool PutAwayDial { get; set; }
        public bool IsRitualFinished { get; set; }
        public int PhysicalSelectedSeason { get; set; }
        [field: SerializeField] public CinemachineVirtualCamera DialCamera { get; private set; }
        [field: SerializeField] public PhysicalSeasonCompass PhysicalSeasonCompass { get; private set; }
        [field: SerializeField] public SeasonSwitchManager SeasonSwitchManager { get; private set; }

        #endregion
        
        #region Animation Hashes
        public int ANIM_SPEED { get; private set; } = Animator.StringToHash("Speed");
        public int ANIM_JUMP { get; private set; } = Animator.StringToHash("Jump");
        public int ANIM_GROUNDED { get; private set; } = Animator.StringToHash("Grounded");
        public int ANIM_FREEFALL { get; private set; } = Animator.StringToHash("FreeFall");
        public int ANIM_RITUAL { get; private set; } = Animator.StringToHash("Ritual");
        public int ANIM_EQUIPCOMPASS { get; private set; } = Animator.StringToHash("EquipCompass");
        public static int ANIM_STARTFISHING { get; private set; } = Animator.StringToHash("StartFishing");
        public static int ANIM_STOPFISHING { get; private set; } = Animator.StringToHash("StopFishing");
        public static int ANIM_FISHCAUGHT { get; private set; } = Animator.StringToHash("FishCaught");
        public int ANIM_TALK { get; private set; } = Animator.StringToHash("Talk");
        public int ANIM_THANK { get; private set; } = Animator.StringToHash("Thank");
        public int ANIM_REWARD { get; private set; } = Animator.StringToHash("Reward");
        public static int ANIM_WATERPLANTS { get; private set; } = Animator.StringToHash("WaterPlants");
        public static int ANIM_FARMWORK { get; private set; } = Animator.StringToHash("FarmWork");
        public static int ANIM_DRIVE { get; private set; } = Animator.StringToHash("Drive");

        public static int ANIM_SHOVEL { get; private set; } = Animator.StringToHash("Shovel");
        #endregion

        #region Item
        public IInteractable PossibleInteractable { get; set; }
        public IInteractable CurrentInteractable { get; set; }
        public bool IsInteractionAnimationDone { get; set; }
        public ICarriable EquippedItem { get; set; }
        public Car CarToEnter { get; set; }
        [field: SerializeField] public Transform CarriableEquipPosition { get; private set; }
        
        [field: SerializeField] public Transform RightHandIKTarget { get; private set; }

        #endregion

        #region Tool

        // Triggered when the Player has used the Tool
        public bool ToolUseAnimEvent { get; set; }
        //Triggered when the Initial ToolUse Animation from Above triggered the Physical Action and this one is completed.
        //Done by Callback
        //E.g. Fishing Rod: Throw Complete or Stop Complete
        //Used for Both!, If there is just one, set it true whenever e.g. Axe Swing is completed
        public bool IsActionCompleted { get; set; }
        //Triggered by the Animation
        public bool ToolStopUseAnimEvent { get; set; }
        //Call when player was e.g. patient enough and the MiniGame should start
        public Hook CurrentFishingHook { get; set; }
        [field: SerializeField] public GameObject FishingInfoPopup { get; private set; }
        //Trigger when Player won the Minigame
        public bool WonMiniGame { get; set; }
        //Trigger when Player lost the Minigame
        public bool LostMiniGame { get; set; }
        [field: SerializeField] public float FishingMouseInputSensitivityMultiplier { get; private set; } = 20f;
        [field: SerializeField] public float FishingControllerInputMultiplier { get; private set; } = 400f;
        [field: SerializeField] public Rigidbody MinigameCircleRigidbody { get; private set; }

        #endregion

        #region NPC
        //The Time the Player looks at the Event before exiting the State
        [field: SerializeField] public float LookAtEventTime { get; private set; } = 3f;
        public bool IsPlayerTalkAnimationDone { get; set; }
        public INpc CurrentNpc { get; set; }
        [field: SerializeField] public Transform PlayerHead { get; private set; }

        #endregion

        #region Sound
        
        [field: SerializeField] public AudioClip[] ThankfulSounds { get; private set; }
        [field: SerializeField] public AudioClip[] TalkSounds { get; private set; }
        [field: SerializeField] public AudioClip[] RitualSounds { get; private set; }
        [field: SerializeField] public AudioClip[] PlayerRewardSounds { get; private set; }

        #endregion
        
        private void Awake()
        {
            PlayerInput = GetComponent<PlayerInput>();
            Animator = GetComponent<Animator>();
            Controller = GetComponent<CharacterController>();
            Input = GetComponent<StarterAssetsInputs>();
            MainCamera = Camera.main;
            GravityHandler = GetComponent<GravityHandler>();
            AudioSource = GetComponent<AudioSource>();
            InteractionSensor = GetComponentInChildren<InteractionSensor>();
            PlayerRewardHandler = GetComponent<PlayerRewardHandler>();
        }

        private void Start() {
            Brain = CinemachineCore.Instance.GetActiveBrain(0);
            DefaultBlend = Brain.m_DefaultBlend;
        }

        public float GetCurrentMoveSpeed() {
            float targetSpeed = Input.sprint ? SprintSpeed : MoveSpeed;
            float inputMagnitude = Input.analogMovement ? Input.move.magnitude : 1f;

            if (Input.move == Vector2.zero) {
                targetSpeed = 0.0f;
            }

            float currentHorizontalSpeed = new Vector3(Controller.velocity.x, 0.0f, Controller.velocity.z).magnitude;

            float speedOffset = 0.1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset) {
                return Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
            }
            return targetSpeed;
        }

        private const float ControllerCheckRate = 5f;
        private float _currentTime = 5f;
        
        private void Update() {
            _currentTime += Time.deltaTime;

            if (_currentTime >= ControllerCheckRate) {
                _currentTime = 0f;
                UIEvents.InputDeviceChanged.Get().Invoke(IsControllerConnected());
            }
        }
        
        public bool IsControllerConnected() {
            var joysticks = UnityEngine.Input.GetJoystickNames();
            return joysticks.Length > 0 && !string.IsNullOrEmpty(joysticks[0]);
        }
    }
}