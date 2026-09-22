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
        Debug.Log("1");
        SelectedSong = 1;
    }
    public void SongB()
    {
        Debug.Log("2");
        SelectedSong = 2;
    }
    public void SongC()
    {
        Debug.Log("3");
        SelectedSong = 3;
    }
    public void SongD()
    {
        Debug.Log("4");
        SelectedSong = 4;
    }
    public void SongE()
    {
        Debug.Log("5");
        SelectedSong = 5;
    }
    public void SongF()
    {
        Debug.Log("6");
        SelectedSong = 6;
    }
    public void SongG()
    {
        Debug.Log("7");
        SelectedSong = 7;
    }
    public void SongH()
    {
        Debug.Log("8");
        SelectedSong = 8;
    }
    public void SongStart()
    {
        SceneManager.LoadScene("Game");
    }
}
