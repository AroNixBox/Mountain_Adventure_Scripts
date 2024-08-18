using System;
using Extensions.FSM;
using Movement.Player.States;
using Player.MonoBehaviors;
using UnityEngine;

namespace Interactable.NPC.States
{
    public class TalkState : IState, IDisposable
    {
        private NPCReferenceManager _npcReferenceManager;
        private SuperTextMesh _dialogueTMP;
        private AudioSource _audioSource;
        private DialogueSO _dialogueSo;
        private int _index;
        
        public TalkState(NPCReferenceManager npcReferenceManager)
        {
            _npcReferenceManager = npcReferenceManager;
            _dialogueSo = _npcReferenceManager.DialogueSO;
            _audioSource = _npcReferenceManager.AudioSource;
            _dialogueTMP = _npcReferenceManager.DialogueTMP;
            
            _npcReferenceManager.DialogueTMP.onCompleteEvent.AddListener(SetFinishedWritingTextOnScreen);
        }
        public void OnEnter()
        {
            //Reset Talk input Flag
            _npcReferenceManager.Talk = false;
            
            //Reset the flag that checks if the text has finished printing to screen
            _npcReferenceManager.HasFinishedPrintingTextToScreen = false;
            
            //Empty the text on screen
            _dialogueTMP.text = string.Empty;

            TalkVisuals();
        }

        private void TalkVisuals()
        {
            //TODO: If is still talking and this function is called again, print out the entire sentence.
            AudioClip audioClipToPlay;
            string textToDisplay;
        
            if (_index < _dialogueSo.DialogueOutput.Count)
            {
                textToDisplay = _dialogueSo.DialogueOutput[_index].Text;
                audioClipToPlay = _dialogueSo.DialogueOutput[_index].AudioClip;
                _index++;
                
                _npcReferenceManager.CurrentDialogueState = DialogueState.Talk;
            }
            else
            {
                //If this is the last sentence, set isDialogueDone to true, to break out of the dialogue
                if (_npcReferenceManager.Quest.IsQuestDone(PlayerRewardHandler.GetPoints()) || _npcReferenceManager.HasRewardedPlayer)
                {
                    audioClipToPlay = _dialogueSo.MissionFinishedOutput.AudioClip;
                    textToDisplay = _dialogueSo.MissionFinishedOutput.Text;
                    
                    if(_npcReferenceManager.HasRewardedPlayer)
                    {
                        _npcReferenceManager.CurrentDialogueState = DialogueState.QuestFinished;
                    }
                    else
                    {
                        switch (_npcReferenceManager.Quest)
                        {
                            case PointQuest:
                                _npcReferenceManager.CurrentDialogueState = DialogueState.LookAtEvent;
                                break;
                            case PhysicalCarryQuest: 
                                _npcReferenceManager.CurrentDialogueState = DialogueState.RewardPlayer;
                                break;
                        }

                    }
                }
                else
                {
                    audioClipToPlay = _dialogueSo.MissionDescriptionOutput.AudioClip;
                    textToDisplay = _dialogueSo.MissionDescriptionOutput.Text;
                    
                    _npcReferenceManager.CurrentDialogueState = DialogueState.DialogueFinished;
                }
            }
            
            var entireText = "<audioClips=casual>" + textToDisplay;
            _dialogueTMP.gameObject.SetActive(true);

            //TODO: On Read Complete get outta this State
            _dialogueTMP.Text = entireText;
            _dialogueTMP.Read();

            _audioSource.PlayOneShot(audioClipToPlay);
        }
        public void Tick()
        {

        }

        public void OnExit()
        {
            
        }

        public Color GizmoState()
        {
            return new Color(1f, 0.8f, 0f);
        }

        private void SetFinishedWritingTextOnScreen()
        {
            if(_dialogueTMP.text == string.Empty) { return; }
            _npcReferenceManager.HasFinishedPrintingTextToScreen = true;
        }

        public void Dispose()
        {
            _npcReferenceManager.DialogueTMP.onCompleteEvent.RemoveListener(SetFinishedWritingTextOnScreen);
        }
    }

    public enum DialogueState
    {
        None,
        Talk,
        DialogueFinished,
        QuestFinished,
        RewardPlayer,
        LookAtEvent
    }
}
