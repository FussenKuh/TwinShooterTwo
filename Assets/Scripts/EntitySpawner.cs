using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    ParticleSystem ps;

    public GameObject prefab;


    public int entitesLeftToSpawn = 10;
    public float spawnRate = 0.5f;

    float timer = 0;


    public static EntitySpawner Create(Vector3 position, int entityCount, float rate, float spawnDelay, GameObject obj, Color particleColor)
    {
        GameObject go = Resources.Load("Prefabs/EntitySpawner") as GameObject;
        EntitySpawner retVal = Instantiate(go, position, Quaternion.identity).GetComponent<EntitySpawner>();

        retVal.Setup(entityCount, rate, spawnDelay, obj, particleColor);
        return retVal;
    }


    void Setup(int entityCount, float rate, float spawnDelay, GameObject obj, Color particleColor)
    {
        prefab = obj;
        spawnRate = rate;
        entitesLeftToSpawn = entityCount;
        timer = spawnDelay;

        var main = ps.main;
        ParticleSystem.MinMaxGradient tmpColor = main.startColor;

        tmpColor.colorMin = particleColor;
        tmpColor.colorMax = new Color(particleColor.r * 0.5f, particleColor.g * 0.5f, particleColor.b * 0.5f);
        main.startColor = tmpColor;

        ps.Play();
    }


    private void Start()
    {
        // For Testing // Setup(10, .5f, prefab, Color.yellow);
    }


    void Awake()
    {
        ps = gameObject.GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = spawnRate;
            if (entitesLeftToSpawn > 0)
            {
                entitesLeftToSpawn--;
                GameObject.Instantiate(prefab, RandomPointOnXYCircle(transform.position, 0.5f), prefab.transform.rotation);
            }
            else
            {
                var em = ps.emission;
                em.rateOverTime = 0;
                Destroy(gameObject, 5f);
            }

        }
    }


    Vector3 RandomPointOnXYCircle(Vector3 center, float radius)
    {
        float angle = Random.Range(0, 2f * Mathf.PI);
        return center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
    }
}
