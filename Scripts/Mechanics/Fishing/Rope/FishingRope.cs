using UnityEngine;
using UnityEngine.Serialization;

namespace Mechanics.Fishing.Rope
{
    public class FishingRope : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform rodTransform;
        [SerializeField] private Transform hookTransform;
        
        [Header("Rope Physics Values")]
        [SerializeField] private float ropeLength = 1f;
        [SerializeField] private float ropeSegmentLength = 0.25f;
        [SerializeField] private float ropeDamping = 0.1f;
        [SerializeField] private float ropeGravity = 98.1f;
        [SerializeField] private float ropeElasticity = 1500f;
        [SerializeField] private float maxRopeVelocity = 250f;
        [SerializeField] private float maxAcceleration = 1000f;
        [SerializeField] private float velocityDampingThreshold = 0.1f;
        [SerializeField] private float velocityDampingFactor = 0.9f;

        private LineRenderer _lineRenderer;
        private RopeSection[] _ropeSections;
        private int _numSections;
        private bool _isRopeHalved = true;

        // Initialize the rope
        private void Start() {
            _lineRenderer = GetComponent<LineRenderer>();
            _numSections = Mathf.CeilToInt(ropeLength / ropeSegmentLength) + 1;
            
            CreateRopeSections();

            _lineRenderer.positionCount = _numSections;
        }

        // Create the rope sections
        private void CreateRopeSections()
        {
            _ropeSections = new RopeSection[_numSections];

            for (var i = 0; i < _numSections; i++)
            {
                _ropeSections[i] = new RopeSection(rodTransform.position);
            }
        }
        /// <summary>
        /// Toggles the rope length between halved and full length.
        /// </summary>
        public void ToggleRopeLength()
        {
            //Is halfed, get long, is long, get halfed
            ropeElasticity = _isRopeHalved ? ropeElasticity / 4 : ropeElasticity * 4;

            _isRopeHalved = !_isRopeHalved;
        }

        private void Update()
        {
            // Update the rope physics
            _ropeSections[0].Pos = rodTransform.position;
            _ropeSections[_numSections - 1].Pos = hookTransform.position;

            // Calculate the velocity and position of each rope section
            for (var i = 1; i < _numSections - 1; i++)
            {
                var velocity = _ropeSections[i].Vel;
                var acceleration = GetAcceleration(i);
                acceleration = Vector3.ClampMagnitude(acceleration, maxAcceleration);

                velocity += acceleration * Time.deltaTime;
                velocity = Vector3.ClampMagnitude(velocity, maxRopeVelocity);
                velocity *= 1f - ropeDamping;

                if (velocity.magnitude < velocityDampingThreshold)
                {
                    velocity *= velocityDampingFactor;
                }

                _ropeSections[i].Vel = velocity;
                _ropeSections[i].Pos += velocity * Time.deltaTime;
            }
            
            ApplySlackToRope();

            // Update the line renderer with the new rope positions
            for (var i = 0; i < _numSections; i++)
            {
                _lineRenderer.SetPosition(i, _ropeSections[i].Pos);
            }
        }

        private Vector3 GetAcceleration(int i)
        {
            // Calculate the acceleration due to gravity and elasticity
            var forceGravity = new Vector3(0f, -ropeGravity, 0f);
            var forceElastic = Vector3.zero;
            
            if (i > 0)
            {
                var diff = _ropeSections[i].Pos - _ropeSections[i - 1].Pos;
                var distance = diff.magnitude;
                var direction = diff.normalized;
                forceElastic += -ropeElasticity * (distance - ropeSegmentLength) * direction;
            }
            
            if (i < _numSections - 1)
            {
                var diff = _ropeSections[i].Pos - _ropeSections[i + 1].Pos;
                var distance = diff.magnitude;
                var direction = diff.normalized;
                forceElastic += -ropeElasticity * (distance - ropeSegmentLength) * direction;
            }

            return (forceGravity + forceElastic) / 1f;
        }

        private void ApplySlackToRope()
        {
            // Apply slack to the rope by adjusting the position of each section
            for (var i = 1; i < _numSections - 1; i++)
            {
                var direction = (_ropeSections[i - 1].Pos - _ropeSections[i].Pos).normalized;
                var distance = (_ropeSections[i - 1].Pos - _ropeSections[i].Pos).magnitude;

                if (distance < ropeSegmentLength)
                {
                    _ropeSections[i].Pos = _ropeSections[i - 1].Pos - direction * ropeSegmentLength;
                }
            }
        }
    }
}