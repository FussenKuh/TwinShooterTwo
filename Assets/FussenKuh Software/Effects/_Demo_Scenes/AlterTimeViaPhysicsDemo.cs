using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlterTimeViaPhysicsDemo : MonoBehaviour
{
    public float timeScale = 0.5f;
    public Text timescaleLabel;

    public GameObject spawnedObjectCollisionPrefab;
    public GameObject spawnedObjectTriggerPrefab;


    // Start is called before the first frame update
    void Start()
    {

    }

    void SpawnObject(Vector3 position, GameObject prefab)
    {
        GameObject tmp = Instantiate(prefab);
        tmp.transform.position = position;
        tmp.transform.Rotate(new Vector3(0, 0, 45));
        Destroy(tmp, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        timescaleLabel.text = "TimeScale: " + Time.timeScale.ToString("N2");

        // Left click to drop an object (collision test)
        if (Input.GetMouseButtonUp(0))
        {
            SpawnObject(new Vector3(-1.8f, 2, 0), spawnedObjectCollisionPrefab);
        }
        // Right click to drop a different object (trigger test)
        if (Input.GetMouseButtonUp(1))
        {
            SpawnObject(new Vector3(1.8f, 2, 0), spawnedObjectTriggerPrefab);
        }

    }
}
