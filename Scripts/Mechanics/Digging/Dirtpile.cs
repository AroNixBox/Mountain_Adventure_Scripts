using System.Linq;
using Extensions;
using Interactable.Carriable.Tool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Digging {
    public class Dirtpile : MonoBehaviour, IReactableToTool {
        [SerializeField, Required] private AudioClip destroyPileSound;
        [SerializeField, Required] private ParticleSystem destroyPileParticles;
        [SerializeField] private DigObjectData[] trash;
        [SerializeField] private DigObjectData[] treasures;
        public void DigOut() {
            // Combine the trash and treasures arrays into a single array
            var allObjects = trash.Concat(treasures).ToArray();

            // Randomly sort the combined array
            var random = new System.Random();
            allObjects = allObjects.OrderBy(x => random.Next()).ToArray();

            // Find the first element that satisfies the condition
            var selectedObject = allObjects.FirstOrDefault(obj => ProbabilityHelper.CheckProbability(obj.rarity));

            // Check if a suitable object was found
            if (selectedObject != null) {
                // Instantiate the selected object
                Instantiate(selectedObject.prefab, transform.position + selectedObject.position, transform.rotation);
                
                AudioSource.PlayClipAtPoint(selectedObject.spawnSound, selectedObject.position);
                Instantiate(selectedObject.spawnParticles, selectedObject.position, Quaternion.identity);
            }
            
            // Destroy the dirt pile
            // Spawn Poof particles and play some sound
            AudioSource.PlayClipAtPoint(destroyPileSound, transform.position);
            Instantiate(destroyPileParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        
        [System.Serializable]
        public class DigObjectData {
            [HorizontalGroup("Data", 0.7f)]
            [VerticalGroup("Data/Left")]
            public Vector3 position;

            [VerticalGroup("Data/Left")]
            public AudioClip spawnSound;

            [VerticalGroup("Data/Left")]
            public ParticleSystem spawnParticles;

            [VerticalGroup("Data/Left")]
            [Range(0, 100)] public int rarity;

            [HorizontalGroup("Data")]
            [PreviewField]
            public GameObject prefab;
        }
    }
}