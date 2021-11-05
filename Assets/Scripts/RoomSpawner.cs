using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public int minSize = 10;
    public int maxSize = 50;

    public int numberOfRooms = 100;
    public GameObject roomPrefab;

    public bool spawnNow = false;
    public int spawnRadius = 5;

    public Transform roomHolder;

    void SpawnNow(int numOfRooms)
    {
        Time.timeScale = 100;

        if (roomHolder != null)
        {
            Destroy(roomHolder.gameObject);
        }

        roomHolder = new GameObject("Room Holder").transform;
        roomHolder.transform.position = Vector3.zero;

        for (int i=0; i< numOfRooms; i++)
        {
            GameObject tmp = GameObject.Instantiate(roomPrefab, roomHolder);

            tmp.transform.localScale = new Vector3(Random.Range(minSize, maxSize), Random.Range(minSize, maxSize), 1);
            tmp.transform.position = Random.insideUnitCircle * spawnRadius;

            SpriteRenderer sr = tmp.GetComponent<SpriteRenderer>();
            sr.color = new Color(Random.Range(0f,1f), Random.Range(0f, 1f),Random.Range(0f, 1f));
        }

        Time.timeScale = 1;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnNow)
        {
            spawnNow = false;
            SpawnNow(numberOfRooms);
        }
    }
}
