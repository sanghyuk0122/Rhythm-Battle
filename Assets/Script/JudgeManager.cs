using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JudgeManager : MonoBehaviour
{
    public static JudgeManager Instance;

    [Header("레인별 입력 키")]
    public KeyCode[] keys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K };

    [Header("판정 범위(초) - 정확한 시간 기준 ±")]
    public float perfectWindow = 0.05f;
    public float goodWindow = 0.12f;
    public float missWindow = 0.2f;   // 이 시간 넘게 지나가면 미스

    [Header("점수")]
    public int perfectScore = 300;
    public int goodScore = 100;

    [Header("UI (선택)")]
    public TMP_Text scoreText;
    public TMP_Text comboText;
    public TMP_Text judgeText;

    public int Score { get; private set; }
    public int Combo { get; private set; }

    Queue<Tile>[] lanes;

    void Awake()
    {
        Instance = this;
        lanes = new Queue<Tile>[keys.Length];
        for (int i = 0; i < lanes.Length; i++) lanes[i] = new Queue<Tile>();
        UpdateUI("");
    }

    // 타일이 생성될 때 NoteSpawner가 호출 (생성 순서 = 판정 순서)
    public void Register(int lane, Tile tile)
    {
        lanes[lane].Enqueue(tile);
    }

    void Update()
    {
        for (int lane = 0; lane < lanes.Length; lane++)
        {
            var q = lanes[lane];

            // 1) 놓쳐서 지나간 타일 처리
            while (q.Count > 0 && NoteSpawner.SongTime - q.Peek().HitTime > missWindow)
            {
                Destroy(q.Dequeue().gameObject);
                Miss();
            }

            // 2) 키 입력 처리
            if (Input.GetKeyDown(keys[lane]) && q.Count > 0)
            {
                Tile tile = q.Peek();
                float diff = Mathf.Abs(tile.HitTime - NoteSpawner.SongTime);

                if (diff <= perfectWindow) Hit(q, perfectScore, "PERFECT");
                else if (diff <= goodWindow) Hit(q, goodScore, "GOOD");
                // goodWindow보다 너무 일찍 눌렀으면 무시 (타일 유지)
            }
        }
    }

    void Hit(Queue<Tile> q, int points, string label)
    {
        Destroy(q.Dequeue().gameObject);   // 타일 제거
        Combo++;
        Score += points + Combo;           // 콤보 보너스
        UpdateUI(label);
    }

    void Miss()
    {
        Combo = 0;
        UpdateUI("MISS");
    }

    void UpdateUI(string judge)
    {
        if (scoreText) scoreText.text = Score.ToString("N0");
        if (comboText) comboText.text = Combo > 1 ? Combo + " COMBO" : "";
        if (judgeText) judgeText.text = judge;
    }
}
