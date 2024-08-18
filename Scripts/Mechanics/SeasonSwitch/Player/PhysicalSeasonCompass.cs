using System.Collections;
using InputSystem;
using UnityEngine;

namespace Mechanics.SeasonSwitch.Player
{
    public class PhysicalSeasonCompass : MonoBehaviour
    {
        [SerializeField] private StarterAssetsInputs input;
        public int PhysicalSelectedSeason { get; private set; }
        public bool IsRotating { get; private set; }
        private bool _canReadInput;

        [SerializeField] private Transform objectToRotate;
        [SerializeField] private float rotationDuration = 0.5f;

        private float _targetRotationY;
        private Coroutine _rotationCoroutine;
        
        public void RotateObject(float angle)
        {
            if (objectToRotate == null || IsRotating) return;

            _targetRotationY = objectToRotate.localEulerAngles.y + angle;

            if (_rotationCoroutine != null)
            {
                StopCoroutine(_rotationCoroutine);
            }

            _rotationCoroutine = StartCoroutine(RotateObjectSmoothly());
        }

        private IEnumerator RotateObjectSmoothly()
        {
            IsRotating = true;
            var elapsedTime = 0f;
            var startRotationY = objectToRotate.localEulerAngles.y;

            while (elapsedTime < rotationDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsedTime / rotationDuration);
                float currentRotationY = Mathf.Lerp(startRotationY, _targetRotationY, t);

                Vector3 currentRotation = objectToRotate.localEulerAngles;
                currentRotation.y = currentRotationY;
                objectToRotate.localEulerAngles = currentRotation;

                yield return null;
            }

            var finalRotation = objectToRotate.localEulerAngles;
            finalRotation.y = _targetRotationY;
            objectToRotate.localEulerAngles = finalRotation;

            IsRotating = false;
        }
    }
}