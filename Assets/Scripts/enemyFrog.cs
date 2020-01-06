using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyFrog : MonoBehaviour
{
    public float speed = 5;
    public int direction = 1;


    // Update is called once per frame
    void Update()
    {
        transform.Translate(2 * Time.deltaTime * speed * direction, 0, 0);


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("limit"))
        {
            direction *= -1;
            Flip();
        }
    }

    void Flip()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

}
