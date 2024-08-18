using Extensions.FSM;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States.Talk
{
    public class RewardState : IState
    {
        private readonly PlayerReferenceManager _playerReferences;
        private readonly AudioSource _audioSource;
        private readonly Animator _animator;
        private readonly CharacterController _controller;
        private readonly GravityHandler _gravityHandler;
        private AudioClip[] _rewardClips;
        private readonly int _rewardAnimationHash;

        public RewardState(PlayerReferenceManager playerReferences, int rewardAnimationHash, AudioClip[] rewardClips)
        {
            _playerReferences = playerReferences;
            _animator = playerReferences.Animator;
            _controller = playerReferences.Controller;
            _gravityHandler = playerReferences.GravityHandler;
            _rewardAnimationHash = rewardAnimationHash;
            _rewardClips = rewardClips;
            _audioSource = playerReferences.AudioSource;
        }
        public void OnEnter()
        {
            _animator.SetTrigger(_rewardAnimationHash);
            _audioSource.PlayOneShot(_rewardClips[Random.Range(0, _rewardClips.Length)]);
        }

        public void Tick()
        {
            _gravityHandler.HandleGravity();
            _controller.Move(new Vector3(0, _playerReferences.VerticalVelocity, 0) * Time.deltaTime);
        }

        public void OnExit()
        {
            _playerReferences.IsPlayerTalkAnimationDone = false;
            _playerReferences.CurrentNpc = null;
        }

        public Color GizmoState()
        {
            return new Color(1f, 0.647f, 0f);
        }
    }
}
