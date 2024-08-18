using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

namespace NonInteractable
{
    public class WanderingNPC : MonoBehaviour
    {
        [SerializeField] private SplineAnimate splineAnimate;
        [SerializeField] private float maxTimeToWait = 25f;
        private Animator _animator;
        private float MinTimeToWait => maxTimeToWait / 5;
        private DateTime _time;
        private bool _isAnimationStopped;

        private readonly int ANIM_DOANIM = Animator.StringToHash("DoAnim");
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _time = DateTime.Now.AddSeconds(UnityEngine.Random.Range(MinTimeToWait, maxTimeToWait));

            if (splineAnimate == null)
            {
                splineAnimate = GetComponentInParent<SplineAnimate>();
            }
        }

        private IEnumerator Start()
        {
            yield return null;
            splineAnimate.Play();
        }

        private void StartAnimation()
        {
            //Start Countdown to call StopAnimation
            splineAnimate.Play();
            _isAnimationStopped = false;
        }

        private void Update()
        {
            if (!_isAnimationStopped && DateTime.Now >= _time)
            {
                _isAnimationStopped = true;
                StopAnimation();
            }
        }

        private void StopAnimation()
        {
            _animator.SetTrigger(ANIM_DOANIM);
            splineAnimate.Pause();
        }

        private void OnAnimEnded(AnimationEvent animationEvent)
        {
            StartAnimation();
            _time = DateTime.Now.AddSeconds(UnityEngine.Random.Range(MinTimeToWait, maxTimeToWait));
        }
    }
}
