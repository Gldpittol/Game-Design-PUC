using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject personPrefab;
    private GameObject temp;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());   
    }

    public IEnumerator SpawnRoutine()
    {
        if (GameController.instance.eGameState == EGameState.GamePlay && GameController.instance.currentPeople < GameController.instance.maxPeople - 1)
        {
            yield return new WaitForSeconds(Random.Range(GameController.instance.minDelayBetweenSpawns, GameController.instance.maxDelayBetweenSpawns));

            bool instantiateLeft = (Random.value < 0.5f);

            if (instantiateLeft)
            {
                temp = Instantiate(personPrefab, new Vector2(Random.Range(-10f, -9f), Random.Range(-2.5f, 2.5f)), Quaternion.identity);
                if (Random.value < GameController.instance.chanceForInfectedSpawn) temp.GetComponent<Person>().StartSelfInfectionFunction();
                else if (Random.value < GameController.instance.chanceForAsymptomaticSpawn) temp.GetComponent<Person>().AsympTransformFunction();
                else if (Random.value < GameController.instance.chanceForRedInfectedSpawn) temp.GetComponent<Person>().StartSelfInfectionRedFunction();
            }
            else
            {
                temp = Instantiate(personPrefab, new Vector2(Random.Range(10f, 9f), Random.Range(-2.5f, 2.5f)), Quaternion.identity);
                if (Random.value < GameController.instance.chanceForInfectedSpawn) temp.GetComponent<Person>().StartSelfInfectionFunction();
                else if (Random.value < GameController.instance.chanceForAsymptomaticSpawn) temp.GetComponent<Person>().AsympTransformFunction();
                else if (Random.value < GameController.instance.chanceForRedInfectedSpawn) temp.GetComponent<Person>().StartSelfInfectionRedFunction();
            }
            StartCoroutine(SpawnRoutine());
        }
    }


}
