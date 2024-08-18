using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupMenuManager : MonoBehaviour
{
    public GameObject popupMenu;

    void Start()
    {
        // Verstecke das Popup-Menü zu Beginn
        popupMenu.SetActive(false);
        // Sperre den Cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Überprüfe, ob die Escape-Taste gedrückt wurde
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePopupMenu();
        }
    }

    public void TogglePopupMenu()
    {
        popupMenu.SetActive(!popupMenu.activeSelf);

        if (popupMenu.activeSelf)
        {
            // Entsperre den Cursor, wenn das Popup-Menü aktiviert ist
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Sperre den Cursor, wenn das Popup-Menü deaktiviert ist
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void RestartGame()
    {
        // Lädt die aktuelle Szene neu
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}