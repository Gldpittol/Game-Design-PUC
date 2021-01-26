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

    public bool isAsymptomatic = false;
    public bool isRedInfected = false;
    public bool isOldGuy = false;

    private Animator animator;

    public GameObject circleOutline;
    public GameObject visualFeedback;
    public GameObject immunityFeedback;

    private float currentImmunityTime = 0f;

    private List<Collider2D> listCollider = new List<Collider2D>();
    private void Start()
    {
        speed = Random.Range(speed * 0.5f, speed * 1.5f);

        immunityFeedback.SetActive(false);

        animator = GetComponent<Animator>();
        cc.enabled = !cc.enabled;

        InitializeTargets();
        GameController.instance.currentPeople += 1;
    }


    private void Update()
    {
        currentImmunityTime -= Time.deltaTime;

        CheckCollisions();

        if((CompareTag("Person") && currentImmunityTime <= 0))
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
        if(currentImmunityTime > 0)
            immunityFeedback.SetActive(true);
        else immunityFeedback.SetActive(false);
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
        listCollider.Add(collision);

        if(collision.CompareTag("Infected") && (CompareTag("Person") || CompareTag("Asymptomatic")))
        {
            //StartSelfInfectionFunction();
            //infectedCollisionAmt++;
        }

        if (collision.CompareTag("RedInfected") && (CompareTag("Person") || CompareTag("Asymptomatic")) && currentImmunityTime <= 0)
        {
            isRedInfected = true;
            StartSelfInfectionFunction();
        }
    }
     private void OnTriggerExit2D(Collider2D collision)
    {

        listCollider.Remove(collision);

        if (collision.CompareTag("Infected") && infectedCollisionAmt > 0)
        {
            //StartSelfInfectionFunction();
            //infectedCollisionAmt--;
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Hospital") && (CompareTag("Infected") || CompareTag("RedInfected")))
        {
            if (GameController.instance.currentInHospital < GameController.instance.hospitalCapacity && isDragging)
            {
                EnterHospital();
            }
        }
    }


    public void Age()
    {
        isOldGuy = true;
        cc.enabled = !cc.enabled;
        animator = GetComponent<Animator>();
        animator.Play("Person@OldGuy");
        visualFeedback.SetActive(true);
    }

    public void EnterHospital()
    {
        tag = "Person";
        GameController.instance.currentPeople -= 1;
        GameController.instance.currentInfected -= 1;
        GameController.instance.currentInHospital += 1;
        if (GameController.instance.currentInHospital == 1) GameController.instance.FreeHospitalFunction();
        Destroy(gameObject);
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

    public void StartSelfInfectionRedFunction()
    {
        isRedInfected = true;
        StartCoroutine(StartSelfInfectionRoutine());
    }

    public void CurePerson()
    {
        currentImmunityTime = GameController.instance.immunityPeriod;
        gameObject.tag = "Person";
        ContagionCircle.SetActive(false);
        if (!isOldGuy) cc.enabled = !cc.enabled;
        sr.color = Color.white;
        GameController.instance.currentInfected -= 1;
        infectedTime = 0;

        if (isOldGuy)
        {
            animator.Play("Person@OldGuy");
            visualFeedback.SetActive(true);
        }
        else
        {
            animator.Play("PersonWalk");
        }
    }

    public IEnumerator StartSelfInfectionRoutine()
    {
        visualFeedback.SetActive(false);
        infectedTime = 0;
        yield return null;

        if(!isRedInfected)
        {
            gameObject.tag = "Infected";
            GameController.instance.currentInfected += 1;
            sr.color = Color.green;
            ContagionCircle.SetActive(true);
            SpriteRenderer temp = ContagionCircle.GetComponent<SpriteRenderer>();
            temp.color = Color.green;
            temp.color = new Color(temp.color.r, temp.color.g, temp.color.b, 0.4f);
            temp = circleOutline.GetComponent<SpriteRenderer>();
            temp.color = Color.green;
            temp.color = new Color(temp.color.r, temp.color.g, temp.color.b, 0.4f);

            if (!isOldGuy) cc.enabled = !cc.enabled;
        }

        else
        {
            gameObject.tag = "RedInfected";
            GameController.instance.currentInfected += 1;
            sr.color = Color.red;
            ContagionCircle.SetActive(true);
            SpriteRenderer temp = ContagionCircle.GetComponent<SpriteRenderer>();
            temp.color = Color.red;
            temp.color = new Color(temp.color.r, temp.color.g, temp.color.b, 0.4f);
            temp = circleOutline.GetComponent<SpriteRenderer>();
            temp.color = Color.red;
            temp.color = new Color(temp.color.r, temp.color.g, temp.color.b, 0.4f);

            if (!isOldGuy) cc.enabled = !cc.enabled;
        }

        if (isOldGuy) animator.Play("Person@OldGuyInfected");
        else
        {
          animator.Play("Person@Infected");
        }

        //if(isDragging)
        //{
        //    isDragging = false;
        //    ReinitializeTargets();
        //}
        yield return null;
    }


    public void AsympTransformFunction()
    {
        tag = "Asymptomatic";
        StartCoroutine(AsympTransform());
    }
    public IEnumerator AsympTransform()
    {
        yield return new WaitForSeconds(GameController.instance.AsymptomaticDelay);
        visualFeedback.SetActive(false);

        if (this.CompareTag("Asymptomatic"))
        {
            while (infectedTime < GameController.instance.AsymptomaticTransformationDuration)
            {

                infectedTime += Time.deltaTime;
                sr.color = Color.Lerp(Color.white, Color.green, infectedTime / GameController.instance.AsymptomaticTransformationDuration);
                yield return null;
            }

            ContagionCircle.SetActive(true);
            SpriteRenderer temp = ContagionCircle.GetComponent<SpriteRenderer>();
            temp.color = Color.green;
            temp.color = new Color(temp.color.r, temp.color.g, temp.color.b, 0.4f);
            if (!isOldGuy) cc.enabled = !cc.enabled;
            temp = circleOutline.GetComponent<SpriteRenderer>();
            temp.color = Color.green;
            temp.color = new Color(temp.color.r, temp.color.g, temp.color.b, 0.4f);

            GameController.instance.currentInfected += 1;
            tag = "Infected";
            if (isOldGuy) animator.Play("Person@OldGuyInfected");
            else
            {
                animator.Play("Person@Infected");
            }

        }



        else yield return null;
    }
     
    public void CheckCollisions()
    {
        infectedCollisionAmt = 0;

        for(int i = 0; i < listCollider.Count; i++)
        {
            if(listCollider[i].gameObject.CompareTag("Infected"))
            {
                infectedCollisionAmt++;
            }
        }
    }
}
