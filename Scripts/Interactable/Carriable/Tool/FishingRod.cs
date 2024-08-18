using System;
using System.Collections;
using System.Linq;
using Interactable.Carriable.Tool;
using Mechanics.Fishing.Minigame;
using Mechanics.Fishing.Rope;
using Player.MonoBehaviors;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Interactable.Carriable.Pickable
{
    public class FishingRod : MonoBehaviour, ITool
    {
        [Header("References")] 
        [SerializeField] private Hook hook;
        [SerializeField] private Transform hookIdlePosition;
        [SerializeField] private FishingRope rope;
        [SerializeField] private float hookReturnSpeed = 1f;
        
        [Header("Audio")]
        [SerializeField] private AudioClip[] hookThrowSounds, hookReturnSounds;
        
        [Header("Values")]
        [SerializeField] private float hookThrowForce = 8f;
        
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;
        private Collider _collider;
        
        private PlayerReferenceManager _playerReferenceManager;
        private PhysicalFish _caughtFish;
        private Rigidbody _hookRigidbody;
        private Collider _hookCollider;
        
        private AudioSource _audioSource;
        private bool _isHookThrown;
        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _audioSource = GetComponent<AudioSource>();
            _playerReferenceManager = FindObjectsByType<PlayerReferenceManager>(FindObjectsSortMode.None).FirstOrDefault();
        }

        private void Start()
        {
            _hookCollider = hook.GetComponent<Collider>();
            _hookRigidbody = hook.GetComponent<Rigidbody>();
            
            _initialPosition = transform.position;
            _initialRotation = transform.rotation;
        }

        #region Interface

        public IEnumerator Interact(Transform equipPosition, Action onInteractComplete)
        {
            _collider.enabled = false;
            transform.SetParent(equipPosition);
            transform.SetLocalPositionAndRotation(EquipCoordinates, Quaternion.Euler(EquipRotation)); 
            
            yield return null;
            //Callback tells the player when Interact is performed
            onInteractComplete?.Invoke();
        }
        //Throw the hook
        public void PerformAction() {
            if (_isHookThrown) {
                StartCoroutine(GetHook(() => {
                    _playerReferenceManager.IsActionCompleted = true;
                }));
            }else {
                ThrowHook();
                _playerReferenceManager.IsActionCompleted = true;
            }
            _isHookThrown = !_isHookThrown;
        }

        public int GetUseActionHash() {
            return PlayerReferenceManager.ANIM_STARTFISHING;
        }

        public int GetStopActionHash() {
            return PlayerReferenceManager.ANIM_STOPFISHING;
        }

        public int GetWonMinigameActionHash() {
            return PlayerReferenceManager.ANIM_FISHCAUGHT;
        }

        public Vector3 InitialPosition => _initialPosition;

        private void ThrowHook() {
            _audioSource.PlayOneShot(hookThrowSounds[Random.Range(0, hookThrowSounds.Length)]);
            _hookRigidbody.transform.SetParent(null);
            _hookCollider.enabled = true;
            
            _hookRigidbody.isKinematic = false;
            _hookRigidbody.AddForce(hookThrowForce * hook.transform.forward, ForceMode.Impulse);
            rope.ToggleRopeLength();

            hook.Init(this);
        }

        //Dropping the fishing rod
        public bool CanPerformDrop() {
            return true;
        }

        public IEnumerator Drop(Action onDropComplete) {
            DropFish();
            
            _collider.enabled = true;
            transform.SetParent(null);
            transform.SetLocalPositionAndRotation(_initialPosition, _initialRotation); 
            
            //Wait for next frame
            yield return null;
            
            onDropComplete?.Invoke();
        }
        /// <summary>
        /// Makes hook Move to its idle position
        /// </summary>
        private IEnumerator GetHook(Action onGetHookComplete)
        {
            _audioSource.PlayOneShot(hookReturnSounds[Random.Range(0, hookReturnSounds.Length)]);
            _hookRigidbody.isKinematic = true;

            rope.ToggleRopeLength();
            
            var duration = hookReturnSpeed;
            float time = 0;
            var startPosition = _hookRigidbody.position;
            var startRotation = _hookRigidbody.rotation;

            while (time < duration)
            {
                var t = time / duration;

                _hookRigidbody.position = Vector3.Lerp(startPosition, hookIdlePosition.position, t);
                _hookRigidbody.rotation = Quaternion.Lerp(startRotation, hookIdlePosition.rotation, t);

                time += Time.deltaTime;
                yield return null;
            }

            _hookRigidbody.position = hookIdlePosition.position;
            _hookRigidbody.rotation = hookIdlePosition.rotation;
            
            _hookRigidbody.transform.SetParent(transform);
            onGetHookComplete?.Invoke();
        }

        public Vector3 EquipCoordinates { get; set; } = new (-0.067f, -0.184f, -1.137f);
        public Vector3 EquipRotation { get; } = new (-6.65f, 101.358f, -97.69f);

        public Vector3 GetPosition()
        {
            return transform.position;
        }
        #endregion


        private void DropFish()
        {
            if (_caughtFish == null) return;
            
            _caughtFish.transform.SetParent(null);
            _caughtFish.Rigidbody.isKinematic = false;
            _caughtFish.Collider.enabled = true;
            
            _caughtFish = null;
        }

        /// <summary>
        /// Destroys the fish on the hook if there is one and instantiate the new caught one
        /// </summary>
        /// <param name="fish"></param>
        public void CatchFish(PhysicalFish fish)
        {
            if(fish == null) { return; }
            
            //Destroy all fish first before catching a new one
            foreach (Transform existingFish in hook.transform)
            {
                if(existingFish.TryGetComponent(out PhysicalFish physicalFish))
                {
                    Destroy(physicalFish.gameObject);
                }
            }
            
            //Instantiate the fish and set it as a child of the hook
            var caughtFish = Instantiate(fish, hook.transform.position, Quaternion.identity);
            caughtFish.transform.SetParent(hook.transform);
            
            _caughtFish = caughtFish;
            
            //caught something
            _playerReferenceManager.WonMiniGame = true;
        }
        
        public void Despawn() {
            // NOOP
            Debug.Log("Tools are not ment to be destroyed");
        }
    }
}