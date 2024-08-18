using System;
using System.Collections.Generic;
using Interactable.Carriable;
using Interactable.NPC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Movement.Player.States
{
    [CreateAssetMenu(fileName = "DialogueSO", menuName = "Dialogue/DialogueSO")]
    public class DialogueSO : ScriptableObject
    {
        [field: SerializeField] public List<StringAudioClipPair> DialogueOutput { get; private set; }= new ();
        [field: SerializeField] public StringAudioClipPair MissionDescriptionOutput { get; private set; }
        [field: SerializeField] public StringAudioClipPair MissionFinishedOutput { get; private set; }
        
        [HorizontalGroup("QuestType")]
        [HideLabel, EnumToggleButtons]
        public QuestType SelectedQuestType;

        [ShowIf("SelectedQuestType", QuestType.PhysicalQuest)]
        public GameObject QuestItem;
        [ShowIf("SelectedQuestType", QuestType.PhysicalQuest)]
        public int TotalNeededItems;

        [ShowIf("SelectedQuestType", QuestType.PhysicalQuest)]
        public int RewardPoints;

        [ShowIf("SelectedQuestType", QuestType.PointQuest)]
        public int TotalNeededPoints;
        [ShowIf("SelectedQuestType", QuestType.PointQuest)]
        public AudioClip NpcEventSound;

        public Quest GetQuest()
        {
            switch (SelectedQuestType)
            {
                case QuestType.PhysicalQuest:
                    if (QuestItem == null)
                    {
                        Debug.LogError("QuestItem is null for PhysicalQuest!");
                        return null;
                    }

                    var components = QuestItem.GetComponents<MonoBehaviour>();
                    foreach (var component in components)
                    {
                        if (component is ICarriable carriable)
                        {
                            return new PhysicalCarryQuest(carriable, TotalNeededItems, RewardPoints);
                        }
                    }

                    Debug.LogError("No component on the QuestItem implements ICarriable!");
                    return null;

                case QuestType.PointQuest:
                    return new PointQuest(TotalNeededPoints);

                default:
                    return null;
            }
        }
    }
    
    [Serializable]
    public class StringAudioClipPair
    {
        [TextArea]
        public string Text;
        public AudioClip AudioClip;
    }
    public enum QuestType
    {
        PhysicalQuest,
        PointQuest
    }
}
