using Extensions.FSM;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States.SeasonDial
{
    public class SeasonDial_SeasonChange : IState
    {
        private readonly PlayerReferenceManager _playerReferences;
        private readonly Animator _animator;
        private readonly CharacterController _controller;
        private AudioSource _audioSource;
        
        private float _elapsedTime = 0f;
        private const float LerpDuration = .8f;
        private const float LerpHeight = 0.5f;
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private bool _isMovingUp = true;
    
        public SeasonDial_SeasonChange(PlayerReferenceManager playerReferences)
        {
            _playerReferences = playerReferences;
            _animator = _playerReferences.Animator;
            _controller = _playerReferences.Controller;
            _audioSource = _playerReferences.AudioSource;
        }
    
        public void OnEnter()
        {
            _animator.SetBool(_playerReferences.ANIM_RITUAL, true);
            
            _elapsedTime = 0f;
            _startPosition = _controller.transform.position;
            _targetPosition = _startPosition + Vector3.up * LerpHeight;
            _isMovingUp = true;
            
            _audioSource.PlayOneShot(_playerReferences.RitualSounds[Random.Range(0, _playerReferences.RitualSounds.Length)]);
        }
    
        public void Tick()
        {
            if (_elapsedTime < LerpDuration)
            {
                MoveCharacter();
                _elapsedTime += Time.deltaTime;
    
                //Because the current Ritual is a Pose not an Animation, we want to stop the animation halfway through the Lerp
                if (IsHalfwayThroughLerp() && _isMovingUp)
                {
                    _animator.SetBool(_playerReferences.ANIM_RITUAL, false);
                }
            }
            else
            {
                _controller.Move(_targetPosition - _controller.transform.position);
    
                if (_isMovingUp)
                {
                    StartMovingDown();
                }
            }
        }
    
        private void MoveCharacter()
        {
            var newPosition = Vector3.Lerp(_startPosition, _targetPosition, _elapsedTime / LerpDuration);
            _controller.Move(newPosition - _controller.transform.position + Vector3.up * 0.001f);
        }
    
        private bool IsHalfwayThroughLerp()
        {
            return _elapsedTime >= LerpDuration / 2f;
        }
    
        private void StartMovingDown()
        {
            _elapsedTime = 0f;
            _startPosition = _controller.transform.position;
            _targetPosition = _startPosition - Vector3.up * LerpHeight;
            _isMovingUp = false;
        }
    
        public void OnExit()
        {
            _playerReferences.SeasonSwitchManager.SwitchSeason(_playerReferences.PhysicalSelectedSeason);
            _playerReferences.IsRitualFinished = false;
        }
    
        public Color GizmoState()
        {
            return new Color(0f, 0f, 0.545f);
        }
    }
}