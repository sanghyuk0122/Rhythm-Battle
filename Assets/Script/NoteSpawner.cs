using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class NoteSpawner : MonoBehaviour
{
    public static float SongTime;     // 모든 타일이 공유하는 현재 노래 시간
    public static float FallSpeed;    // 초당 이동 거리
    public static float JudgeZ;       // 판정선 Z 좌표

    [Header("리소스")]
    public TextAsset chartFile;
    public AudioSource music;
    public GameObject tilePrefab;

    [Header("레이아웃")]
    public Transform[] notePositions;  // 각 레인의 노트 생성 위치 (4개)
    public float[] laneX = { -3f, -1f, 1f, 3f };
    public float tileY = 0f;           // 타일 높이(Y) 고정값
    public float spawnZ = 20f;         // 타일이 생성되는 Z 위치
    public float judgeZ = 0f;          // 판정선 Z 위치
    public Transform judgeLine;        // 판정선 오브젝트 (연결하면 그 Z 위치를 사용)
    public float startDelay = 2f;      // 시작 전 대기(초)

    [Header("속도 조절")]
    [Range(1f, 40f)]
    public float fallSpeed = 15f;      // 타일 속도 (클수록 빠름)
    public float speedStep = 1f;       // ↑/↓ 키 한 번에 바뀌는 양
    public bool showSpeedGui = true;   // 현재 속도 화면 표시
    [Header("싱크 보정")]
    public float noteDelay = 0f;      // 초. +면 노트가 늦게 도착
    public float syncStep = 0.01f;    // ←/→ 한 번에 바뀌는 양(초)

    const float MinSpeed = 1f;
    const float MaxSpeed = 40f;

    ChartData chart;
    int nextIndex;
    float fallTime;
    double startDspTime;
    bool playing;
    public int TotalNotes => chart != null ? chart.notes.Count : 0;   // 차트 전체 노트 수
    public bool IsPlaying => playing;                                  // 곡이 시작됐는지
    GUIStyle speedStyle;

    // 낙하 시간 다시 계산 (속도나 판정선이 바뀔 때마다 호출)
    void RecalcFallTime()
    {
        float startZ = spawnZ;
        if (notePositions != null && notePositions.Length > 0 && notePositions[0] != null)
            startZ = notePositions[0].position.z;

        fallTime = Mathf.Abs(startZ - JudgeZ) / FallSpeed;
    }

    // 판정선 Z를 설정
    public void SetJudgeZ(float z)
    {
        judgeZ = z;
        JudgeZ = z;
        RecalcFallTime();
    }

    // 타일 속도를 설정 (게임 중에도 호출 가능)
    public void SetFallSpeed(float speed)
    {
        fallSpeed = Mathf.Clamp(speed, MinSpeed, MaxSpeed);
        FallSpeed = fallSpeed;
        RecalcFallTime();
    }

    // 판정 시간에 해당하는 타일의 Z 위치 계산
    public static float GetTileZ(float hitTime)
    {
        float timeDiff = hitTime - SongTime;
        return JudgeZ + timeDiff * FallSpeed;
    }

    void Start()
    {
        if (JudgeManager.Instance == null)
        {
            JudgeManager judgeManager = FindAnyObjectByType<JudgeManager>();

            if (judgeManager == null)
            {
                Debug.Log("JudgeManager를 찾을 수 없어서 자동으로 생성합니다.");
                GameObject judgeManagerObj = new GameObject("JudgeManager");
                judgeManager = judgeManagerObj.AddComponent<JudgeManager>();
            }

            JudgeManager.Instance = judgeManager;
        }

        if (chartFile == null)
        {
            Debug.LogError("NoteSpawner: chartFile이 할당되지 않았습니다!");
            return;
        }
        if (music == null)
        {
            Debug.LogError("NoteSpawner: music AudioSource가 할당되지 않았습니다!");
            return;
        }
        if (tilePrefab == null)
        {
            Debug.LogError("NoteSpawner: tilePrefab이 할당되지 않았습니다!");
            return;
        }

        chart = ChartData.Parse(chartFile.text);

        if (chart == null || chart.notes.Count == 0)
        {
            Debug.LogError("NoteSpawner: 차트 데이터가 비어있거나 로드되지 않았습니다!");
            return;
        }

        FallSpeed = Mathf.Clamp(fallSpeed, MinSpeed, MaxSpeed);

        // 판정선 오브젝트가 있으면 그 Z 위치를 judgeZ로 사용
        if (judgeLine != null)
            judgeZ = judgeLine.position.z;

        SetJudgeZ(judgeZ);

        Debug.Log($"NoteSpawner 초기화 완료: 총 노트 개수 = {chart.notes.Count}, FallSpeed = {FallSpeed}, FallTime = {fallTime}");

        startDspTime = AudioSettings.dspTime + startDelay;
        music.PlayScheduled(startDspTime);
        playing = true;
    }

    void Update()
    {
        HandleSpeedInput();

        if (!playing) return;

        SongTime = (float)(AudioSettings.dspTime - startDspTime);

        if (chart == null || chart.notes.Count == 0)
            return;

        while (nextIndex < chart.notes.Count &&
               SongTime >= chart.notes[nextIndex].hitTime - fallTime)
        {
            Spawn(chart.notes[nextIndex]);
            nextIndex++;
        }
    }

    // ↑ / ↓ 키로 속도 조절
    void HandleSpeedInput()
    {
        bool up, down, left, right;
#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb == null) return;
        up = kb.upArrowKey.wasPressedThisFrame;
        down = kb.downArrowKey.wasPressedThisFrame;
        left = kb.leftArrowKey.wasPressedThisFrame;
        right = kb.rightArrowKey.wasPressedThisFrame;
#else
    up = Input.GetKeyDown(KeyCode.UpArrow);
    down = Input.GetKeyDown(KeyCode.DownArrow);
    left = Input.GetKeyDown(KeyCode.LeftArrow);
    right = Input.GetKeyDown(KeyCode.RightArrow);
#endif
        if (up) SetFallSpeed(fallSpeed + speedStep);
        if (down) SetFallSpeed(fallSpeed - speedStep);
        if (left) noteDelay -= syncStep;
        if (right) noteDelay += syncStep;
    }

    void Spawn(NoteData note)
    {
        if (tilePrefab == null)
        {
            Debug.LogError("Spawn: tilePrefab이 할당되지 않았습니다!");
            return;
        }

        if (note.lane < 0 || note.lane >= laneX.Length)
        {
            Debug.LogError($"Spawn: 유효하지 않은 lane 값입니다. lane = {note.lane}");
            return;
        }

        Vector3 spawnPosition;
        if (notePositions != null && notePositions.Length > note.lane && notePositions[note.lane] != null)
            spawnPosition = notePositions[note.lane].position;
        else
            spawnPosition = new Vector3(laneX[note.lane], tileY, spawnZ);

        var go = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);

        if (go == null)
        {
            Debug.LogError("Spawn: Instantiate 실패");
            return;
        }

        var tile = go.GetComponent<Tile>();

        if (tile == null)
        {
            Debug.LogError("Spawn: Tile 컴포넌트를 찾을 수 없습니다. Prefab에 Tile 컴포넌트가 있는지 확인하세요!");
            Destroy(go);
            return;
        }

        tile.Init(note.hitTime, spawnPosition.x);

        if (JudgeManager.Instance != null)
            JudgeManager.Instance.Register(note.lane, tile);
        else
            Debug.LogError("Spawn: JudgeManager.Instance가 null입니다!");
    }

    // 현재 속도 표시 (왼쪽 아래)
    void OnGUI()
    {
        if (!showSpeedGui) return;

        if (speedStyle == null)
        {
            speedStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
            speedStyle.normal.textColor = Color.white;
        }
        speedStyle.fontSize = Screen.height / 35;

        GUI.Label(new Rect(20, Screen.height - Screen.height / 12f, 600, Screen.height / 14f),
                  $"SPEED  {fallSpeed:0.0}   (↑ / ↓)", speedStyle);
    }
}