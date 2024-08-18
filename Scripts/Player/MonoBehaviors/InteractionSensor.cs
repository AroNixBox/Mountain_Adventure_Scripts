using System.Collections.Generic;
using Interactable;
using Interactable.Carriable;
using Mechanics.Gardening;
using UnityEngine;

namespace Player.MonoBehaviors
{
    public class InteractionSensor : MonoBehaviour
    {
        // Define variables for interaction range, text, and input
        [SerializeField] private float interactRange = 0.75f;
        [SerializeField] private float yOffset = 0f; // Add yOffset here
        [SerializeField] private SuperTextMesh interactText;
        [SerializeField] private string mouseInteractInput = "E";
        [SerializeField] private string controllerInteractInput = "Button West";

        // Constants for text effects
        private const string WobblyEffect = "<w=default>";
        private const string EndWobblyEffect = "</w>";
        private string _deviceInput;

        // Method to get the closest interactable object
        public IInteractable GetClosestInteractable() {
            // Define variables for colliders and interactables
            Collider[] collidersInRange = new Collider[10];
            Vector3 sphereCastOrigin = transform.position + transform.forward * interactRange + new Vector3(0, yOffset, 0); // Add yOffset to the sphereCast origin
            int collidersInRangeCount = Physics.OverlapSphereNonAlloc(sphereCastOrigin, interactRange, collidersInRange);
            var pickableTransforms = new List<IInteractable>();

            // Loop through colliders and add interactables to list
            for (int i = 0; i < collidersInRangeCount; i++) {
                var col = collidersInRange[i];
                if (col != null && col.TryGetComponent<IInteractable>(out var interactable)) {
                    pickableTransforms.Add(interactable);
                }
            }

            // Find the closest interactable object
            IInteractable closestInteractable = null;
            foreach (var obj in pickableTransforms) {
                if (closestInteractable == null) {
                    closestInteractable = obj; 
                }else {
                    var closestDistance = (sphereCastOrigin - closestInteractable.GetPosition()).sqrMagnitude;
                    var currentDistance = (sphereCastOrigin - obj.GetPosition()).sqrMagnitude;
                    if (currentDistance < closestDistance)
                    {
                        closestInteractable = obj;
                    }
                }
            }

            // Update the interact text based on the closest interactable
            if(closestInteractable != null) {
                var interactableName = closestInteractable switch {
                    PhysicalFish physicalFish => physicalFish.FishName,
                    GrownPlant plant => plant.PlantName,
                    DiggableObstacle diggable => diggable.DiggableName,
                    _ => closestInteractable.GetType().Name
                };

                UpdateInteractText(interactableName);
                return closestInteractable;
            }

            UpdateInteractText(string.Empty);
            return null;
        }

        // Method to update the interact text
        private void UpdateInteractText(string newInteractText) {
            string newFullText;

            newFullText = newInteractText != string.Empty ? $"Press {WobblyEffect}{_deviceInput}{EndWobblyEffect} to interact with {newInteractText}" : "";

            if (newFullText != GetInteractionText()) {
                interactText.text = newFullText;
            }
        }

        // Method to get the current interaction text
        private string GetInteractionText() {
            return interactText.text;
        }

        // Event listeners for UI updates and input device changes
        private void OnEnable() {
            UIEvents.PassTextToUI.Get("Interact").AddListener(UpdateInteractText);
            UIEvents.InputDeviceChanged.Get().AddListener(IsControllerInputDevice);
        }

        // Method to check if the input device is a controller
        private void IsControllerInputDevice(bool arg0) {
            _deviceInput = arg0 ? controllerInteractInput : mouseInteractInput;
        }

        // Remove event listeners when the object is destroyed
        private void OnDestroy() {
            UIEvents.PassTextToUI.Get("Interact").RemoveListener(UpdateInteractText);
            UIEvents.InputDeviceChanged.Get().RemoveListener(IsControllerInputDevice);
        }
    }
}