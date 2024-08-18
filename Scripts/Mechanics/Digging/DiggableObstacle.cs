using System;
using System.Collections;
using Interactable.Carriable;
using Player.Sensors;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class DiggableObstacle : MonoBehaviour, ICarriable {
    [field: SerializeField] public string DiggableName { get; private set; }
    [field: SerializeField] public Vector3 EquipCoordinates { get; set; } = new Vector3(0,0,0);
    [field: SerializeField] public Vector3 EquipRotation { get; set; } = new Vector3(0,0,0);
    
    [BoxGroup("Despawning Object")] [SerializeField] private ParticleSystem despawnEffect;
    [BoxGroup("Despawning Object")] [SerializeField] private AudioClip despawnSound;
    private Rigidbody _rigidBody;
    private void Awake() {
        _rigidBody = GetComponent<Rigidbody>();
        
        // Subscribe to the OnTimerStop event
        _despawnTimer = new CountdownTimer(Random.Range(.1f, 1.3f));
        _despawnTimer.OnTimerStop += OnTimerFinished;
    }

    private float _bounceDuration = 0.5f;
    private float _bounceHeight = 0.5f;

    private Vector3 _startPosition;

    private void Start() {
        _startPosition = transform.position;
        StartCoroutine(Bounce());
    }

    private IEnumerator Bounce() {
        float time = 0;

        while (time < _bounceDuration)
        {
            float bounceValue = Mathf.Sin((time / _bounceDuration) * Mathf.PI) * _bounceHeight;
            transform.position = _startPosition + new Vector3(0, bounceValue, 0);
            yield return null;
            time += Time.deltaTime;
        }
        
        transform.position = _startPosition;
    }
    public IEnumerator Interact(Transform position, Action onInteractFinished) {
        _rigidBody.isKinematic = true;

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

    public IEnumerator Drop(Action onDropFinished)
    {
        _rigidBody.isKinematic = false;
        transform.SetParent(null);

        yield return new WaitForSeconds(1f);
        onDropFinished?.Invoke();
    }
    
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