using Interactable.Carriable;

namespace Interactable.NPC
{
    public abstract class Quest
    {
        /// <summary>
        /// The Item that the Player needs to bring, if its a Pointquest it will be empty
        /// </summary>
        /// <param name="broughtItem"></param>
        public abstract void AddItem(ICarriable broughtItem);
        /// <summary>
        /// The Item that the Player needs to bring, if its a Pointquest it will be empty
        /// </summary>
        /// <param name="broughtItem"></param>
        public abstract void RemoveItem(ICarriable broughtItem);
        /// <summary>
        /// The Reward Points that gets added if the Quest is a Physicalquest, if Pointquest RewardPoints will be subtracted
        /// </summary>
        /// <returns></returns>
        public abstract int GetRewardPoints(bool pointQuest = false);
        /// <summary>
        /// Checks if the Quest is done, if its a Physicalquest it will check if the Player has brought all Items (so currentPoints should stay 0!,
        /// if its a Pointquest it will check if the Player has enough Points
        /// </summary>
        /// <param name="currentPoints"></param>
        /// <returns></returns>
        public abstract bool IsQuestDone(int currentPoints = 0);
    }
}