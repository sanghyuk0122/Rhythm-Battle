using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    [Header("규칙")]
    public int missLimit = 20;              // 이 횟수만큼 미스가 나면 게임 오버

    [Header("이동할 씬 이름")]
    public string songSelectSceneName = "SongSelect";   // 게임 오버 시
    public string storySceneName = "Story";             // 클리어 시

    [Header("연출")]
    public float endDelay = 2.5f;           // 결과 글자를 보여준 뒤 씬 이동까지 대기(초)
    public bool showGui = true;

    NoteSpawner spawner;
    int missCount;
    int judgedCount;
    bool ended;
    string resultText = "";
    GUIStyle resultStyle, missStyle;

    void OnEnable()
    {
        JudgeManager.OnJudged += HandleJudged;
    }

    void OnDisable()
    {
        JudgeManager.OnJudged -= HandleJudged;
    }

    void Start()
    {
        spawner = FindAnyObjectByType<NoteSpawner>();
        if (spawner == null)
            Debug.LogError("GameFlowManager: NoteSpawner를 찾을 수 없습니다!");
    }

    // 판정이 날 때마다 호출됨
    void HandleJudged(Judgement j, int combo)
    {
        if (ended) return;

        judgedCount++;
        if (j == Judgement.Miss) missCount++;

        // 게임 오버가 클리어보다 우선
        if (missCount >= missLimit)
        {
            EndGame(false);
            return;
        }

        // 모든 노트가 판정(히트 또는 미스)됐으면 클리어
        if (spawner != null && spawner.TotalNotes > 0 && judgedCount >= spawner.TotalNotes)
        {
            EndGame(true);
        }
    }

    void EndGame(bool clear)
    {
        ended = true;
        resultText = clear ? "CLEAR!" : "GAME OVER";

        // 더 이상 노트가 나오거나 입력되지 않도록 정지
        if (spawner != null)
        {
            spawner.enabled = false;
            if (spawner.music != null && !clear) spawner.music.Stop();
        }
        if (JudgeManager.Instance != null)
            JudgeManager.Instance.enabled = false;

        // 남아있는 타일 제거 (게임 오버 시)
        if (!clear)
        {
            foreach (var tile in FindObjectsByType<Tile>())
                Destroy(tile.gameObject);
        }

        StartCoroutine(LoadSceneRoutine(clear ? storySceneName : songSelectSceneName));
    }

    IEnumerator LoadSceneRoutine(string sceneName)
    {
        yield return new WaitForSecondsRealtime(endDelay);

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("GameFlowManager: 이동할 씬 이름이 비어 있습니다.");
            yield break;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    void OnGUI()
    {
        if (!showGui) return;

        if (resultStyle == null)
        {
            resultStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            missStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperRight,
                fontStyle = FontStyle.Bold
            };
        }

        resultStyle.fontSize = Screen.height / 8;
        missStyle.fontSize = Screen.height / 30;

        // 오른쪽 위: 현재 미스 / 제한
        Color warn = missCount >= missLimit * 0.7f ? new Color(1f, 0.3f, 0.3f) : Color.white;
        DrawLabel(new Rect(Screen.width - 420, 20, 400, 100), $"MISS  {missCount} / {missLimit}", missStyle, warn);

        // 화면 중앙: 결과
        if (ended)
        {
            Color c = resultText == "CLEAR!" ? new Color(1f, 0.9f, 0.2f) : new Color(1f, 0.3f, 0.3f);
            DrawLabel(new Rect(0, 0, Screen.width, Screen.height), resultText, resultStyle, c);
        }
    }

    void DrawLabel(Rect r, string text, GUIStyle style, Color color)
    {
        style.normal.textColor = Color.black;
        GUI.Label(new Rect(r.x + 2, r.y + 2, r.width, r.height), text, style);
        style.normal.textColor = color;
        GUI.Label(r, text, style);
    }
}