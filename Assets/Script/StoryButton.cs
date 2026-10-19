using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneName = "Story8";

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}