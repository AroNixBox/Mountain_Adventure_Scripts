using System;
using System.Collections;
using UnityEngine;

namespace Interactable
{
    public interface IInteractable
    {
        /// <summary>
        /// Item is on the ground and Player can interact with it
        /// </summary>
        /// <param name="position"></param>
        /// <param name="onInteractFinished"></param>
        IEnumerator Interact(Transform position, Action onInteractFinished);
        /// <summary>
        /// Returns the position of the Item to figure out which one is the closest item to the Player
        /// </summary>
        /// <returns></returns>
        Vector3 GetPosition();
    }
}


