using UnityEngine;

public class Tile : MonoBehaviour
{
    public float HitTime { get; private set; }
    float x;

    public void Init(float hitTime, float x)
    {
        HitTime = hitTime;
        this.x = x;
    }

    void Update()
    {
        // 남은 시간으로 위치를 계산 (노래와 싱크 유지)
        // SongTime이 음수일 때는 아직 노래 시작 전이므로 spawnY에 머물러 있음
        float timeDiff = HitTime - NoteSpawner.SongTime;
        float y = NoteSpawner.JudgeY + timeDiff * NoteSpawner.FallSpeed;

        transform.position = new Vector3(x, y, 0f);
        // 삭제(히트/미스)는 JudgeManager가 담당
    }
}
