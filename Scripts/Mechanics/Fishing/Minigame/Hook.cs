using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interactable.Carriable;
using Interactable.Carriable.Pickable;
using Mechanics.Fishing.Buoyancy;
using Mechanics.Fishing.Data;
using Player.MonoBehaviors;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mechanics.Fishing.Minigame
{
    [RequireComponent(typeof(AudioSource))]
    public class Hook : BuoyancyObject
    {
        [Header("Fishing Rod")]
        [SerializeField] private RodFishHolder rodFishHolder;
        [SerializeField] private FishBaitController fishBaitController;
        private PlayerReferenceManager _playerReferenceManager;
        
        [Header("Fishing Rod")]
        [Tooltip("The target points the hook will move between")]
        [SerializeField] private Transform[] targetPoints;
        
        [Header("Audio")]
        [SerializeField] private AudioClip[] hookSplashSounds;

        [Header("Values")] 
        [SerializeField] private float waitTimeBeforeGameStarts = 3.5f;
        
        private FishingRod _fishingRod;
        private AudioSource _audioSource;
        
        public IEnumerator MoveHookCoroutine { get; private set; }
        
        private int _currentTargetIndex;
        private float _currentSpeed;

        private readonly UIEvents.CountdownData _countdownData = new ();
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _playerReferenceManager = FindObjectsByType<PlayerReferenceManager>(FindObjectsSortMode.None).FirstOrDefault();
        }

        /// <summary>
        /// Starts the Waiting for MiniGame, if there is a baitFish the chance is higher to catch a rare fish
        /// </summary>
        /// <param name="fishingRod"></param>
        /// <param name="baitFish"></param>
        public void Init(FishingRod fishingRod, PhysicalFish baitFish = null)
        {
            _fishingRod = fishingRod;
            StartCoroutine(WaitAndRequestFish(baitFish));
        }
        
        /// <summary>
        /// Picks a fish, waits, if fish found starts the MiniGame
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitAndRequestFish(PhysicalFish baitFish = null) {
            if (baitFish != null) { // Only if there is a fish on the Hook already, when throwing it in. So we use a fish to catch another Fish
                // TODO: Catch something cool when we have a fish on the rod
            }
            PhysicalFish fish = GetFish();
            
            var waitTime = Random.Range(rodFishHolder.SmallestWaitTime, fish.WaitTimeUntilFishBites);
            var clampedWaitTime = Mathf.Clamp(waitTime, 5, 20);

            yield return new WaitForSeconds(clampedWaitTime);

            //We arent in water or no fish found
            if (!IsInWater || fish == null) { yield break;}
            
            
            //Create the Coroutine save it locally to stop it when needed, Finish Action is Aborting the Players Possess of the FishBaitController
            //Tells the Rod that the player won the MiniGame and caught the fish
            MoveHookCoroutine = MoveHook(fish.MinigameDuration, () => {
                // Callback when the game has ended
                fishBaitController.Abort();
                _fishingRod.CatchFish(fish); 
                _playerReferenceManager.WonMiniGame = true;
                MoveHookCoroutine = null;
            }, fish);
            
            //Method Init puts on new camera, sets MiniGameObject to object (MiniGame Prep)
            //This callback is Invoked fom the FishBaitController when the fishing fails.
            fishBaitController.Init(transform.position, () => {
                if (MoveHookCoroutine != null) { // Abortion:
                    
                    // Coroutines can only be stopped from the same Class that started them
                    _playerReferenceManager.StopCoroutine(MoveHookCoroutine);
                    MoveHookCoroutine = null;
                    _playerReferenceManager.LostMiniGame = true;
                }
                fish = null;
            }); 
            
            if(MoveHookCoroutine == null) { yield break; } // Last check if the game was aborted (Hook pulled out of water)
            
            // Flag to go into the InfoPanelState on the PlayerStateMachine
            _playerReferenceManager.CurrentFishingHook = this;
        }

        /// <summary>
        /// Moves the Hook between target points
        /// </summary>
        /// <param name="moveTime"></param>
        /// <param name="callback"></param>
        /// <param name="fish"></param>
        /// <returns></returns>
        private IEnumerator MoveHook(float moveTime, Action callback, PhysicalFish fish) {
            List<Vector3> localTargetPoints = new List<Vector3>();
            foreach (var targetPoint in targetPoints) {
                var targetPointWithOnlyY = new Vector3(targetPoint.position.x, transform.position.y, targetPoint.position.z);
                localTargetPoints.Add(targetPointWithOnlyY);
            }
    
            float time = 0;
            float movementMultiplier = 0;
            _currentTargetIndex = 0;

            while (time < moveTime) {
                movementMultiplier += 0.01f;
                _currentSpeed = movementMultiplier * fish.MiniGameDifficulty;
                _currentSpeed = Mathf.Clamp(_currentSpeed, fish.MiniGameDifficulty, fish.MiniGameDifficulty * 3);
                while ((transform.position - localTargetPoints[_currentTargetIndex]).sqrMagnitude > 0.1f)
                {
                    Vector3 direction = (localTargetPoints[_currentTargetIndex] - transform.position).normalized;
                    transform.position += direction * (_currentSpeed * Time.deltaTime);
                    time += Time.deltaTime; 
                    yield return null;
                }
                _currentTargetIndex = (_currentTargetIndex + 1) % localTargetPoints.Count;
            }

            callback?.Invoke();
        }
        
        private PhysicalFish GetFish() {
            _fishIndex = (_fishIndex + 1) % rodFishHolder.fishDataList.Count;
            return rodFishHolder.fishDataList[_fishIndex];
        }
        private int _fishIndex;
        
        //For some reason this is also called when the hook exits the water
        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            if (other.gameObject.layer == WaterLayer)
            {
                _audioSource.PlayOneShot(hookSplashSounds[Random.Range(0, hookSplashSounds.Length)]);
            }
            
        }
    }
}