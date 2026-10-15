using UnityEngine;
using UnityEngine.SceneManagement;

public class SelecSong : MonoBehaviour
{
    public static SelecSong Instance;

    public int SelectedSong = 0; // 0 = 아직 선택 안 함

    // 곡 번호 순서대로 씬 이름 입력 (인스펙터에서 수정 가능)
    public string[] sceneNames = new string[8]
    {
        "Tell your world", // 1
        "Senbon",          // 2
        "Gostlol",                // 3
        "Vampire",                // 4
        "The Intense Voice of Hatsune Miku",                // 5
        "Die",                // 6
        "Machine Gun Poem Doll",                // 7
        "Sosil"                 // 8
    };

   

    // 곡 선택 버튼용 (씬 전환 X, 선택만 저장)
    public void SongA() { SelectedSong = 1; }
    public void SongB() { SelectedSong = 2; }
    public void SongC() { SelectedSong = 3; }
    public void SongD() { SelectedSong = 4; }
    public void SongE() { SelectedSong = 5; }
    public void SongF() { SelectedSong = 6; }
    public void SongG() { SelectedSong = 7; }
    public void SongH() { SelectedSong = 8; }

    // Play 버튼용 (선택된 곡의 씬으로 전환)
    public void SongStart()
    {
        Debug.Log("SongStart 호출됨, SelectedSong = " + SelectedSong);
        // ...나머지 코드
        int index = SelectedSong - 1;

        if (SelectedSong == 0)
        {
            Debug.Log("곡을 먼저 선택하세요.");
            return;
        }

        if (index < 0 || index >= sceneNames.Length || string.IsNullOrEmpty(sceneNames[index]))
        {
            Debug.LogWarning("선택된 곡에 연결된 씬이 없습니다: " + SelectedSong);
            return;
        }

        SceneManager.LoadScene(sceneNames[index]);
    }
}