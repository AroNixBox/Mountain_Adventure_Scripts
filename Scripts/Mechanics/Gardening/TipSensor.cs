using System.Collections.Generic;
using Interactable.Carriable.Tool;
using UnityEngine;

namespace Mechanics.Gardening {
    public class TipSensor : MonoBehaviour {
        private Rigidbody _rigidbody;
        private SphereCollider _collider;
        private readonly List<IReactableToTool> _reactableObjects = new ();

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<SphereCollider>();

            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out IReactableToTool reactableObject)) {
                _reactableObjects.Add(reactableObject);
            }
        }

        public T GetClosestObjectOfType<T>() where T : class, IReactableToTool {
            T closestObject = default;
            float closestDistance = float.MaxValue;
            for (int i = _reactableObjects.Count - 1; i >= 0; i--) {
                IReactableToTool reactableObject = _reactableObjects[i];
                if (reactableObject is T t) {
                    Component component = t as Component;
                    if (component == null || component.gameObject == null) {
                        _reactableObjects.RemoveAt(i);
                        continue;
                    }
                    Vector3 difference = component.transform.position - transform.position;
                    float distance = difference.sqrMagnitude;
                    if (distance < closestDistance) {
                        closestDistance = distance;
                        closestObject = t;
                    }
                }
            }
            return closestObject;
        }

        private void OnTriggerExit(Collider other) {
            if (other.TryGetComponent(out IReactableToTool reactableObject)) {
                _reactableObjects.Remove(reactableObject);
            }
        }
    }
}