using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum EGameState
{
    GamePlay,
    Victory,
    GameOver
}


public class GameController : MonoBehaviour
{
    public static GameController instance;

    public float minDelayBetweenSpawns;
    public float maxDelayBetweenSpawns;
    public float chanceForInfectedSpawn;
    public float timeToSurvive;
    public float maxInfected;
    public float hospitalCapacity;
    public float hospitalCareTime;

    private float currentTime;
    public EGameState eGameState = EGameState.GamePlay;

    [HideInInspector]public float currentInfected = 0;
    [HideInInspector] public float currentPeople = 0;
    [HideInInspector] public float currentInHospital = 0;


    public Text timeRemainingText;
    public Text inFectedText;
    public Text totalPeopleText;
    public Text onHospitalText;
    public Text nextVacancyText;


    private float timeUntilNextVacancy = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(eGameState == EGameState.GamePlay)
        {
            currentTime += Time.deltaTime;
            timeRemainingText.text = "Survive For: " + (timeToSurvive - currentTime).ToString("F2") + "s";
            inFectedText.text = currentInfected + " / " + maxInfected + " Infected";
            totalPeopleText.text = currentPeople + " Moving Around";
            onHospitalText.text = "Current in Hospital: " + currentInHospital + " / " + hospitalCapacity; 

            if(timeUntilNextVacancy > 0)
            {
                timeUntilNextVacancy -= Time.deltaTime;
                nextVacancyText.text = "Time Until Next Vancancy: " + timeUntilNextVacancy.ToString("F2") + "s";
            }
            else
            {
                timeUntilNextVacancy -= 0;
                nextVacancyText.text = "Time Until Next Vancancy: -";
            }

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
        }
        else
        {
            Time.timeScale = 0f;
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
}
