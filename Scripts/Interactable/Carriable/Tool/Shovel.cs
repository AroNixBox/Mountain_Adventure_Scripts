using Mechanics;
using Mechanics.Digging;
using Player.MonoBehaviors;
using UnityEngine;

namespace Interactable.Carriable.Tool {
    public class Shovel : BaseOneActionTool {
        public override Vector3 EquipCoordinates { get; set; } = new (0.097f, 0.046f, -0.026f);
        public override Vector3 EquipRotation { get; } = new (-105.163f, -415.912f, 88.606f);
        public override void PerformAction() {
            Dirtpile dirtpile = ToolTip.GetClosestObjectOfType<Dirtpile>();
            if (dirtpile != null) {
                dirtpile.DigOut();
            }
        
            PlayerReferenceManager.IsActionCompleted = true; // Flag tells when the Action is done so the Player can return out of the state
        }

        public override int GetUseActionHash() {
            return PlayerReferenceManager.ANIM_SHOVEL;
        }
    }
}
