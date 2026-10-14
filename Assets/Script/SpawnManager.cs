using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject Note;
    public GameObject Point1;
    public GameObject Point2;
    public GameObject Point3;
    public GameObject Point4;

    IEnumerator Start() //코루틴
    {
        Instantiate(Note, Point1.transform.position, Point1.transform.rotation); //1번 위치에서 노트가 떨어지는 코드
        yield return new WaitForSeconds(0.1f);//0.1초 기다리는 코드
        Instantiate(Note, Point2.transform.position, Point2.transform.rotation); //2번 위치에서 노트가 떨어지는 코드
        yield return new WaitForSeconds(0.1f);//0.1초 기다리는 코드
        Instantiate(Note, Point3.transform.position, Point3.transform.rotation); //3번 위치에서 노트가 떨어지는 코드
        yield return new WaitForSeconds(0.1f); //0.1초 기다리는 코드
        Instantiate(Note, Point4.transform.position, Point4.transform.rotation); //4번 위치에서 노트가 떨어지는 코드
    }

    // Update is called once per frame
    void Update()
    {

    }
}
