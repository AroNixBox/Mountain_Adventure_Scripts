using System;
using Extensions.FSM;
using UnityEngine;

namespace Interactable.NPC.States
{
    public class LookAtEventState : IState
    {
        private readonly AudioSource _audioSource;
        private readonly Animator _animator;
        private NPCReferenceManager _npcReferenceManager;
        private AudioClip _clipToPlay;
        private readonly int _lookAtEventAnim;
        private float _waitTime;
        private DateTime _time;
        private bool _canExit;
        private bool _eventTriggered;

        public LookAtEventState(NPCReferenceManager npcReferences, int lookAtEventAnim, AudioClip clipToPlay, float waitTime)
        {
            _animator = npcReferences.Animator;
            _lookAtEventAnim = lookAtEventAnim;
            _clipToPlay = clipToPlay;
            _audioSource = npcReferences.AudioSource;
            _npcReferenceManager = npcReferences;
            _waitTime = waitTime;
        }
        public void OnEnter()
        {
            _canExit = false;
            _animator.SetTrigger(_lookAtEventAnim);
            
            _npcReferenceManager.OnPointQuestFinished.Invoke();
        
            _time = DateTime.Now.AddSeconds(_waitTime);
            _eventTriggered = false;
        }

        public void Tick()
        {
            if (!_eventTriggered && DateTime.Now >= _time.AddSeconds(-_waitTime * 2 / 3))
            {
                _eventTriggered = true;
                _audioSource.PlayOneShot(_clipToPlay);
            }

            if (DateTime.Now >= _time)
            {
                _canExit = true;
            }
        }

        public bool CanExit()
        {
            return _canExit;
        }

        public void OnExit()
        {
        }

        public Color GizmoState()
        {
            return new Color(0f, 1f, 0.89f);
        }
    }
}