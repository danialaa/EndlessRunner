using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Objects")]
    [SerializeField]
    List<GameObject> cityPool = new List<GameObject>();

    List<GameObject> cities = new List<GameObject>();

    [Header("Data")]
    [SerializeField]
    float speed;
    [SerializeField]
    float damageAlertFadeSpeed;
    [SerializeField]
    float damageMaxTransparency;
    bool isRunning = true;

    [Header("UI")]
    [SerializeField]
    List<GameObject> lives = new List<GameObject>();
    [SerializeField]
    GameObject menuPopup;
    int score;
    [SerializeField]
    TMP_Text scoreText;
    [SerializeField]
    GameObject damageAlert;

    bool isFadingIn, isFadingOut;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        GenerateCity();
        Time.timeScale = 0;
        menuPopup.SetActive(true);
        menuPopup.GetComponent<MenuView>().ShowStartMenu();
    }

    IEnumerator FadeDamageAlertIn()
    {
        while (isFadingIn && damageAlert.GetComponent<Image>().color.a < damageMaxTransparency)
        {
            Color currentColor = damageAlert.GetComponent<Image>().color;
            float newAlpha = currentColor.a + (damageAlertFadeSpeed * Time.deltaTime);
            damageAlert.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            yield return null;
        }

        isFadingIn = false;
        isFadingOut = true;
        StartCoroutine(FadeDamageAlertOut());
    }

    IEnumerator FadeDamageAlertOut()
    {
        while (isFadingOut && damageAlert.GetComponent<Image>().color.a > 0)
        {
            Color currentColor = damageAlert.GetComponent<Image>().color;
            float newAlpha = currentColor.a - (damageAlertFadeSpeed * Time.deltaTime);
            damageAlert.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            yield return null;
        }

        isFadingOut = false;
    }

    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

    void GenerateCity()
    {
        for (int i = 0; i < cityPool.Count; i++)
        {
            cities.Add(GameObject.Instantiate(cityPool[i], cityPool[i].transform.position, Quaternion.identity));
            cities[cities.Count - 1].SetActive(true);
        }
    }

    void RecycleCityFromPool(int i)
    {
        Destroy(cities[i]);
        cities[i] = null;

        int randomCityIndex = Random.Range(0, cityPool.Count);

        cities[i] = GameObject.Instantiate(cityPool[randomCityIndex], new Vector3(cityPool[randomCityIndex].transform.position.x,
                                                                                  cityPool[randomCityIndex].transform.position.y,
                                                                                  4), Quaternion.identity);
        cities[i].SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    void FixedUpdate()
    {
        if (isRunning)
        {
            MoveCity();
        }
    }

    public void ReduceLife(int lives)
    {
        if (lives >= 0)
        {
            this.lives[lives].SetActive(false);
        }

        isFadingIn = true;
        StartCoroutine(FadeDamageAlertIn());

        if(lives <= 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        PlayerPrefs.SetInt("Highscore", score);
        menuPopup.SetActive(true);
        menuPopup.GetComponent<MenuView>().GameOver();
    }

    public void PauseGame()
    {
        menuPopup.SetActive(true);
        menuPopup.GetComponent<MenuView>().PauseGame();
    }

    void MoveCity()
    {
        for (int i = 0; i < cities.Count; i++)
        {
            if (cities[i] != null)
            {
                cities[i].transform.position = new Vector3(cities[i].transform.position.x, cities[i].transform.position.y, cities[i].transform.position.z - (speed * Time.deltaTime));

                if (cities[i].transform.position.z <= -16)
                {
                    RecycleCityFromPool(i);
                }
            }
        }
    }

    public void ResumeGame()
    {
        menuPopup.SetActive(false);
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
