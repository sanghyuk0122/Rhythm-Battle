using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;   // 씬 이동에 필요

public class Story3 : MonoBehaviour
{
    public Text Story33;

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
        StartCoroutine(StoryThree());
    }

    IEnumerator StoryThree()
    {
        yield return new WaitForSeconds(3f);

        yield return ShowText("3막 : 낯선 일상\n무감각해진 감성");
        yield return ShowText("대기실의 불은 어딘가 차갑고 창백하다.");
        yield return ShowText("미쿠는 자신에게 늘 영감을 주었던 음악들을 재생한다.");
        yield return ShowText("스피커에서 선명한 소리가 흘러나오지만, 아무것도 느껴지지 않는다.");
        yield return ShowText("가장 좋아하던 곡이 흐르는데도 가슴은 텅 비어 있다.");
        yield return ShowText("익숙해야 할 멜로디가 남의 노래처럼 낯설고,\n유리벽 너머에서 들리는 소리처럼 아득하다.");
        yield return ShowText("미쿠는 재생 버튼을 몇 번이고 다시 누른다.\n달라지는 것은 없었다.");
        yield return ShowText("조용히 스피커를 끄자, 정적이 방 안을 삼킨다");

        Story33.text = "";

        // 마지막 텍스트가 투명해진 뒤 1초 대기 후 씬 이동
        yield return new WaitForSeconds(sceneChangeDelay);
        SceneManager.LoadScene("SongSelect");
    }

    IEnumerator ShowText(string message)
    {
        Story33.text = message;
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
        Color c = Story33.color;
        c.a = a;
        Story33.color = c;
    }
}