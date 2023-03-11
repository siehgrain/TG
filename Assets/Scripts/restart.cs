using UnityEngine;
using UnityEngine.SceneManagement;

public class restart : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Restart the current scene when a collision occurs
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
