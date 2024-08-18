using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartOnTrigger : MonoBehaviour
{
    // Tag, nach dem gesucht wird
    public string triggerTag = "Player";

    // Methode, die aufgerufen wird, wenn ein anderer Collider den Trigger betritt
    private void OnTriggerEnter(Collider other)
    {
        // Überprüfen, ob der andere Collider den gewünschten Tag hat
        if (other.CompareTag(triggerTag))
        {
            // Aktuelle Szene neu laden
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}