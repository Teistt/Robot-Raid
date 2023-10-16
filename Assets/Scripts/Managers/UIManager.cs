using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreTMP;
    [SerializeField] private TextMeshProUGUI timeTMP;
    [SerializeField] private TextMeshProUGUI finalScoreTMP;
    [SerializeField] private TextMeshProUGUI finalTimeTMP;
    [SerializeField] private GameObject gameOverCanva;

    private bool isGameOver = false;
    private GameManager gm;


    #region Actions
    private void OnEnable()
    {
        GameManager.OnGameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= OnGameOver;
    }
    #endregion

    void Awake()
    {
        gm = GetComponent<GameManager>();
    }


    void Update()
    {
        if (isGameOver) return;
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


    private void OnGameOver()
    {
        Debug.Log("GAME LOST");

        isGameOver = true;

        finalScoreTMP.text = scoreTMP.text;
        finalTimeTMP.text = timeTMP.text;

        scoreTMP.gameObject.SetActive(false);
        timeTMP.gameObject.SetActive(false);
        gameOverCanva.SetActive(true);
    }

}
