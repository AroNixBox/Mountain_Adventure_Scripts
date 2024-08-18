using UnityEngine;
using Random = UnityEngine.Random;

namespace Player.MonoBehaviors
{
    public class PlayerAnimationEventListener : MonoBehaviour
    {
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
        [SerializeField] private AudioClip hoeClip;
        [SerializeField] private AudioClip wateringCanClip;
        [SerializeField] private AudioClip shovelClip;
    
        private PlayerReferenceManager _playerReferenceManager;
        private CharacterController _controller;

        private void Awake()
        {
            _playerReferenceManager = GetComponent<PlayerReferenceManager>(); 
        }

        private void Start()
        {
            _controller = _playerReferenceManager.Controller;
        }

        // animation events

        #region Footsteps

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        #endregion

        #region SeasonDial

        private void OnSeasonDialPulledOut(AnimationEvent animationEvent) {
            _playerReferenceManager.PulledOutDial = true;
        }
        
        private void OnSeasonDialPutBack(AnimationEvent animationEvent) {
            _playerReferenceManager.PutAwayDial = true;
        }
        
        private void OnRitualEnd(AnimationEvent animationEvent) {
            _playerReferenceManager.IsRitualFinished = true;
        }

        #endregion

        #region Fishing
        
        private void OnThrowHook(AnimationEvent animationEvent) {
            _playerReferenceManager.ToolUseAnimEvent = true;
        }
        
        private void OnGetHook(AnimationEvent animationEvent) {
            _playerReferenceManager.ToolStopUseAnimEvent = true;
        }

        #endregion

        #region Interaction

        public void OnInteractComplete(AnimationEvent animationEvent) {
            _playerReferenceManager.IsInteractionAnimationDone = true;
        }

        #endregion

        #region Dialogue
        
        private void OnPlayerTalkAnimDone(AnimationEvent animationEvent)
        {
            _playerReferenceManager.IsPlayerTalkAnimationDone = true;
        }

        #endregion


        #region ONLY SOUNDS

        public void HoeSound() {
            AudioSource.PlayClipAtPoint(hoeClip, transform.position);
        }
        
        public void WateringCanSound() {
            AudioSource.PlayClipAtPoint(wateringCanClip, transform.position);
        }
        
        public void ShovelSound() {
            AudioSource.PlayClipAtPoint(shovelClip, transform.position);
        }
        

        #endregion
    }
}
