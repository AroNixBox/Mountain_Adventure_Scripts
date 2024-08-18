using Mechanics;
using Mechanics.Gardening;
using Player.MonoBehaviors;
using UnityEngine;

namespace Interactable.Carriable.Tool {
    public class Plow : BaseOneActionTool {
        public override Vector3 EquipCoordinates { get; set; } = new (0.108f, 0.005f, -0.13f);
        public override Vector3 EquipRotation { get; } = new (-99.829f, 34.185f, -23.50702f);
        public override void PerformAction() {
            PlantableArea plantableArea = ToolTip.GetClosestObjectOfType<PlantableArea>();
            if (plantableArea != null) {
                plantableArea.PlantSeed();
            }
            
            PlayerReferenceManager.IsActionCompleted = true; // Flag tells when the Action is done so the Player can return out of the state
        }

        public override int GetUseActionHash() {
            return PlayerReferenceManager.ANIM_FARMWORK;
        }
    }
}
