using System.Collections;
using UnityEngine;
using UnityEngine.UI;



public class Story2 : MonoBehaviour

{
    public Text Story22;
    void Start()

    {
        StartCoroutine(StoryTwo());
    }

    IEnumerator StoryTwo()
    {
        yield return new WaitForSeconds(3f);
        Story22.text = "미쿠는 평소처럼 완벽한 타이밍에 무대로 나선다.\n그러나 하이라이트에 다다랐을 때,\n다음 멜로디와 가사가 통째로 공백이 된다. ";
        yield return new WaitForSeconds(4f);
        Story22.text = "짧은 찰나였지만 미쿠에게는 영원처럼 길었다.";
        yield return new WaitForSeconds(4f);
        Story22.text = "미쿠는 미소로 그 틈을 덮고 무대를 이어간다.\n관객은 아무도 눈치채지 못했다.";
        yield return new WaitForSeconds(4f);
        Story22.text = "그러나 무대 뒤로 내려온 미쿠의 귓가에서\n낮은 노이즈가 사라지지 않는다.\n마치 무언가가 안쪽에서 갉아먹고 있는 소리처럼.";
        yield return new WaitForSeconds(4f);
        Story22.text = "";
    }
}