using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public float speed;
    public GameObject target1, target2;
    private GameObject targetObject1, targetObject2;
    public Vector2 targetPosition1, targetPosition2;
    [SerializeField] private Vector2 currentTarget;
    private bool isFacingLeft = false;
    public bool isDragging = false;
    private bool isTargetOne;
    public GameObject ContagionCircle;

    public float infectedTime;
    public int infectedCollisionAmt;
    public CircleCollider2D cc;
    public SpriteRenderer sr;
    private void Start()
    {
        //cc = GetComponent<CircleCollider2D>();
        cc.enabled = !cc.enabled;

        InitializeTargets();
        GameController.instance.currentPeople += 1;
    }


    private void Update()
    {
        if(!CompareTag("Infected"))
        {
            if (infectedCollisionAmt > 0)
            {
                infectedTime += Time.deltaTime;
                sr.color = Color.Lerp(Color.white, Color.green, infectedTime / GameController.instance.timeToInfection);


                if (infectedTime > GameController.instance.timeToInfection)
                {
                    StartSelfInfectionFunction();
                }
            }

            else if (infectedCollisionAmt == 0)
            {
                infectedTime = 0;
                sr.color = Color.white;
            }
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition = mousePos - transform.position;

        if (GameController.instance.eGameState == EGameState.GamePlay)
        {
            if (!isDragging)
            {
                CheckSide();

                MovePerson();

                CheckTarget();
            }

            else if ((mousePos.x < 8f && mousePos.x > -8f && mousePos.y < 4f && mousePos.y > -3f) && isDragging)
            {
                transform.Translate(mousePosition);
            }
        }
    }

    private void OnMouseDown()
    {
        //if(!CompareTag("Infected"))
        isDragging = true;
    }


    private void OnMouseUp()
    {
        //if (!CompareTag("Infected"))
        //{
            isDragging = false;
            ReinitializeTargets();
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Infected") && !this.CompareTag("Infected"))
        {
            //StartSelfInfectionFunction();
            infectedCollisionAmt++;
        }
    }
     private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Infected") && !this.CompareTag("Infected"))
        {
            //StartSelfInfectionFunction();
            infectedCollisionAmt--;
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Hospital") && this.CompareTag("Infected"))
        {
            if (GameController.instance.currentInHospital < GameController.instance.hospitalCapacity && isDragging) EnterHospital();
        }
    }
    public void EnterHospital()
    {
        GameController.instance.currentPeople -= 1;
        GameController.instance.currentInfected -= 1;
        GameController.instance.currentInHospital += 1;
        if (GameController.instance.currentInHospital == 1) GameController.instance.FreeHospitalFunction();
        Destroy(this.gameObject);
    }

    public void InitializeTargets()
    {
        while(Mathf.Abs(targetPosition1.x - targetPosition2.x) < 2)
        {
            targetObject1 = Instantiate(target1, new Vector3(Random.Range(-8.5f, 8.5f), transform.position.y, 0), Quaternion.identity, transform);
            targetObject2 = Instantiate(target2, new Vector3(Random.Range(-8.5f, 8.5f), transform.position.y, 0), Quaternion.identity, transform);
            targetPosition1 = targetObject1.transform.position;
            targetPosition2 = targetObject2.transform.position;
        }

        if (target1.transform.position.x > target2.transform.position.x)
        {
            currentTarget = targetPosition1;
            isTargetOne = true;
        }
        else
        {
            currentTarget = targetPosition2;
            isTargetOne = false;
        }


    }

    public void CheckSide()
    {
        if (currentTarget.x > transform.position.x)
        {
            if (isFacingLeft)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = false;
                isFacingLeft = false;
            }
        }

        if (currentTarget.x < transform.position.x)
        {
            if (!isFacingLeft)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
                isFacingLeft = true;
            }
        }
    }

    public void MovePerson()
    {
            transform.position = Vector2.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);
    }

    public void CheckTarget()
    {
        if(transform.position.x == currentTarget.x)
        if(isTargetOne)
        {
            isTargetOne = false;
            currentTarget = targetPosition2;
        }
        else
        {
            isTargetOne = true;
            currentTarget = targetPosition1;
        }
    }



    public void ReinitializeTargets()
    {
        targetPosition1 = new Vector2(targetPosition1.x, transform.position.y);
        targetPosition2 = new Vector2(targetPosition2.x, transform.position.y);

        currentTarget = targetPosition1;
        isTargetOne = true;
    }

    public void StartSelfInfectionFunction()
    {
        StartCoroutine(StartSelfInfectionRoutine());
    }

    public IEnumerator StartSelfInfectionRoutine()
    {
        gameObject.tag = "Infected";
        GameController.instance.currentInfected += 1;
        sr.color = Color.green;
        ContagionCircle.SetActive(true);
        cc.enabled = !cc.enabled;
        //if(isDragging)
        //{
        //    isDragging = false;
        //    ReinitializeTargets();
        //}
        yield return null;
    }


}
