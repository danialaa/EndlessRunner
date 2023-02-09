using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuView : MonoBehaviour
{
    [SerializeField]
    TMP_Text menuTitle;
    [SerializeField]
    GameObject resumeButton;
    [SerializeField]
    GameObject restartButton;
    [SerializeField]
    GameObject startButton;
    [SerializeField]
    TMP_Text highscore;

    public void ShowStartMenu()
    {
        menuTitle.text = "3D Runner";
        Time.timeScale = 0;
        restartButton.SetActive(false);
        resumeButton.SetActive(false);
        startButton.SetActive(true);
        highscore.text = "Highscore: " + PlayerPrefs.GetInt("Highscore").ToString();
        highscore.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        menuTitle.text = "Game Over";
        Time.timeScale = 0;
        restartButton.SetActive(false);
        resumeButton.SetActive(false);
        startButton.SetActive(true);
        highscore.text = "Highscore: " + PlayerPrefs.GetInt("Highscore").ToString();
        highscore.gameObject.SetActive(true);
    }

    public void PauseGame()
    {
        menuTitle.text = "Pause";
        Time.timeScale = 0;
        restartButton.SetActive(true);
        resumeButton.SetActive(true);
        startButton.SetActive(false);
        highscore.gameObject.SetActive(false);
    }
}
