using UnityEngine;

public class RestartButton : MonoBehaviour
{
    private PopupMenuManager popupMenuManager;

    void Start()
    {
        // Finde das PopupMenuManager-Skript im Canvas
        popupMenuManager = FindObjectOfType<PopupMenuManager>();
    }

    public void OnRestartButtonClick()
    {
        popupMenuManager.RestartGame();
    }
}