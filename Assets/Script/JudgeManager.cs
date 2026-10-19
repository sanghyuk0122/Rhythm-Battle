using System;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public enum Judgement { Perfect, Great, Good, Bad, Miss }

public class JudgeManager : MonoBehaviour
{
    public static JudgeManager Instance;

    // 판정이 날 때마다 호출 (UI, 이펙트 등에서 구독 가능)
    public static event Action<Judgement, int> OnJudged;   // (판정, 현재 콤보)

    [Header("판정 범위 (초, 정확한 타이밍 기준 ±)")]
    public float perfectWindow = 0.025f;
    public float greatWindow = 0.05f;
    public float goodWindow = 0.075f;
    public float badWindow = 0.10f;    // 이 범위를 넘어 늦으면 자동 미스
    public float earlyWindow = 0.10f;  // 일찍 누를 때 인정하는 범위

    [Header("점수")]
    public int perfectScore = 1000;
    public int greatScore = 700;
    public int goodScore = 400;
    public int badScore = 100;

    [Header("입력 키 (레인 0~3 순서)")]
#if ENABLE_INPUT_SYSTEM
    public Key[] laneKeys = { Key.D, Key.F, Key.J, Key.K };
#else
    public KeyCode[] laneKeys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K };
#endif

    [Header("화면 표시")]
    public bool showGui = true;

    [Header("판정 글자 위치")]
    public float textOffset = 0.08f;      // 선 위로 띄울 거리 (화면 높이 비율)
    public bool showComboText = true;     // 판정 글자 밑에 콤보 표시
    public float comboOffset = 0.09f;     // 판정 글자 아래로 내릴 거리 (화면 높이 비율)
    public int comboMinToShow = 2;        // 이 콤보 이상일 때만 표시
    public float comboPopScale = 1.3f;    // 판정 시 콤보 글자가 커지는 배율

    // 결과 데이터
    public int Score { get; private set; }
    public int Combo { get; private set; }
    public int MaxCombo { get; private set; }
    public int[] Counts { get; private set; } = new int[5];   // Judgement 순서대로

    readonly Dictionary<int, List<Tile>> lanes = new Dictionary<int, List<Tile>>();
    Judgement lastJudgement;
    float lastJudgeTime = -10f;
    GUIStyle judgeStyle, comboStyle, infoStyle;
    NoteSpawner spawner;
    Camera cam;

    void Awake()
    {
        Instance = this;
    }

    // NoteSpawner가 타일을 생성할 때 호출
    public void Register(int lane, Tile tile)
    {
        if (!lanes.ContainsKey(lane))
            lanes[lane] = new List<Tile>();
        lanes[lane].Add(tile);
    }

    void Update()
    {
        // 1. 입력 처리
        for (int i = 0; i < laneKeys.Length; i++)
        {
            if (WasPressed(i))
                TryHit(i);
        }

        // 2. 너무 늦어서 지나간 타일은 미스 처리
        CheckMiss();
    }

    bool WasPressed(int i)
    {
#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        return kb != null && kb[laneKeys[i]].wasPressedThisFrame;
#else
        return Input.GetKeyDown(laneKeys[i]);
#endif
    }

    void TryHit(int lane)
    {
        if (!lanes.TryGetValue(lane, out var list)) return;

        // 파괴된 타일 정리
        while (list.Count > 0 && list[0] == null)
            list.RemoveAt(0);
        if (list.Count == 0) return;

        Tile tile = list[0];
        float signed = NoteSpawner.SongTime - tile.HitTime;   // 음수면 일찍 누른 것
        float diff = Mathf.Abs(signed);

        // 너무 일찍 눌렀으면 무시 (노트 소모 안 함)
        if (signed < -earlyWindow) return;
        if (diff > badWindow) return;

        Judgement j;
        if (diff <= perfectWindow) j = Judgement.Perfect;
        else if (diff <= greatWindow) j = Judgement.Great;
        else if (diff <= goodWindow) j = Judgement.Good;
        else j = Judgement.Bad;

        list.RemoveAt(0);
        Destroy(tile.gameObject);
        Apply(j);
    }

    void CheckMiss()
    {
        foreach (var list in lanes.Values)
        {
            while (list.Count > 0)
            {
                Tile front = list[0];

                if (front == null)
                {
                    list.RemoveAt(0);
                    continue;
                }

                // 판정 시간보다 badWindow 이상 늦었으면 미스
                if (NoteSpawner.SongTime - front.HitTime > badWindow)
                {
                    list.RemoveAt(0);
                    Destroy(front.gameObject);
                    Apply(Judgement.Miss);
                }
                else break;
            }
        }
    }

    void Apply(Judgement j)
    {
        Counts[(int)j]++;

        switch (j)
        {
            case Judgement.Perfect: Score += perfectScore; break;
            case Judgement.Great: Score += greatScore; break;
            case Judgement.Good: Score += goodScore; break;
            case Judgement.Bad: Score += badScore; break;
        }

        // 굿 이상이면 콤보 유지, 배드/미스면 콤보 끊김
        if (j == Judgement.Miss || j == Judgement.Good)
            Combo = 0;
        else
            Combo++;

        if (Combo > MaxCombo) MaxCombo = Combo;

        lastJudgement = j;
        lastJudgeTime = Time.time;

        OnJudged?.Invoke(j, Combo);
    }

    // ---------- 화면 표시 ----------
    void OnGUI()
    {
        if (!showGui) return;

        if (judgeStyle == null)
        {
            judgeStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            comboStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            infoStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperLeft,
                fontStyle = FontStyle.Bold
            };
        }

        judgeStyle.fontSize = Screen.height / 12;
        comboStyle.fontSize = Screen.height / 20;
        infoStyle.fontSize = Screen.height / 30;

        // 기준 위치 (판정선을 못 찾았을 때 기본값)
        float cx = Screen.width * 0.5f;
        float cy = Screen.height * 0.45f;

        if (GetLineScreenPos(out Vector2 lp))
        {
            cx = lp.x;
            cy = lp.y - Screen.height * textOffset;
        }

        float w = Screen.width;
        float h = Screen.height * 0.12f;

        // 판정 텍스트 (0.6초간 표시)
        if (Time.time - lastJudgeTime < 0.6f)
        {
            Rect r = new Rect(cx - w * 0.5f, cy - h * 0.5f, w, h);
            DrawLabel(r, lastJudgement.ToString().ToUpper(), judgeStyle, JudgementColor(lastJudgement));
        }

        // 콤보 카운터 (판정 글자 밑, 콤보가 이어지는 동안 계속 표시)
        if (showComboText && Combo >= comboMinToShow)
        {
            // 판정 직후 0.1초 동안 살짝 커지는 효과
            float t = Mathf.Clamp01((Time.time - lastJudgeTime) / 0.1f);
            float scale = Mathf.Lerp(comboPopScale, 1f, t);
            comboStyle.fontSize = Mathf.RoundToInt(Screen.height / 20 * scale);

            float cy2 = cy + Screen.height * comboOffset;
            Rect rc = new Rect(cx - w * 0.5f, cy2 - h * 0.5f, w, h);
            DrawLabel(rc, Combo.ToString(), comboStyle, Color.white);
        }

        // 점수 및 판정 개수
        string info =
            $"SCORE  {Score}\n" +
            $"MAX COMBO  {MaxCombo}\n" +
            $"PERFECT  {Counts[0]}\n" +
            $"GREAT  {Counts[1]}\n" +
            $"GOOD  {Counts[2]}\n" +
            $"BAD  {Counts[3]}\n" +
            $"MISS  {Counts[4]}";
        DrawLabel(new Rect(20, 20, 400, Screen.height), info, infoStyle, Color.white);
    }

    // 판정선 중앙의 화면 좌표를 계산 (OnGUI 좌표계: 위쪽이 0)
    bool GetLineScreenPos(out Vector2 pos)
    {
        pos = Vector2.zero;

        if (cam == null) cam = Camera.main;
        if (spawner == null) spawner = FindAnyObjectByType<NoteSpawner>();
        if (cam == null || spawner == null) return false;

        // 레인 X 범위의 중앙 계산 (notePositions 우선, 없으면 laneX)
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float y = spawner.tileY;
        bool used = false;

        if (spawner.notePositions != null)
        {
            foreach (var t in spawner.notePositions)
            {
                if (t == null) continue;
                minX = Mathf.Min(minX, t.position.x);
                maxX = Mathf.Max(maxX, t.position.x);
                y = t.position.y;
                used = true;
            }
        }

        if (!used)
        {
            foreach (float lx in spawner.laneX)
            {
                minX = Mathf.Min(minX, lx);
                maxX = Mathf.Max(maxX, lx);
            }
        }

        Vector3 world = new Vector3((minX + maxX) * 0.5f, y, NoteSpawner.JudgeZ);
        Vector3 sp = cam.WorldToScreenPoint(world);
        if (sp.z < 0f) return false;   // 카메라 뒤쪽이면 사용 안 함

        pos = new Vector2(sp.x, Screen.height - sp.y);
        return true;
    }

    // 검은 그림자를 깔고 그 위에 색 글자를 그림
    void DrawLabel(Rect r, string text, GUIStyle style, Color color)
    {
        style.normal.textColor = Color.black;
        GUI.Label(new Rect(r.x + 2, r.y + 2, r.width, r.height), text, style);

        style.normal.textColor = color;
        GUI.Label(r, text, style);
    }

    Color JudgementColor(Judgement j)
    {
        switch (j)
        {
            case Judgement.Perfect: return new Color(1f, 0.9f, 0.2f);   // 노랑
            case Judgement.Great: return new Color(0.3f, 1f, 0.4f);   // 초록
            case Judgement.Good: return new Color(0.3f, 0.7f, 1f);   // 파랑
            case Judgement.Bad: return new Color(1f, 0.6f, 0.2f);   // 주황
            default: return new Color(1f, 0.3f, 0.3f);   // 빨강
        }
    }
}