using UnityEngine;

public class Tile : MonoBehaviour
{
    public float HitTime { get; private set; }
    float x;
    float y;

    public void Init(float hitTime, float x)
    {
        HitTime = hitTime;
        this.x = x;
        // Instantiate 직후 호출되므로 현재 위치의 Y가 곧 생성 높이
        this.y = transform.position.y;
    }

    void Update()
    {
        // 남은 시간으로 Z 위치를 계산 (노래와 싱크 유지)
        float z = NoteSpawner.GetTileZ(HitTime);
        transform.position = new Vector3(x, y, z);
        // 삭제(히트/미스)는 JudgeManager가 담당
    }
}