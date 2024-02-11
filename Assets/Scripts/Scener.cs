using UnityEngine;
using UnityEngine.SceneManagement;


public class Scener : MonoBehaviour
{
    public void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
