using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyButton : MonoBehaviour
{
    public string lobbySceneName = "Lobby";
    public string songSelectSceneName = "SongSelect";

    public void GoToLobby()
    {
        LoadSceneSafe(lobbySceneName);
    }

    public void GoToSongSelect()
    {
        LoadSceneSafe(songSelectSceneName);
    }

    private void LoadSceneSafe(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("씬 이름이 비어 있습니다.");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}