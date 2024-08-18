using System;
using System.Collections;
using Player.Sensors;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Interactable.Carriable
{
    public class PhysicalFish : MonoBehaviour, ICarriable {
        [field: SerializeField] public string FishName {get; private set; }
        [field: SerializeField, Range(1f, 100f)] public int WaitTimeUntilFishBites { get; private set; } = 1;
        [field: SerializeField, Range(1f, 100f)] public int MinigameDuration { get; private set; } = 7;
        [field: SerializeField, Range(.4f, 2f)] public float MiniGameDifficulty { get; private set; } = .75f; // Speed of Hook in a Minigame
        
        [BoxGroup("Despawning Object")] [SerializeField] private ParticleSystem despawnEffect;
        [BoxGroup("Despawning Object")] [SerializeField] private AudioClip despawnSound;
        
        public Rigidbody Rigidbody { get; private set; }
        public Collider Collider { get; private set; }

        private void Awake() {
            Rigidbody = GetComponent<Rigidbody>();
            Collider = GetComponent<Collider>();
            
            // Subscribe to the OnTimerStop event
            _despawnTimer = new CountdownTimer(Random.Range(.1f, 1.3f));
            _despawnTimer.OnTimerStop += OnTimerFinished;
        }

        public IEnumerator Interact(Transform position, Action onInteractFinished)
        {
            Rigidbody.isKinematic = true;

            //AnimationEvent was Thrown
            transform.SetParent(position);
            transform.SetLocalPositionAndRotation(EquipCoordinates, Quaternion.Euler(EquipRotation));
            
            yield return new WaitForSeconds(1f);
            onInteractFinished?.Invoke();
        }

        public Vector3 GetPosition() {
            return transform.position;
        }

        public bool CanPerformDrop() {
            return DropSensor.IsEmpty();
        }

        public IEnumerator Drop(Action onDropFinished) {
            Rigidbody.isKinematic = false;
            transform.SetParent(null);

            yield return new WaitForSeconds(1f);
            onDropFinished?.Invoke();
        }

        public Vector3 EquipCoordinates { get; set; } = new Vector3(0.0012f, 0.0028f, 0.286f);
        public Vector3 EquipRotation { get; } = new Vector3(-62f, -288.283f, 206.441f);
        
        // Create a new CountdownTimer with a duration of 5 seconds
        private CountdownTimer _despawnTimer;
        
        public void Despawn() {
            // Start the timer
            _despawnTimer.Start();
        }

        private void OnTimerFinished() {
            // This logic will be executed when the timer finishes
            Instantiate(despawnEffect, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(despawnSound, transform.position);
            Destroy(gameObject);
        }

        private void Update() {
            // Update the timer
            _despawnTimer.Tick(Time.deltaTime);
        }
    }
}