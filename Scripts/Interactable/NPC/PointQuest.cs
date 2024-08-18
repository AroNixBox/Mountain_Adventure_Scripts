using System.Data;
using Interactable.Carriable;

namespace Interactable.NPC
{
    public class PointQuest : Quest
    {
        private readonly int _totalPoints;

        public PointQuest(int totalPoints)
        {
            _totalPoints = totalPoints;
        }

        //Remove the Points on the Player later on
        public override int GetRewardPoints(bool pointQuest = false)
        {
            return -_totalPoints;
        }

        public override bool IsQuestDone(int currentPoints = 0)
        {
            return currentPoints >= _totalPoints;
        }
        
        //Stay Empty!!!
        public override void AddItem(ICarriable broughtItem)
        {
            // NOOP, if there is an item in the collider, this will still be called. but stay empty
        }
        //Stay Empty!!!
        public override void RemoveItem(ICarriable broughtItem)
        {
            // NOOP, if there is an item in the collider, this will still be called. but stay empty

        }
    }
}