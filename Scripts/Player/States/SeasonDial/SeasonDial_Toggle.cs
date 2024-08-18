using Extensions.FSM;
using Player.MonoBehaviors;
using UnityEngine;

namespace Player.States.SeasonDial
{
    /// <summary>
    /// Transition to this State only when the SeasonDial is pulled out!
    /// </summary>
    public class SeasonDial_Toggle : IState
    {
        private readonly PlayerReferenceManager _playerReferences;

        public SeasonDial_Toggle(PlayerReferenceManager playerReferences)
        {
            _playerReferences = playerReferences;
        }
        public void OnEnter()
        {
            _playerReferences.PulledOutDial = false;
            _playerReferences.DialCamera.Priority = _playerReferences.CinemachineActivePriority;
        }

        public void Tick()
        {
            //If the Dial is Rotating already, return
            if (_playerReferences.PhysicalSeasonCompass.IsRotating) return;

            var scrollInput = _playerReferences.Input.scroll;

            if (scrollInput < 0)
            {
                IncreaseSeason();
                _playerReferences.PhysicalSeasonCompass.RotateObject(-90f);
            }
            else if (scrollInput > 0)
            {
                DecreaseSeason();
                _playerReferences.PhysicalSeasonCompass.RotateObject(90f);
            }
            
            _playerReferences.Input.scroll = 0;
            
            //Apply Gravity
            _playerReferences.GravityHandler.HandleGravity();
            _playerReferences.Controller.Move(new Vector3(0, _playerReferences.VerticalVelocity, 0) * Time.deltaTime);
        }

        public void OnExit()
        {
            _playerReferences.DialCamera.Priority = _playerReferences.CameraInactivePriority;
        }
        private void IncreaseSeason()
        {
            _playerReferences.PhysicalSelectedSeason++;
            if (_playerReferences.PhysicalSelectedSeason > 3)
            {
                _playerReferences.PhysicalSelectedSeason = 0;
            }
        }

        private void DecreaseSeason()
        {
            _playerReferences.PhysicalSelectedSeason--;
            if (_playerReferences.PhysicalSelectedSeason < 0)
            {
                _playerReferences.PhysicalSelectedSeason = 3;
            }
        }
        public bool IsDifferentSeasonLockedIn()
        {
            return _playerReferences.PhysicalSelectedSeason != _playerReferences.SeasonSwitchManager.CurrentSeason;
        }

        public Color GizmoState()
        {
            return new Color(0.529f, 0.808f, 0.922f);
        }
    }
}
