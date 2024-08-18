using Interactable.Carriable.Tool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Gardening {
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public class PlantableArea : MonoBehaviour, IReactableToTool {
        [BoxGroup("Plant"), HideLabel][SerializeField] private Plant plant;
        private Rigidbody _rigidbody;
        private SphereCollider _collider;
    
        private GameObject _growingPlant;
        private int _currentUpgradeIndex;
        private void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<SphereCollider>();
        
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
            _collider.isTrigger = true;
        }

        public void PlantSeed() {
            if (_growingPlant != null) return; // Only Grow if there is no plant;
        
            var currentPlantData = plant.upgrades[_currentUpgradeIndex];
            _growingPlant = InstantiatePlant(currentPlantData);
            _currentUpgradeIndex++;
        }

        public void GrowPlant() {
            if (_growingPlant == null) return; // Only Upgrade if there is a plant
        
            Destroy(_growingPlant);
            
            if (_currentUpgradeIndex < plant.upgrades.Length) {
                var currentPlantData = plant.upgrades[_currentUpgradeIndex];
                _growingPlant = InstantiatePlant(currentPlantData);
                _currentUpgradeIndex++;
            } else {
                if (plant.grownPlant.prefab.GetComponent<GrownPlant>() == null) {
                    Debug.LogError("GrownPlant Script is missing on the Grown Plant Prefab");
                    return;
                }
                
                InstantiatePlant(plant.grownPlant);
                _currentUpgradeIndex = 0;
            }
        }

        private GameObject InstantiatePlant(PlantData plantData) {
            var newPlant = Instantiate(plantData.prefab, transform);
            newPlant.transform.localPosition = plantData.position;
            AudioSource.PlayClipAtPoint(plantData.spawnSound, transform.position);
            var particles = Instantiate(plantData.upgradeParticles, transform);
            particles.transform.localPosition = Vector3.zero;
            return newPlant;
        }

        [System.Serializable]
        public class Plant {
            [Header("Upgrades")]
            public PlantData[] upgrades;
            [Header("Fully Grown Plant")] [InfoBox("GrownPlant Script needs to be added on the Prefab")]
            public PlantData grownPlant; 
        }
        

        [System.Serializable]
        public class PlantData {
            [HorizontalGroup("Data", 0.7f)]
            [VerticalGroup("Data/Left")]
            public Vector3 position;

            [VerticalGroup("Data/Left")]
            public AudioClip spawnSound;

            [VerticalGroup("Data/Left")]
            public ParticleSystem upgradeParticles;

            [HorizontalGroup("Data")]
            [HideLabel]
            [PreviewField(ObjectFieldAlignment.Center, Height = 100)]
            public GameObject prefab;
        }
    }
}