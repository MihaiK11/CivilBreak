using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKeyDown) // Detects any key press
        {
            SceneManager.LoadScene("copy"); // Change to your scene name
        }
    }
}
