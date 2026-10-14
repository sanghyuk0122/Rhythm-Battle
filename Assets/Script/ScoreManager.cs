using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text ComboPoint;
    public Text ScorePoint;
    public Text precisionP;
    public int ComboP = 0;
    public int ScoreP = 0;

    public void AddCombo()
    {
        ComboP++;
        ComboPoint.text = ComboP.ToString();
    }
    public void ResetCombo()
    {
        ComboP = 0;
        ComboPoint.text = ComboP.ToString();
    }
    public void AddScore()
    {
        ScoreP += 120;
        ScorePoint.text = ScoreP.ToString("D6");
    }
    public void Perfect()
    {
        precisionP.text = "Prefect";
    }
    public void Miss()
    {
        precisionP.text = "Miss";
    }
}