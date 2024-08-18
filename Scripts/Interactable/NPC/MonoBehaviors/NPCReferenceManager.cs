using Cinemachine;
using Interactable.NPC.States;
using Movement.Player.States;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Interactable.NPC
{
    public class NPCReferenceManager : MonoBehaviour
    {
        #region MonoBehaviours
        public Animator Animator { get; private set; }
        public AudioSource AudioSource { get; private set; }
        
        #endregion

        #region References
        public Transform PlayerHead { get; private set; }
        [field: SerializeField] public SuperTextMesh DialogueTMP { get; private set; }
        [field: SerializeField] public Transform NpcHead { get; private set; }
        [field: SerializeField, InlineEditor] public DialogueSO DialogueSO { get; private set; }
        //Regarding if DialogueSO ICarriable is Null or not, one of them is assigned in Start
        public Quest Quest { get; private set; }
        
        [ShowIf("@DialogueSO != null && DialogueSO.SelectedQuestType == QuestType.PointQuest")]
        public UnityEvent OnPointQuestFinished;
        [ShowIf("@DialogueSO != null && DialogueSO.SelectedQuestType == QuestType.PointQuest")]
        public float LookAtEventWaitTime = 2f;
        
        
        /// <summary>
        /// Used to localize the Transform of the first Subscriber of the OnPointQuestFinished Event
        /// </summary>
        /// <returns></returns>
        public Transform GetFirstEventListener()
        {
            if (OnPointQuestFinished != null && OnPointQuestFinished.GetPersistentEventCount() > 0)
            {
                var listenerObject = OnPointQuestFinished.GetPersistentTarget(0);
                //If ListenerObject is on a GameObject
                if (listenerObject is GameObject listenerGameObject)
                {
                    return listenerGameObject.transform;
                }
                else
                {
                    //If ListenerObject is on a Component
                    if (listenerObject is Component listenerComponent)
                    {
                        return listenerComponent.transform;
                    }
                }
            }

            return null;
        }
        
        #endregion

        #region Camera
        private Transform _vcamTarget;
        private CinemachineVirtualCamera _vcam;
        private readonly CinemachineBlendDefinition _easeBlend = new (
            CinemachineBlendDefinition.Style.EaseInOut, 2f);
        private CinemachineBrain _brain;
        private CinemachineBlendDefinition _defaultBlend;
        
        private CinemachineVirtualCamera _eventVcam;
        private Transform _eventCameraTarget;

        private readonly int _virtualCameraActivePriority = 20;
        private int _virtualCameraInactivePriority;

        #endregion
        
        #region Animation Hashes

        public static readonly int ANIM_TURNSPEED = Animator.StringToHash("TurnSpeed");
        public static readonly int ANIM_TURN = Animator.StringToHash("Turn");
        public static readonly int ANIM_POINT = Animator.StringToHash("Point");
        public static readonly int ANIM_REWARDPLAYER = Animator.StringToHash("RewardPlayer");

        #endregion

        #region Transition Flags
        
        public bool HasFinishedPrintingTextToScreen { get; set; }
        public DialogueState CurrentDialogueState { get; set; } = DialogueState.None;
        public bool Interact { get; set; }
        public bool Talk { get; set; }
        public bool HasRewardedPlayer { get; set; }

        #endregion
        private void Awake() {
            AudioSource = GetComponent<AudioSource>();
            Animator = GetComponent<Animator>();
        }
        private void Start() {
            if(GameReferenceCarrier.Instance == null)
            {
                Debug.LogError("GameReferenceCarrier is missing in the Scene!");
                return;
            }
            
            //Assign our References through the Manager
            PlayerHead = GameReferenceCarrier.Instance.PlayerHead;
            _vcamTarget = GameReferenceCarrier.Instance.VcamTarget;
            _vcam = GameReferenceCarrier.Instance.Vcam;
            _eventVcam = GameReferenceCarrier.Instance.EventVcam;
            _eventCameraTarget = GameReferenceCarrier.Instance.EventCameraTarget;
            
            Quest = DialogueSO.GetQuest();
            
            _brain = CinemachineCore.Instance.GetActiveBrain(0);
            _defaultBlend = _brain.m_DefaultBlend;
        }

        public void SetupDialogueCamera(Transform playerHeadTransform) {
            var midPoint = (playerHeadTransform.position + NpcHead.position) / 2;
            _vcamTarget.position = midPoint;
        
            var vcamTargetRotation = Quaternion.LookRotation(playerHeadTransform.forward);
            _vcamTarget.rotation = vcamTargetRotation;
        
            //Change the blend of the camera
            _brain.m_DefaultBlend = _easeBlend;
        
            _vcam.Priority = _virtualCameraActivePriority;
        }
        
        public void SetupEventCamera(Transform eventTransform) {
            Vector3 direction = (eventTransform.position - NpcHead.position).normalized;
            Vector3 position = NpcHead.position + direction * 7f + Vector3.up * 5f;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            _eventCameraTarget.rotation = targetRotation;
            _eventCameraTarget.position = position;

            _eventVcam.Priority = _virtualCameraActivePriority;
            _vcam.Priority = _virtualCameraInactivePriority;
        }
        public void ResetDialogueCamera() {
            _vcam.Priority = _virtualCameraInactivePriority;
            _eventVcam.Priority = _virtualCameraInactivePriority;
            _brain.m_DefaultBlend = _defaultBlend;
        }
    }
}