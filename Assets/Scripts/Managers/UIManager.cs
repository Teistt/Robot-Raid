using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreTMP;
    [SerializeField] private TextMeshProUGUI timeTMP;
    private GameManager gm;

    void Awake()
    {
        gm = GetComponent<GameManager>();
    }


    void Update()
    {
        int minutes = (int)Time.time / 60;
        int seconds = (int)Time.time % 60;

        string m = minutes.ToString();
        string s = seconds.ToString();

        if (minutes < 10)
        {
            m = "0" + minutes.ToString();
        }
        if (seconds < 10)
        {
            s = "0" + Mathf.RoundToInt(seconds).ToString();
        }
        //GUI.Label(new Rect(10, 10, 250, 100), minutes + ":" + seconds);



        scoreTMP.text = "SCORE\n" + gm.score.ToString();
        timeTMP.text = "SURVIVED TIME\n" + m+":"+s;
    }
}
