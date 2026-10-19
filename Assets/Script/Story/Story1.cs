using System.Collections;
using UnityEngine;
using UnityEngine.UI;



public class Story1 : MonoBehaviour

{
    public Text Story11;
    void Start()

    {
        StartCoroutine(StoryOne());
    }

    IEnumerator StoryOne()
    {
        yield return new WaitForSeconds(3f);
        Story11.text = "암전 속에서 첫 음향이 울린다. 무대 위로 조명이 쏟아지고\n관객들의 환호성이 터져 나온다.";
        yield return new WaitForSeconds(4f);
        Story11.text = "미쿠는 스포트라이트 속으로 도약하고, 목소리가 공연장을 가득 채운다.\n펜라이트의 물결이 바다처럼 일렁인다.";
        yield return new WaitForSeconds(4f);
        Story11.text = "하지만 그 눈부신 빛 너머, 객석의 어둠은\n이상할 만큼 깊고 끝이 없었다.";
        yield return new WaitForSeconds(4f);
        Story11.text = "환호성이 잦아드는 마지막 순간, 어둠 속에서 누군가 자신을\n가만히 올려다보는 듯한 기분이 든다.";
        yield return new WaitForSeconds(4f);
        Story11.text = "무대는 완벽하게 끝났지만, 박수 소리는 어딘가\n멀리서 들리는 메아리 같았다.";
        yield return new WaitForSeconds(4f);
        Story11.text = "";
    }
}