using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    // 인스펙터에서 실제 씬 이름으로 수정하세요
    public string menuSceneName = "Lobby";
    public string songSelectSceneName = "SongSelect";

    // 메뉴로 돌아가기 버튼용
    public void GoToLobby()
    {
        LoadSceneSafe(menuSceneName);
    }

    // 곡 선택창으로 가기 버튼용
    public void GoToSongSelect()
    {
        LoadSceneSafe(songSelectSceneName);
    }

    private void LoadSceneSafe(string Lobby)
    {
        if (string.IsNullOrEmpty(Lobby))
        {
            Debug.LogWarning("씬 이름이 비어 있습니다.");
            return;
        }

        // 일시정지(Time.timeScale = 0) 상태에서 나가는 경우를 대비
        Time.timeScale = 1f;

        SceneManager.LoadScene(Lobby);
    }
}