using UnityEngine;

namespace Mechanics.Fishing.Buoyancy
{
    [RequireComponent(typeof(Rigidbody))]
    public class BuoyancyObject : MonoBehaviour
    {
        [SerializeField] private Transform[] floatPoints;
        [SerializeField] private float underwaterDrag = 3f;
        [SerializeField] private float underwaterAngularDrag = 1f;
        [SerializeField] private float airDrag = 0;
        [SerializeField] private float airAngularDrag = 0.05f;
        [SerializeField] private float floatingPower = 15f;
        private Rigidbody _rigidbody;
        
        protected int WaterLayer => LayerMask.NameToLayer("Water");
        protected bool IsInWater { get; private set; }
        private bool _isUnderwater;
        private float _waterHeight;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == WaterLayer)
            {
                IsInWater = true;
                _waterHeight = other.transform.position.y;
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == WaterLayer)
            {
                IsInWater = false;
            }
        }

        private void FixedUpdate()
        {
            if (IsInWater)
            {
                bool isUnderwater = false;
                foreach (var floatPoint in floatPoints)
                {
                    float difference = floatPoint.position.y - _waterHeight;

                    if (difference < 0)
                    {
                        Vector3 buoyancyForce = Vector3.up * (floatingPower * Mathf.Abs(difference));
                        _rigidbody.AddForceAtPosition(buoyancyForce, floatPoint.position, ForceMode.Force);
                        isUnderwater = true;
                    }
                }

                if (isUnderwater != _isUnderwater)
                {
                    _isUnderwater = isUnderwater;
                    SwitchState(_isUnderwater);
                }
            }
            else
            {
                if (_isUnderwater)
                {
                    _isUnderwater = false;
                    SwitchState(false);
                }
            }
        }

        private void SwitchState(bool isUnderwater)
        {
            if (isUnderwater)
            {
                _rigidbody.drag = underwaterDrag;
                _rigidbody.angularDrag = underwaterAngularDrag;
            }
            else
            {
                _rigidbody.drag = airDrag;
                _rigidbody.angularDrag = airAngularDrag;
            }
        }
    }
}