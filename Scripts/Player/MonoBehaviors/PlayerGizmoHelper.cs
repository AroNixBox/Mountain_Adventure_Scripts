using InputSystem;
using UnityEngine;

namespace Player.MonoBehaviors
{
    public class PlayerGizmoHelper : MonoBehaviour
    {
        private PlayerReferenceManager _playerReferenceManager;
        private StarterAssetsInputs _input;
        private Camera _mainCamera;

        private void Awake()
        {
            _playerReferenceManager = GetComponent<PlayerReferenceManager>();
        }

        private void Start()
        {
            _input = _playerReferenceManager.Input;
            _mainCamera = _playerReferenceManager.MainCamera;
        }

        private void Update()
        {
            DrawMovementDirection();
        }

        private void DrawMovementDirection()
        {
            if (_input.move != Vector2.zero)
            {
                // Create a direction vector from the input on x and y axis to get the direction Vector, normalize it to get a unit vector
                Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
    
                // Get the angle of the input direction in radians, convert it to degrees and add the camera's y rotation
                float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                       _mainCamera.transform.eulerAngles.y;
    
                // Get the direction the player should move in
                Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
                // Get the start and end position of the line
                Vector3 startPosition = transform.position + transform.up * 0.5f;
                Vector3 endPosition = startPosition + targetDirection * 10.0f;
    
                // Convert the subtraction of both positions to a direction vector
                Vector3 directionToTarget = (endPosition - transform.position).normalized;
                // Get the angle between the player's forward direction and the direction to the target
                float angle = Vector3.Angle(transform.forward, directionToTarget);

                // Get the cross product of the player's forward direction and the direction to the target
                // to determine if the angle is positive or negative
                Vector3 crossProduct = Vector3.Cross(transform.forward, directionToTarget);
    
                // If the y value of the cross product is negative, the angle is negative
                if (crossProduct.y < 0)
                {
                    angle = -angle;
                }
    
                Debug.DrawLine(startPosition, endPosition, Color.blue);
            }
        }
    }
}
