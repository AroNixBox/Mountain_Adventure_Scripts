using UnityEngine;

namespace Player.Sensors {
    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
    public class DropSensor : MonoBehaviour {
        private Rigidbody _rigidbody;
        private static BoxCollider _boxCollider;
        private static Collider[] _results = new Collider[1];

        private void Awake() {
            _boxCollider = GetComponent<BoxCollider>();
            _boxCollider.isTrigger = true;
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
        }

        
        public static bool IsEmpty() {
            var mask = -1;
            // Exclude triggers
            int num = Physics.OverlapBoxNonAlloc(_boxCollider.bounds.center, _boxCollider.bounds.extents, _results, _boxCollider.transform.rotation, mask, QueryTriggerInteraction.Ignore);
            int count = 0;
            for (int i = 0; i < num; i++) {
                if (_results[i].gameObject != _boxCollider.gameObject && !_results[i].gameObject.CompareTag("Player")) {
                    count++;
                }
            }
            return count == 0;
        }
    }
}