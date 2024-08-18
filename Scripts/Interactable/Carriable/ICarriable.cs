using System;
using System.Collections;
using UnityEngine;

namespace Interactable.Carriable
{
    public interface ICarriable : IInteractable
    {
        /// <summary>
        /// Determines if the item can be dropped
        /// </summary>
        /// <returns></returns>
        bool CanPerformDrop();

        /// <summary>
        /// Drop Logic.
        /// IMPORTANT: call onDropFinished when the drop is finished!!
        /// </summary>
        /// <param name="onDropFinished"></param>
        /// <returns></returns>
        IEnumerator Drop(Action onDropFinished);
        Vector3 EquipCoordinates { get; set; }
        Vector3 EquipRotation { get; }
        void Despawn();
    }
}