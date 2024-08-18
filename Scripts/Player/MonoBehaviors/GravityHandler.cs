using UnityEngine;

namespace Player.MonoBehaviors
{
    public class GravityHandler : MonoBehaviour
    {
        [field: SerializeField] public float Gravity { get; private set; } = -15.0f;
        [SerializeField] private float fallTimeOut = 0.15f;
        private float _fallTimeOutDelta;
        private const float TerminalVelocity = 53.0f;
    
        private PlayerReferenceManager _playerReferenceManager;
    
        //Offset of the Grounded SphereCastCheck
        [field: SerializeField] public float GroundedOffset { get; private set; } = -0f;
    
        //Radius of Grounded Check, Should match CharacterController
        [field: SerializeField] public float GroundedRadius { get; private set; } = 0.17f;
        [field: SerializeField] public LayerMask GroundLayers { get; private set; }
        public bool IsGrounded { get; private set; }
        //Time required to perform a jump again.
        public float JumpTimeoutDelta { get; set; }

        private void Awake()
        {
            _playerReferenceManager = GetComponent<PlayerReferenceManager>();
        }
        private void FixedUpdate()
        {
            IsGrounded = GroundedCheck();
        }

        private bool GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            return Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);
        }
        
        public void HandleGravity()
        {
            if(IsGrounded)
            {
                // reset the fall timeout timer
                _fallTimeOutDelta = fallTimeOut;
                
                // stop our velocity dropping infinitely when grounded
                if (_playerReferenceManager.VerticalVelocity < 0.0f)
                {
                    _playerReferenceManager.VerticalVelocity = -2f;
                } 
                
                if (JumpTimeoutDelta >= 0)
                {
                    JumpTimeoutDelta -= Time.deltaTime;
                }
            }
            

            // fall timeout
            if (_fallTimeOutDelta >= 0.0f)
            {
                _fallTimeOutDelta -= Time.deltaTime;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_playerReferenceManager.VerticalVelocity < TerminalVelocity)
            {
                _playerReferenceManager.VerticalVelocity += Gravity * Time.deltaTime;
            }
        }
        public bool FallTimeOut()
        {
            return _fallTimeOutDelta > 0.0f;
        }
        public bool CanJump()
        {
            return JumpTimeoutDelta <= 0;
        }
    }
}