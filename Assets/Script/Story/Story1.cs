using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;   // 씬 이동에 필요

public class Story1 : MonoBehaviour
{
    public Text Story11;

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
        StartCoroutine(StoryOne());
    }

    IEnumerator StoryOne()
    {
        yield return new WaitForSeconds(3f);

        yield return ShowText("1막 : 첫 번째 공연\n스포트라이트 아래의 미쿠");
        yield return ShowText("암전 속에서 첫 음향이 울린다. 무대 위로 조명이 쏟아지고\n관객들의 환호성이 터져 나온다.");
        yield return ShowText("미쿠는 스포트라이트 속으로 도약하고, 목소리가 공연장을 가득 채운다.\n펜라이트의 물결이 바다처럼 일렁인다.");
        yield return ShowText("하지만 그 눈부신 빛 너머, 객석의 어둠은\n이상할 만큼 깊고 끝이 없었다.");
        yield return ShowText("환호성이 잦아드는 마지막 순간, 어둠 속에서 누군가 자신을\n가만히 올려다보는 듯한 기분이 든다.");
        yield return ShowText("무대는 완벽하게 끝났지만, 박수 소리는 어딘가\n멀리서 들리는 메아리 같았다.");

        Story11.text = "";

        // 마지막 텍스트가 투명해진 뒤 1초 대기 후 씬 이동
        yield return new WaitForSeconds(sceneChangeDelay);
        SceneManager.LoadScene("SongSelect");
    }

    IEnumerator ShowText(string message)
    {
        Story11.text = message;
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
        Color c = Story11.color;
        c.a = a;
        Story11.color = c;
    }
}