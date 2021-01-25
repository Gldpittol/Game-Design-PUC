using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorScript : MonoBehaviour
{
    private Vector2 originalPos;
    private bool goRight;
    public float speed;
    private void Awake()
    {
        originalPos = transform.position;

        if (originalPos.x < 0) goRight = true;
        else
        {
            goRight = false;
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }
    private void Update()
    {
        if (goRight) transform.position = new Vector2(transform.position.x + speed * Time.deltaTime, transform.position.y);
        else transform.position = new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Infected") || collision.CompareTag("RedInfected"))
        {
            collision.GetComponent<Person>().CurePerson();
        }
    }
}
