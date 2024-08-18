using System;
using Interactable.Carriable;
using Player.MonoBehaviors;
using UnityEngine;

namespace Interactable.NPC
{
    public class NPCSensor : MonoBehaviour
    {
        [SerializeField] private NPCReferenceManager npc;
        
        private void Start() {
            if(npc == null) throw new NullReferenceException("NPCSensor: NPC is null!");

            //If the NPC has a PointQuest, disable the sensor
            if (npc.Quest is PointQuest) {
                gameObject.SetActive(false);
            }
        }
        private void OnTriggerEnter(Collider other) {
            if (!other.TryGetComponent(out ICarriable detectedCarriable)) return;
            
            npc.Quest.AddItem(detectedCarriable);
        }
        private void OnTriggerExit(Collider other) {
            if (!other.TryGetComponent(out ICarriable detectedCarriable)) return;
            
            npc.Quest.RemoveItem(detectedCarriable);
        }
    }
}

