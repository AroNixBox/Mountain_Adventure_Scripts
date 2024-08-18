using System;
using UnityEngine;

namespace Player.MonoBehaviors
{
    public class PlayerRewardHandler : MonoBehaviour
    {
        [SerializeField] private SuperTextMesh rewardText;
        private static int _currentPoints;
        private DateTime _hideTime;

        private void Start()
        {
            _currentPoints = 50;
            rewardText.text = string.Empty;
        }

        private void OnEnable()
        {
            UIEvents.PassNumber.Get("RewardPoints").AddListener(AddPoints);
            UIEvents.PassNumber.Get("RemovePoints").AddListener(RemovePoints); 
         }
        private void AddPoints(int points)
        {
            _currentPoints += points;
            ShowPoints();
        }
        public static int GetPoints()
        {
            return _currentPoints;
        }

        private void RemovePoints(int points)
        {
            _currentPoints -= points;
            ShowPoints();
        }
        private void Update()
        {
            if (DateTime.Now > _hideTime)
            {
                rewardText.text = string.Empty;
            }
        }
        private void ShowPoints()
        {
            //TODO Count the points up/ down from the points before to now
            //TODO Show the points in a nice way, maybe add sound?
            
            rewardText.text = "<c=rainbow><w=seasick>" + _currentPoints + "</w></c>";
            _hideTime = DateTime.Now.AddSeconds(4);
        }

        private void OnDestroy()
        {
            UIEvents.PassNumber.Get("RemovePoints").AddListener(RemovePoints);
            UIEvents.PassNumber.Get("RewardPoints").RemoveListener(AddPoints);
        }
    }
}
