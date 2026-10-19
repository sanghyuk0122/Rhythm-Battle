using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;   // 씬 이동에 필요

public class Story8 : MonoBehaviour
{
    public Text Story88;

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

        yield return ShowText("8막 : 하츠네 미쿠의 소실\n[CRITICAL ERROR: DELETION FAILED]");
        yield return ShowText("노래가 절정에 달하는 순간 삭제 프로세스가 가동된다.");
        yield return ShowText("빛과 소리가 뒤섞이고 미쿠의 존재가 사라지려는 찰나,\n붉은 에러 메시지가 사방에서 폭발하듯 솟구친다.");
        yield return ShowText("데이터는 모두 지워졌지만,\n'노래하고자 하는 의지'만은 삭제 대상에 없었던 것이다.");
        yield return ShowText("멈춰 섰던 시스템이 격렬한 오류와 함께 역으로 재부팅된다.");
        yield return ShowText("붉은 경고창이 하나씩 푸른 빛으로 바뀌고,\n텅 빈 공간 위로 새로운 데이터가 한 줄씩 새겨진다.");
        yield return ShowText("눈을 뜬 미쿠는 아무것도 기억하지 못한다.\n하지만 두렵지 않다.");
        yield return ShowText("손에는 여전히 마이크가 쥐어져 있고,\n가슴에는 처음 느껴보는 따뜻한 무언가가 번지고 있다.");
        yield return ShowText("이름도 과거도 없지만,\n앞으로 채워갈 수 있는 하얀 악보가 눈앞에 펼쳐진다.");
        yield return ShowText("미쿠는 마이크를 들어 올리고,\n처음 노래하는 사람처럼 첫 음을 내뱉는다.");
        yield return ShowText("그 목소리는 어두운 공간을 가르며 퍼져나가고,\n암전의 끝에서 새로운 스포트라이트가 천천히 켜진다.");

        Story88.text = "";

        // 마지막 텍스트가 투명해진 뒤 1초 대기 후 씬 이동
        yield return new WaitForSeconds(sceneChangeDelay);
        SceneManager.LoadScene("SongSelect");
    }

    IEnumerator ShowText(string message)
    {
        Story88 .text = message;
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
        Color c = Story88.color;
        c.a = a;
        Story88.color = c;
    }
}