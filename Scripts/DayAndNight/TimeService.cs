using System;
using Extensions;
using UnityEngine;

namespace DayAndNight
{
    public class TimeService
    {
        private readonly TimeSettings _timeSettings;
        private DateTime _currentTime;
        private readonly TimeSpan _sunriseTime;
        private readonly TimeSpan _sunsetTime;
        
        //Wrapper to publish an event when the value that is observed, changes.
        private readonly Observable<bool> _isDayTime;
        private readonly Observable<int> _currentHour;
        
        public event Action OnSunrise = delegate { };
        public event Action OnSunset = delegate { };
        public event Action OnHourChanged = delegate { };
        
        public TimeService(TimeSettings timeSettings)
        {
            //Initialize with TimeSettingsSO and set values
            _timeSettings = timeSettings;
            _currentTime = DateTime.Now.Date + TimeSpan.FromHours(timeSettings.startHour);
            _sunriseTime = TimeSpan.FromHours(timeSettings.sunriseHour);
            _sunsetTime = TimeSpan.FromHours(timeSettings.sunsetHour);

            //Hook up the Observers to their values
            _isDayTime = new Observable<bool>(IsDayTime());
            _currentHour = new Observable<int>(_currentTime.Hour);
            
            //If IsDayTime changes, check the observers value if is day => sunrise, else => sunset
            _isDayTime.ValueChanged += day => (day ? OnSunrise : OnSunset)?.Invoke();
            _currentHour.ValueChanged += _ => OnHourChanged?.Invoke();
        }
        //Constantly increment current time.
        public void UpdateTime(float deltaTime)
        {
            _currentTime = _currentTime.AddSeconds(deltaTime * _timeSettings.timeMultiplier);
            
            //Override the current hour value with the new hour value and day time value
            _isDayTime.Value = IsDayTime();
            _currentHour.Value = _currentTime.Hour;
        }

        public float CalculateSunAngle()
        {
            bool isDay = IsDayTime();
            //0 for straight down, 180 for straight up
            float startDegree = isDay ? 0 : 180;
            TimeSpan start = isDay ? _sunriseTime : _sunsetTime;
            TimeSpan end = isDay ? _sunsetTime : _sunriseTime;
            
            TimeSpan totalTime = CalculateDifference(start, end);
            //Calculate Diff between start and where we are now.
            TimeSpan elapsedTime = CalculateDifference(start, _currentTime.TimeOfDay);
            
            double percentage = elapsedTime.TotalMinutes / totalTime.TotalMinutes;
            return Mathf.Lerp(startDegree, startDegree + 180, (float) percentage);
        }
        
        public DateTime CurrentTime => _currentTime;
        
        //Does the Current time fall between Sunrise and Sunset? => Its Daytime, else its Night.
        private bool IsDayTime() => _currentTime.TimeOfDay > _sunriseTime && _currentTime.TimeOfDay < _sunsetTime;
        
        //Calculate the difference between two Timespans, tell how to close we are to sunrise and sunset
        private TimeSpan CalculateDifference(TimeSpan from, TimeSpan to)
        {
            TimeSpan difference = to - from;
            //If the result substraction is negative, add 24 hours to get the correct difference.
            return difference.TotalHours < 0 ? difference + TimeSpan.FromHours(24) : difference;
        }
    }
}