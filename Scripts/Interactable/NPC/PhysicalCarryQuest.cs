using System.Collections.Generic;
using Interactable.Carriable;
using Mechanics.Gardening;
using UnityEngine;

namespace Interactable.NPC
{
    public class PhysicalCarryQuest : Quest
    {
        private string _carriableName;
        private List<ICarriable> _collectedItems = new ();
        private readonly int _totalItems;
        private readonly int _rewardPoints;
        private bool questAccomplished;

        // Constructor initializes the quest with an item, total items, and reward points.
        // If you want to add another item that has a specific name int he class, you need to add a case for it.
        public PhysicalCarryQuest(ICarriable item, int totalItems, int rewardPoints)
        {
            _totalItems = totalItems;
            _rewardPoints = rewardPoints;

            _carriableName = item switch {
                PhysicalFish fish => fish.FishName,
                GrownPlant plant => plant.PlantName,
                DiggableObstacle diggable => diggable.DiggableName,
                _ => item.GetType().Name
            };
        }

        // Adds an item to the quest if it matches the quest's item
        // If you want to add another item that has a specific name in the class, you need to add a case for it.
        public override void AddItem(ICarriable broughtItem) {
            if(questAccomplished) {return;} // Because we only have one quest per npc, deactivate the Sensor if the quest is done.
            
            string broughtItemName = broughtItem switch {
                PhysicalFish fish => fish.FishName,
                GrownPlant plant => plant.PlantName,
                DiggableObstacle diggable => diggable.DiggableName,
                _ => broughtItem.GetType().Name
            };

            if (broughtItemName == _carriableName) {
                _collectedItems.Add(broughtItem);
            }
        }

        // Removes an item from the quest if it matches the quest's item
        // If you want to add another item that has a specific name in the class, you need to add a case for it.
        
        public override void RemoveItem(ICarriable broughtItem) {
            if(questAccomplished) {return;} // Because we only have one quest per npc, deactivate the Sensor if the quest is done.
            
            string broughtItemName = broughtItem switch {
                PhysicalFish fish => fish.FishName,
                GrownPlant plant => plant.PlantName,
                DiggableObstacle diggable => diggable.DiggableName,
                _ => broughtItem.GetType().Name
            };

            if (broughtItemName == _carriableName) {
                _collectedItems.Remove(broughtItem);
            }
        }
        
        // Returns the reward points for the quest
        public override int GetRewardPoints(bool pointQuest = false) {
            if (pointQuest) return _rewardPoints;

            // Destroy the items
            // We know we have enough Items in the List so we can destroy them
            for(int i = _collectedItems.Count - 1; i >= 0; i--) { // Start from the end of the list to avoid index out of range exceptions, because we are removing items from the list in runtime and then still requesting
                _collectedItems[i].Despawn();

                // Check if the item still exists in the list before removing it
                if (_collectedItems.Contains(_collectedItems[i])) {
                    _collectedItems.RemoveAt(i); // Remove the item at the current index
                }
            }
            // Doublecheck if we correctly removed them from the List

            return _rewardPoints;
        }

        // Checks if the quest is done by comparing the collected items with the total items
        public override bool IsQuestDone(int currrentPoints = 0) {
            //Ignore the currentPoints
            if (_collectedItems.Count >= _totalItems) {
                questAccomplished = true;
                return true;
            }

            return false;
        }
    }
}