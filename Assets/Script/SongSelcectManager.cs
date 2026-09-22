using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class SelecSong : MonoBehaviour
{
    public int SelectedSong;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void SongA()
    {
        SelectedSong = 1;
    }
    public void SongB()
    {
        SelectedSong = 2;
    }
    public void SongC()
    {
        SelectedSong = 3;
    }
    public void SongD()
    {
        SelectedSong = 4;
    }
    public void SongE()
    {
        SelectedSong = 5;
    }
    public void SongF()
    {
        SelectedSong = 6;
    }
    public void SongG()
    {
        SelectedSong = 7;
    }
    public void SongH()
    {
        SelectedSong = 8;
    }
    public void SongStart()
    {
        SceneManager.LoadScene("Game");
    }
}
