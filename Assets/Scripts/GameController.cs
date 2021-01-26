using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum EGameState
{
    GamePlay,
    Victory,
    GameOver
}


public class GameController : MonoBehaviour
{
    public static GameController instance;


    [Header("Level Parameters")]
    public float minDelayBetweenSpawns;
    public float maxDelayBetweenSpawns;
    public float chanceForInfectedSpawn;
    public float chanceForAsymptomaticSpawn;
    public float chanceForRedInfectedSpawn;
    public float timeToSurvive;
    public float maxPeople;
    public float maxInfected;
    public float hospitalCapacity;
    public float hospitalCareTime;
    public float timeToInfection;
    public float AsymptomaticDelay;
    public float AsymptomaticTransformationDuration;
    public float chanceToBeOldGuy;
    public float minTimeDoctor;
    public float maxTimeDoctor;
    public float immunityPeriod = 5f;

    private float currentTime;
    [HideInInspector]public float actualTimeDoctor;
    public EGameState eGameState = EGameState.GamePlay;
    public string nextLevelName;
    public string currentLevelName;

    [HideInInspector]public float currentInfected = 0;
    [HideInInspector] public float currentPeople = 0;
    
    [HideInInspector] public float currentInHospital = 0;

    [Header("Texts")]
    public Text timeRemainingText;
    public Text inFectedText;
    public Text totalPeopleText;
    public Text onHospitalText;
    public Text nextVacancyText;

    [Header("Canvas")]
    public GameObject youWinCanvas;
    public GameObject youLoseCanvas;
    public GameObject optionsCanvas;

    private float timeUntilNextVacancy = 0;

    private bool isOptionsOnScreen;

    private void Awake()
    {
        Time.timeScale = 1f;
        instance = this;
        actualTimeDoctor = Random.Range(minTimeDoctor, maxTimeDoctor);
    }

    private void Update()
    {
        if(eGameState == EGameState.GamePlay)
        {
            currentTime += Time.deltaTime;    
            
            if(currentTime > actualTimeDoctor)
            {
                Spawner.instance.SpawnDoctor();
                actualTimeDoctor = 999999f;
            }


            timeRemainingText.text = /*"Survive For: " + */(timeToSurvive - currentTime).ToString("F2") + "s";
            inFectedText.text = currentInfected + " / " + maxInfected/* + " Infected"*/;
            totalPeopleText.text = currentPeople.ToString() /* + " Moving Around"*/;
            onHospitalText.text = /*"Current in Hospital: " + */currentInHospital + " / " + hospitalCapacity; 

            if(timeUntilNextVacancy > 0)
            {
                timeUntilNextVacancy -= Time.deltaTime;
                nextVacancyText.text = /*"Time Until Next Vancancy: " + */timeUntilNextVacancy.ToString("F2") + "s";
            }
            else
            {
                timeUntilNextVacancy -= 0;
                nextVacancyText.text = /*"Time Until Next Vancancy: -"*/ "-";
            }

            if (currentInfected > (maxInfected * 0.75)) inFectedText.color = Color.red;
            else inFectedText.color = Color.white;

            if (currentInfected >= maxInfected)
            {
                eGameState = EGameState.GameOver;
                timeRemainingText.text = "You Lost!";
            }

            if (currentTime >= timeToSurvive)
            {
                eGameState = EGameState.Victory;
                timeRemainingText.text = "You Won!";
            }


            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isOptionsOnScreen)
                {
                    Time.timeScale = 0f;
                    optionsCanvas.SetActive(true);
                    isOptionsOnScreen = true;
                }
                else
                {
                    Time.timeScale = 1f;
                    optionsCanvas.SetActive(false);
                    isOptionsOnScreen = false;
                }
            }


        }
        else if(eGameState == EGameState.GameOver)
        {
            Time.timeScale = 0f;
            youLoseCanvas.gameObject.SetActive(true);
            if(Input.GetKeyDown(KeyCode.R))
            {
                LoadNewLevel(currentLevelName);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        else if (eGameState == EGameState.Victory)
        {
            Time.timeScale = 0f;
            youWinCanvas.gameObject.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Return))
            {
                LoadNewLevel(nextLevelName);
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }

    public void FreeHospitalFunction()
    {
        StartCoroutine(FreeHospitalRoutine());
    }

    public IEnumerator FreeHospitalRoutine()
    {
        timeUntilNextVacancy = hospitalCareTime;
        yield return new WaitForSeconds(hospitalCareTime);
        if(currentInHospital > 0) currentInHospital -= 1;
        if (currentInHospital > 0) StartCoroutine(FreeHospitalRoutine());
    }

    public void LoadNewLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
