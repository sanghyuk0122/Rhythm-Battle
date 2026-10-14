using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public static float SongTime;     // 모든 타일이 공유하는 현재 노래 시간
    public static float FallSpeed;    // 초당 이동 거리
    public static float JudgeY;       // 판정선 Y 좌표

    [Header("리소스")]
    public TextAsset chartFile;       // 채보 txt (확장자 .txt)
    public AudioSource music;
    public GameObject tilePrefab;

    [Header("레이아웃")]
    public Transform[] notePositions;  // 각 레인의 노트 생성 위치 (4개)
    public float[] laneX = { -3f, -1f, 1f, 3f }; // 레인별 X 좌표
    public float spawnY = 6f;                    // 타일이 생성되는 높이
    public float judgeY = -4f;                   // 판정선 높이
    public float fallSpeed = 8f;                 // 낙하 속도
    public float startDelay = 2f;                // 시작 전 대기(초)

    ChartData chart;
    int nextIndex;
    float fallTime;      // 생성부터 판정선까지 걸리는 시간
    double startDspTime;
    bool playing;

    void Start()
    {
        // JudgeManager 자동 검색 (없으면 생성)
        if (JudgeManager.Instance == null)
        {
            // 씬에서 JudgeManager 찾기
            JudgeManager judgeManager = FindFirstObjectByType<JudgeManager>();

            if (judgeManager == null)
            {
                // JudgeManager가 없으면 자동 생성
                Debug.Log("JudgeManager를 찾을 수 없어서 자동으로 생성합니다.");
                GameObject judgeManagerObj = new GameObject("JudgeManager");
                judgeManager = judgeManagerObj.AddComponent<JudgeManager>();
                Debug.Log("JudgeManager가 자동으로 생성되었습니다.");
            }
            else
            {
                Debug.Log("기존 JudgeManager를 찾았습니다.");
            }

            // JudgeManager.Instance 설정
            JudgeManager.Instance = judgeManager;
        }

        // null 체크
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

        FallSpeed = fallSpeed;
        JudgeY = judgeY;
        fallTime = (spawnY - judgeY) / fallSpeed;

        Debug.Log($"NoteSpawner 초기화 완료: 총 노트 개수 = {chart.notes.Count}, FallSpeed = {FallSpeed}, FallTime = {fallTime}");

        // 정확한 싱크를 위해 dspTime 사용
        startDspTime = AudioSettings.dspTime + startDelay;
        music.PlayScheduled(startDspTime);
        playing = true;
    }

    void Update()
    {
        if (!playing) return;

        // 현재 노래 시간 계산
        SongTime = (float)(AudioSettings.dspTime - startDspTime);

        // 차트 데이터 확인
        if (chart == null || chart.notes.Count == 0)
            return;

        // 다음 노트가 생성되어야 할 시간에 도달했는지 확인
        while (nextIndex < chart.notes.Count &&
               SongTime >= chart.notes[nextIndex].hitTime - fallTime)
        {
            Spawn(chart.notes[nextIndex]);
            nextIndex++;
        }
    }

    void Spawn(NoteData note)
    {
        // null 체크
        if (tilePrefab == null)
        {
            Debug.LogError("Spawn: tilePrefab이 할당되지 않았습니다!");
            return;
        }

        // lane 범위 체크
        if (note.lane < 0 || note.lane >= laneX.Length)
        {
            Debug.LogError($"Spawn: 유효하지 않은 lane 값입니다. lane = {note.lane}");
            return;
        }

        // 생성 위치 결정 (notePositions이 있으면 사용, 없으면 기본값)
        Vector3 spawnPosition;
        if (notePositions != null && notePositions.Length > note.lane && notePositions[note.lane] != null)
        {
            // NotePosition Transform의 위치에서 생성
            spawnPosition = notePositions[note.lane].position;
        }
        else
        {
            // 기본값: laneX와 spawnY 사용
            spawnPosition = new Vector3(laneX[note.lane], spawnY, 0f);
        }

        // Tile 오브젝트 생성
        var go = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);

        if (go == null)
        {
            Debug.LogError("Spawn: Instantiate 실패");
            return;
        }

        // Tile 컴포넌트 가져오기
        var tile = go.GetComponent<Tile>();

        if (tile == null)
        {
            Debug.LogError("Spawn: Tile 컴포넌트를 찾을 수 없습니다. Prefab에 Tile 컴포넌트가 있는지 확인하세요!");
            Destroy(go);
            return;
        }

        // Tile 초기화
        tile.Init(note.hitTime, laneX[note.lane]);

        // JudgeManager에 등록
        if (JudgeManager.Instance != null)
        {
            JudgeManager.Instance.Register(note.lane, tile);
        }
        else
        {
            Debug.LogError("Spawn: JudgeManager.Instance가 null입니다!");
        }
    }
}
