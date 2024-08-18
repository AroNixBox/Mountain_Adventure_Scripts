using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// Sets the text for the MinigamePopup based on if there is a controller connected or not
/// </summary>
public class MinigameTextPopupSetter : MonoBehaviour {
    [SerializeField] private TMP_Text minigameDescription;
    [SerializeField] private Description manualDescription;
    [SerializeField] private Description continueDescription;
    // Because this popup gets disabled each time it "despawns" we only need to check here if there is a controller connected
    private void OnEnable() {
        StringBuilder sb = new StringBuilder();
        sb.Append(manualDescription.firstTextHalf + " ");
        sb.Append(IsControllerConnected() ? manualDescription.controllerInputButton : manualDescription.mouseInputButton);
        sb.Append(" " + manualDescription.secondTextHalf);

        sb.AppendLine();
        sb.AppendLine();
        
        sb.Append(continueDescription.firstTextHalf + " ");
        sb.Append(IsControllerConnected() ? continueDescription.controllerInputButton : continueDescription.mouseInputButton);
        sb.Append(" " + continueDescription.secondTextHalf);

        minigameDescription.text = sb.ToString();
    }

    private bool IsControllerConnected() {
        var joysticks = Input.GetJoystickNames();
        return joysticks.Length > 0 && !string.IsNullOrEmpty(joysticks[0]);
    }

    [System.Serializable]
    public class Description {
        [TextArea(3, 10)] public string firstTextHalf;
        public string controllerInputButton;
        public string mouseInputButton;
        [TextArea(3, 10)] public string secondTextHalf;
    }
}
