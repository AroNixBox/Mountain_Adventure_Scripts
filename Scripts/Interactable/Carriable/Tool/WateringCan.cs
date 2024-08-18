using Mechanics.Gardening;
using Player.MonoBehaviors;
using UnityEngine;

namespace Interactable.Carriable.Tool {
    public class WateringCan : BaseOneActionTool {
        public override Vector3 EquipCoordinates { get; set; } = new (0.498f, 0.033f, 0.132f);
        public override Vector3 EquipRotation { get; } = new (-4.047f, -228.585f, -89.029f);
        public override void PerformAction() {
            PlantableArea plantableArea = ToolTip.GetClosestObjectOfType<PlantableArea>();
            if (plantableArea != null) {
                plantableArea.GrowPlant();
            }
        
            PlayerReferenceManager.IsActionCompleted = true; // Flag tells when the Action is done so the Player can return out of the state
        }

        public override int GetUseActionHash() {
            return PlayerReferenceManager.ANIM_WATERPLANTS;
        }
    }
}
