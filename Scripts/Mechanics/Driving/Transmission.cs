using UnityEngine;

namespace Mechanics.Driving {
    public class Transmission
    {
        private int CurrentGear { get; set; }
        private readonly int _maxGears;
        private readonly float _minSpeedPerGear;
        private readonly float _maxSpeed;
        private readonly AudioClip _shiftSound;
        private readonly AudioClip _driveSound;
        private readonly AudioSource _audioSource;

        public Transmission(int maxGears, float minSpeedPerGear, float maxSpeedPerGear, AudioClip shiftSound, AudioClip driveSound, AudioSource audioSource, float maxSpeed) {
            _maxGears = maxGears;
            _minSpeedPerGear = minSpeedPerGear;
            _shiftSound = shiftSound;
            _driveSound = driveSound;
            _audioSource = audioSource;
            CurrentGear = 1;
            _maxSpeed = maxSpeed;
        }

        public void ShiftGears(float speed) {
            if (speed > _maxSpeed) {
                speed = _maxSpeed;
            }
            float upshiftThreshold = CurrentGear * _minSpeedPerGear * 2 * 1.2f; // 20% buffer for upshifting
            float downshiftThreshold = (CurrentGear - 1) * _minSpeedPerGear * 2 * 0.8f; // 20% buffer for downshifting
            if (speed > upshiftThreshold && CurrentGear < _maxGears) {
                CurrentGear++;
                _audioSource.PlayOneShot(_shiftSound, 0.25f);
            }
            else if (speed < downshiftThreshold && CurrentGear > 1) {
                CurrentGear--;
                _audioSource.PlayOneShot(_shiftSound, 0.25f);
            }
        }

        public void AdjustEngineSound(float speed) {
            _audioSource.clip = _driveSound;
            _audioSource.pitch = Mathf.Lerp(0.5f, 1.5f, (speed - (CurrentGear - 1) * _minSpeedPerGear * 2) / (CurrentGear * _minSpeedPerGear * 2));
            if (!_audioSource.isPlaying) {
                _audioSource.Play();
            }
        }
    }
}