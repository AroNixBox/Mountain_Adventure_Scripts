namespace Extensions {
    using UnityEngine;

    public static class ProbabilityHelper {
        public static bool CheckProbability(int probability) {
            float randomValue = Random.value;
            return randomValue <= (probability / 100f);
        }
    }
}