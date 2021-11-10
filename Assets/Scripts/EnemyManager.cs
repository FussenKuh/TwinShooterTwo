using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{

    [SerializeField]
    List<Enemy> enemies = new List<Enemy>();
    public List<Enemy> Enemy
    {
        get { return enemies; }
    }

    [SerializeField]
    WorldSpawner level;

    [SerializeField]
    Enemy enemyPrefab;

    [SerializeField]
    float spawnDelay = 0.5f;

    [SerializeField]
    CameraSystem cameraSystem;

    [SerializeField]
    int maxNumberOfEnemies = 1;

    public void Initialize()
    {
        GameObject tmp = Resources.Load("Prefabs/Enemy") as GameObject;
        enemyPrefab = tmp.GetComponent<Enemy>();

        cameraSystem = GameObject.Find("Main Camera System").GetComponent<CameraSystem>();

        if (cameraSystem == null)
        {
            Debug.LogError("EnemyManager: Cannot locate the Main Camera System");
        }
        else
        {
            DontDestroyOnLoad(cameraSystem.gameObject);
        }
    }

    private void Awake()
    {
        Messenger<WorldSpawner>.AddListener("WorldSpawned", OnWorldSpawned);
    }

    private void OnDestroy()
    {
        Messenger<WorldSpawner>.RemoveListener("WorldSpawned", OnWorldSpawned);
    }

    void OnWorldSpawned(WorldSpawner w)
    {
        level = w;
        CurrentSpawner = RandomSpawn;
        StartCoroutine(ManageEnemies());
    }


    Vector3 RandomPointOnXYCircle(Vector3 center, float radius)
    {
        float angle = Random.Range(0, 2f * Mathf.PI);
        return center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
    }

    float OriginalRandomSpawn()
    {
        if (enemies.Count < maxNumberOfEnemies && cameraSystem != null)
        {
            Bounds cameraBounds = cameraSystem.CalculatedBounds;

            Vector3 desiredPos = new Vector3(Random.Range(-cameraBounds.extents.x, cameraBounds.extents.x), Random.Range(-cameraBounds.extents.y, cameraBounds.extents.y), 0);
            desiredPos += cameraBounds.center;
            desiredPos.z = 0;

            desiredPos.x = (int)desiredPos.x + 0.5f;
            desiredPos.y = (int)desiredPos.y + 0.5f;

            GridObject go = level.grid.GetGridObject(desiredPos);

            if (go != null)
            {
                if (level.grid.GetGridObject(desiredPos).Walkable)
                {
                    AddEnemy(desiredPos);
                }
            }
            else
            {
                // We've randomly chosen a place outside of the level boundary. Don't do anything
                //Debug.LogError("No grid object defined at " + desiredPos);
            }
        }

        float retVal = 0.1f;
        return retVal;
    }

    float NoSpawn()
    {
        float retVal = 0.1f;

        return retVal;
    }

    [SerializeField]
    float spawnDist = 10;

    float RandomSpawn()
    {
        if (enemies.Count < maxNumberOfEnemies)
        {
            Vector3 desiredPos = RandomPointOnXYCircle(PlayerManager.Instance.AveragePlayersLocation, spawnDist);
            GridObject go = level.grid.GetGridObject(desiredPos);

            int tries = 0;
            while (go == null && tries < 1000)
            {
                tries++;
                desiredPos = RandomPointOnXYCircle(PlayerManager.Instance.AveragePlayersLocation, spawnDist);
                go = level.grid.GetGridObject(desiredPos);
            }

            

            if (go != null)
            {
                desiredPos = level.grid.GetWorldCenterPosition(desiredPos);
                if (level.grid.GetGridObject(desiredPos).Walkable)
                {
                    int max = Random.Range(4, 10);
                    for (int i = 0; i < max; i++)
                    {
                        Vector3 offset = Random.insideUnitCircle;
                        AddEnemy(desiredPos + offset);
                    }
                }
            }
            else
            {
                // We've randomly chosen a place outside of the level boundary. Don't do anything
                //Debug.LogError("No grid object defined at " + desiredPos);
            }
        }

        float retVal = 0.75f;
        return retVal;
    }

    delegate float SpawnerType();
    SpawnerType CurrentSpawner;

    IEnumerator ManageEnemies()
    {
        Debug.Log("Starting to manage the enemies");
        while (true)
        {
            yield return new WaitForSeconds(CurrentSpawner());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public void AddEnemy(Vector3 desiredPos)
    {
        
        enemies.Add(GameObject.Instantiate(enemyPrefab.gameObject, desiredPos, enemyPrefab.gameObject.transform.rotation).GetComponent<Enemy>());
        enemies[enemies.Count - 1].transform.parent = transform;

        //GridObject go = level.grid.GetGridObject(desiredPos);
        //go.Occupied = true;

    }

    public void Kill(GameObject objectToKill)
    {
        if (enemies.Count == 0) { return; }

        int count = 0;
        while (count < enemies.Count && enemies[count].gameObject != objectToKill)
        {
            count++;
        }

        if (count < enemies.Count)
        {
            Destroy(enemies[count].gameObject);
            enemies.RemoveAt(count);
            
            
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
