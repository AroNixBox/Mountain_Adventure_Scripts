using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace DayAndNight
{
    public class TimeManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI timeText;
        [InlineEditor, SerializeField] private TimeSettings timeSettings;
        
        [SerializeField] private Light sun;
        [SerializeField] private Light moon;
        
        [SerializeField] private Volume volume;
        [SerializeField] private Material skyboxMaterial;

        [SerializeField] private RectTransform dial;
        
        [Header("Values")]
        [SerializeField] private float maxSunIntensity = 3f;
        [SerializeField] private float maxMoonIntensity = 1.5f;
        [SerializeField] private AnimationCurve lightIntensityCurve;

        [SerializeField] private Color dayAmbientLight;
        [SerializeField] private Color nightAmbientLight;

        private float _initialDialRotation;
        private ColorAdjustments _colorAdjustments;
        private TimeService _timeService;
        
        //Set how to subscribe and unsubscribe to the events
        public event Action OnSunrise
        {
            add => _timeService.OnSunrise += value;
            remove => _timeService.OnSunrise -= value;
        }
        public event Action OnSunset
        {
            add => _timeService.OnSunset += value;
            remove => _timeService.OnSunset -= value;
        }
        public event Action OnHourChanged
        {
            add => _timeService.OnHourChanged += value;
            remove => _timeService.OnHourChanged -= value;
        }
        
        private void Start()
        {
            //Create a new TimeService and provide it with the referenced SO
            _timeService = new TimeService(timeSettings);
            
            //get reference to the color adjustments in the volume
            volume.profile.TryGet(out _colorAdjustments);
            
            //Save the initial rotation of the dial
            _initialDialRotation = dial.rotation.eulerAngles.z;
            
            
            //Event Debugs
            // OnSunrise += () => Debug.Log("Sunrise");
            // OnSunset += () => Debug.Log("Sunset");
            // OnHourChanged += () => Debug.Log("Hour Changed");
        }
        
        private void Update()
        {
            UpdateTimeOfDay();
            RotateSun();
            UpdateLightSettings();
            UpdateSkyBlend();

            //Speed up or slow down time
            // if (Input.GetKeyDown(KeyCode.M))
            // {
            //     timeSettings.timeMultiplier *= 2f;
            // }
            // if (Input.GetKeyDown(KeyCode.N))
            // {
            //     timeSettings.timeMultiplier /= 2f;
            // }
        }

        private void UpdateSkyBlend()
        {
            //How far across the sky the sun has actually travelled
            float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);
            float blend = lightIntensityCurve.Evaluate(dotProduct);
            skyboxMaterial.SetFloat("_Blend", blend);
        }
        private void UpdateLightSettings()
        {
            //Calculate the dot product between the suns forward vector and the down vector
            //Dot product is 1 when the vectors are parallel, -1 when they are opposite
            float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);
            sun.intensity = Mathf.Lerp(0, maxSunIntensity, lightIntensityCurve.Evaluate(dotProduct));
            moon.intensity = Mathf.Lerp(0, maxMoonIntensity, lightIntensityCurve.Evaluate(dotProduct));
            
            if(_colorAdjustments == null) { return; }

            _colorAdjustments.colorFilter.value =
                Color.Lerp(nightAmbientLight, dayAmbientLight, lightIntensityCurve.Evaluate(dotProduct));
        }

        private void RotateSun()
        {
            float rotation = _timeService.CalculateSunAngle();
            //Rotate the sun around the x-axis
            sun.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.right);

            //Instead of adding to the current rotation, set the rotation to the initial rotation + the calculated rotation
            //Because it is more predictable and easier to debug
            dial.rotation = Quaternion.Euler(0, 0, rotation + _initialDialRotation);
        }
        private void UpdateTimeOfDay()
        {
            //Update the time service with the delta time
            _timeService.UpdateTime(Time.deltaTime);

            if (timeText != null)
            {
                //Show hours an Minutes in this format: eg. 12:00
                timeText.text = _timeService.CurrentTime.ToString("hh:mm");
            }
        }
    }
}