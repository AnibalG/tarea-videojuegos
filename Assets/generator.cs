using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generator : MonoBehaviour
{
    public GameObject verticalWall;
    public GameObject horizontalGround;
    public float spawnTime = 5.0f;
    private Vector2 screenBounds;
    // Start is called before the first frame update
    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        StartCoroutine(wallFall());
    }

    private void spawnWall()
    {
        GameObject randomWall = Instantiate(verticalWall) as GameObject;
        GameObject randomGround = Instantiate(horizontalGround) as GameObject;
        randomWall.transform.position = new Vector2(Random.Range(-screenBounds.x, screenBounds.x), screenBounds.y * 3);
        randomGround.transform.position = new Vector2(Random.Range(-screenBounds.x + 2, screenBounds.x - 2), screenBounds.y);
    }

    IEnumerator wallFall()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnTime);
            spawnWall();
        }
    }

}
