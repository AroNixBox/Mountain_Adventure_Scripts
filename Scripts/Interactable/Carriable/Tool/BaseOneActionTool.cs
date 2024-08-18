using System;
using System.Collections;
using System.Linq;
using Mechanics.Gardening;
using Player.MonoBehaviors;
using UnityEngine;

namespace Interactable.Carriable.Tool {
    public abstract class BaseOneActionTool : MonoBehaviour, ITool {
        protected TipSensor ToolTip;
        private Collider _collider;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;
        protected PlayerReferenceManager PlayerReferenceManager;

        private void Awake() {
            ToolTip = GetComponentInChildren<TipSensor>();
            _collider = GetComponent<Collider>();
            PlayerReferenceManager = FindObjectsByType<PlayerReferenceManager>(FindObjectsSortMode.None).FirstOrDefault();
        }

        private void Start() {
            _initialPosition = transform.position;
            _initialRotation = transform.rotation;
        }

        public IEnumerator Interact(Transform position, Action onInteractFinished) {
            _collider.enabled = false;
            transform.SetParent(position);
            transform.SetLocalPositionAndRotation(EquipCoordinates, Quaternion.Euler(EquipRotation)); 
            
            yield return null;
            //Callback tells the player when Interact is performed
            onInteractFinished?.Invoke();
        }

        public Vector3 GetPosition() {
            return transform.position;
        } 

        public bool CanPerformDrop () {
            return true;
        }

        public IEnumerator Drop(Action onDropFinished) {
            _collider.enabled = true;
            transform.SetParent(null);
            transform.SetLocalPositionAndRotation(_initialPosition, _initialRotation); 
            
            //Wait for next frame
            yield return null;
            
            onDropFinished?.Invoke();
        }
        // TODO: Override this in the child classes
        public abstract Vector3 EquipCoordinates { get; set; }
        public abstract Vector3 EquipRotation { get; }
        public abstract void PerformAction();

        public virtual int GetUseActionHash() { // TODO: Override
            return 0; // OVERRIDE WITH THE CORRECT HASH
        }

        public int GetStopActionHash() {// 0 is returned if the player has no StopUse Action
            return 0;
        }

        public int GetWonMinigameActionHash(){ // 0 is returned if the player has no StopUse Action
            return 0;
        }

        public Vector3 InitialPosition => _initialPosition;

        public virtual void Despawn() {
            // NOOP, if want to destroy a tool, override this method
        }
    }
}
