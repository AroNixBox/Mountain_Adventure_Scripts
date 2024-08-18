using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mechanics.SeasonSwitch
{
    public class SeasonSwitchManager : MonoBehaviour
    {
         /*
         * 0 = Spring
         * 1 = Summer
         * 2 = Autumn
         * 3 = Winter
         */
    
        [SerializeField] private List<SeasonTransform> seasons;
        public int CurrentSeason { get; private set; } 
    
        //TODO: Read the values properly, so Spring one is really 0 physically.
        public void SwitchSeason(int seasonToSwitchTo) {
            CurrentSeason = seasonToSwitchTo;
            for(var i = 0; i < seasons.Count; i++) {
                foreach(var season in seasons[i].Element) {
                    season.gameObject.SetActive(i == seasonToSwitchTo);
                }
            }
        }

        [System.Serializable]
        public class SeasonTransform {
            [FormerlySerializedAs("Season")] public Transform[] Element;
        }
    }
}
