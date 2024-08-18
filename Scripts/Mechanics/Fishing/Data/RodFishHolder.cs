using System.Collections.Generic;
using Interactable.Carriable;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Fishing.Data
{
    [CreateAssetMenu(fileName = "NewRodFishHolder", menuName = "Rod Fish Holder")]
    public class RodFishHolder : ScriptableObject
    {
        [field: SerializeField] public float SmallestWaitTime { get; private set; } = 3f;
        [field: SerializeField] public int NumberOfPoints { get; private set; } = 6;
        [field: SerializeField] public float MovementRadius { get; private set; } = 5f;
        
        [InlineEditor] public List<PhysicalFish> fishDataList;
        
    }
}
