using UnityEngine;

namespace Interactable.Carriable.Tool
{
    public interface ITool : ICarriable
    {
        /// <summary>
        /// Item is picked up and Player uses it
        /// </summary>
        void PerformAction();
        int GetUseActionHash();
        //If Player has no StopUse Action (e.g. Axe or Hammer), return 0
        int GetStopActionHash();
        //If Player has no Minigame Action (e.g. Axe or Hammer), return 0
        int GetWonMinigameActionHash();
        // Initial position where the Item is returned to on drop
        Vector3 InitialPosition { get; }
    }
}