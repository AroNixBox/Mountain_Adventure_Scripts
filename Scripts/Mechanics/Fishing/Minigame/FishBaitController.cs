using System;
using Cinemachine;
using UnityEngine;

namespace Mechanics.Fishing.Minigame
{
    public class FishBaitController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CinemachineVirtualCamera baitControllerCamera;
    
        [Header("Values")]
        [SerializeField] private int cameraActivePriority = 20;
        
        private const int CameraInactivePriority = 0;
        private Rigidbody _rigidbody;
        private bool _shouldReadCollision;
        private Action _fishingFailed;

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Sets the bait position to the fishing rod, switches the active camera to the MiniGame Camera
        /// and the callback for when the fishing fails
        /// </summary>
        /// <param name="baitPosition"></param>
        /// <param name="fishingFailedCallback"></param>
        public void Init(Vector3 baitPosition, Action fishingFailedCallback)
        {
            gameObject.SetActive(true);
            //Set the parents position to the bait position
            transform.parent.position = baitPosition;
            transform.localPosition = Vector3.zero;
            baitControllerCamera.Priority = cameraActivePriority;
            _fishingFailed = fishingFailedCallback;
            _shouldReadCollision = true;
        }
        
        /// <summary>
        /// Cancels the Possession of the Bait, resets the position and disables the camera
        /// </summary>
        public void Abort() {
            //Reset the position relative to the parent
            transform.localPosition = Vector3.zero;
            baitControllerCamera.Priority = CameraInactivePriority;
            _shouldReadCollision = false;
            gameObject.SetActive(false);
        }

        //Observing the MiniGame
        private void OnTriggerExit(Collider other)
        {
            if(!_shouldReadCollision) { return; }
            if (!other.transform.TryGetComponent(out Hook hook)) return;
        
            //Player has failed the MiniGame
            _fishingFailed?.Invoke();
            Abort();
        }
    }
}
