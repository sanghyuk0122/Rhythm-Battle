using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;   // 씬 이동에 필요

public class Story2 : MonoBehaviour
{
    public Text Story22;

    [Header("페이드 설정")]
    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;
    public float holdTime = 3f;

    [Header("씬 전환 설정")]
    public string nextSceneName = "SongSelect";
    public float sceneChangeDelay = 1f;   // 텍스트가 투명해진 뒤 대기 시간

    void Start()
    {
        SetAlpha(0f);
        StartCoroutine(StoryTwo());
    }

    IEnumerator StoryTwo()
    {
        yield return new WaitForSeconds(3f);

        yield return ShowText("2막 : 어긋난 선율\n작은 균열의 시작");
        yield return ShowText("미쿠는 평소처럼 완벽한 타이밍에 무대로 나선다.");
        yield return ShowText("그러나 하이라이트에 다다랐을 때,\n다음 멜로디와 가사가 통째로 공백이 된다.");
        yield return ShowText("미쿠는 미소로 그 틈을 덮고 무대를 이어간다.\n관객은 아무도 눈치채지 못했다.");
        yield return ShowText("그러나 무대 뒤로 내려온 미쿠의 귓가에서\n낮은 노이즈가 사라지지 않는다.");
        yield return ShowText("마치 무언가가 안쪽에서 갉아먹고 있는 소리처럼.");

        Story22.text = "";

        // 마지막 텍스트가 투명해진 뒤 1초 대기 후 씬 이동
        yield return new WaitForSeconds(sceneChangeDelay);
        SceneManager.LoadScene("SongSelect");
    }

    IEnumerator ShowText(string message)
    {
        Story22.text = message;
        yield return Fade(0f, 1f, fadeInTime);
        yield return new WaitForSeconds(holdTime);
        yield return Fade(1f, 0f, fadeOutTime);
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, t / duration));
            yield return null;
        }
        SetAlpha(to);
    }

    void SetAlpha(float a)
    {
        Color c = Story22.color;
        c.a = a;
        Story22.color = c;
    }
}