using TMPro;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

public class NoteSetting : MonoBehaviour
{
    public float speed;
    public bool CanTouchA;
    public bool CanTouchB;
    public bool CanTouchC;
    public bool CanTouchD;
    public bool Miss;
    public TextMeshProUGUI ComboPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0f, 0f, -1f) * speed * Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (CanTouchA == true)
            {
                FindAnyObjectByType<ScoreManager>().AddCombo();
                FindAnyObjectByType<ScoreManager>().AddScore();
                FindAnyObjectByType<ScoreManager>().Perfect();
                Destroy(gameObject);
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (CanTouchB == true)
            {
                FindAnyObjectByType<ScoreManager>().AddCombo();
                FindAnyObjectByType<ScoreManager>().AddScore();
                FindAnyObjectByType<ScoreManager>().Perfect();
                Destroy(gameObject);
            }
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (CanTouchC == true)
            {
                FindAnyObjectByType<ScoreManager>().AddCombo();
                FindAnyObjectByType<ScoreManager>().AddScore();
                FindAnyObjectByType<ScoreManager>().Perfect();
                Destroy(gameObject);
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (CanTouchD == true)
            {
                FindAnyObjectByType<ScoreManager>().AddCombo();
                FindAnyObjectByType<ScoreManager>().AddScore();
                FindAnyObjectByType<ScoreManager>().Perfect();
                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Perfect1")
        {
            CanTouchA = true;
        }
        else
        {
            CanTouchA = false;
        }
        if (other.gameObject.name == "Perfect2")
        {
            CanTouchB = true;
        }
        else
        {
            CanTouchB = false;
        }
        if (other.gameObject.name == "Perfect3")
        {
            CanTouchC = true;
        }
        else
        {
            CanTouchC = false;
        }
        if (other.gameObject.name == "Perfect4")
        {
            CanTouchD = true;
        }
        else
        {
            CanTouchD = false;
        }
        if (other.gameObject.name == "DeadLine")
        {
            FindAnyObjectByType<ScoreManager>().Miss();
            FindAnyObjectByType<ScoreManager>().ResetCombo();
            Destroy(gameObject);
        }
    }
}
