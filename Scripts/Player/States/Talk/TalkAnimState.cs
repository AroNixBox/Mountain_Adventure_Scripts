using Extensions.FSM;
using Interactable.NPC;
using Interactable.NPC.States;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States.Talk
{
    public class TalkAnimState : IState
    {
        private readonly PlayerReferenceManager _playerReferences;
        private readonly AudioSource _audioSource;
        private readonly Animator _animator;
        private readonly CharacterController _controller;
        private readonly GravityHandler _gravityHandler;
        private AudioClip[] _playerClips;
        private readonly int _talkAnimationHash;
        private bool _isLastAnim;

        public TalkAnimState(PlayerReferenceManager playerReferences, int talkAnimationHash, AudioClip[] playerClips, bool isLastAnim = false)
        {
            _playerReferences = playerReferences;
            _animator = playerReferences.Animator;
            _controller = playerReferences.Controller;
            _gravityHandler = playerReferences.GravityHandler;
            _talkAnimationHash = talkAnimationHash;
            _playerClips = playerClips;
            _audioSource = playerReferences.AudioSource;
            _isLastAnim = isLastAnim;
        }
        public void OnEnter()
        {
            _animator.SetTrigger(_talkAnimationHash);
            _audioSource.PlayOneShot(_playerClips[Random.Range(0, _playerClips.Length)]);
        }

        public void Tick()
        {
            _gravityHandler.HandleGravity();
            _controller.Move(new Vector3(0, _playerReferences.VerticalVelocity, 0) * Time.deltaTime);
        }

        public void OnExit()
        {
            _playerReferences.IsPlayerTalkAnimationDone = false;
            
            if (_playerReferences.CurrentNpc.GetDialogueState() is DialogueState.DialogueFinished or DialogueState.QuestFinished)
            {
                _playerReferences.CurrentNpc.Talk();
            }
            
            if(_isLastAnim)
            {
                _playerReferences.CurrentNpc = null;
            }
        }

        public Color GizmoState()
        {
            return new Color(1f, 0.647f, 0f);
        }
    }
}
