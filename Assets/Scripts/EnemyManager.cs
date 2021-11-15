using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{

    [SerializeField]
    List<Enemy> enemies = new List<Enemy>();

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

    Dictionary<string, Enemy> enemyPrefabs = new Dictionary<string, Enemy>();

    public void Initialize()
    {
        

        var prefabs = Resources.LoadAll("Prefabs/Enemies",typeof(GameObject));

        Debug.Log("Loaded " + prefabs.Length + " Enemy prefabs");
        foreach (GameObject g in prefabs)
        {
            Debug.Log(g.name + " Loaded");
            enemyPrefabs.Add(g.name, g.GetComponent<Enemy>());
        }

        GameObject tmp = Resources.Load("Prefabs/Enemies/Enemy") as GameObject;
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
        Messenger<Player>.AddListener("ExitedStartZone", OnExitedStartZone);
    }

    private void OnDestroy()
    {
        Messenger<WorldSpawner>.RemoveListener("WorldSpawned", OnWorldSpawned);
        Messenger<Player>.RemoveListener("ExitedStartZone", OnExitedStartZone);
    }

    void OnExitedStartZone(Player player)
    {
        CurrentSpawner = RandomSpawn;
    }

    void OnWorldSpawned(WorldSpawner w)
    {
        level = w;
        CurrentSpawner = NoSpawn;
        StartCoroutine(ManageEnemies());
    }


    Vector3 RandomPointOnXYCircle(Vector3 center, float radius)
    {
        float angle = Random.Range(0, 2f * Mathf.PI);
        return center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
    }

    

    float NoSpawn()
    {
        float retVal = 0.1f;

        return retVal;
    }

    [SerializeField]
    float spawnDist = 10;

    float FastEnemySpawn()
    {
        if (enemies.Count < maxNumberOfEnemies)
        {
            Vector3 desiredPos = RandomPointOnXYCircle(PlayerManager.Instance.AveragePlayersLocation, spawnDist);
            GridObject go = level.grid.GetGridObject(desiredPos);

            int tries = 0;
            while (go == null && tries < 5000)
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
                    AddEnemySpawner("Enemy - Fast", desiredPos, 1, 0.1f, 1f);
                    CurrentSpawner = RandomSpawn;
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


    float RandomSpawn()
    {
        if (enemies.Count < maxNumberOfEnemies)
        {

            // get list of available grid positions
            int x, y;
            level.grid.GetXY(PlayerManager.Instance.AveragePlayersLocation, out x, out y);
            List<GridObject> availableSpots = new List<GridObject>();

            for (int a = x - (int)spawnDist; a <= x + (int)spawnDist; a++)
            {
                for (int b = y - (int)spawnDist; b <= y + (int)spawnDist; b++)
                {

                    GridObject tmpGO = level.grid.GetGridObject(a, b);
                    if (tmpGO != null)
                    {
                        if (tmpGO.Walkable)
                        {
                            availableSpots.Add(tmpGO);
                           
                        }
                    }
                }
            }

            //foreach (GridObject g in availableSpots)
            //{
            //    Vector3 pos = level.grid.GetWorldCenterPosition(g.X, g.Y);
            //    var tmp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //    tmp.transform.position = pos;
            //    Destroy(tmp, 0.1f);
            //}

            Vector3 desiredPos = RandomPointOnXYCircle(PlayerManager.Instance.AveragePlayersLocation, spawnDist);
            // pick one
            if (availableSpots.Count > 0)
            {
                //Debug.Log("Found " + availableSpots.Count + " locations");
                desiredPos = level.grid.GetWorldCenterPosition(availableSpots[Random.Range(0, availableSpots.Count - 1)].X, availableSpots[Random.Range(0, availableSpots.Count - 1)].Y);
            }
            else
            {
                Debug.LogWarning("Didn't find any available spots on the grid. What's up with that?");
            }


            
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
                    int numOfEnemies = Random.Range(2, 5);
                    AddEnemySpawner("Enemy", desiredPos, numOfEnemies, 0.1f, 1f);
                }
            }
            else
            {
                // We've randomly chosen a place outside of the level boundary. Don't do anything
                Debug.LogError("No grid object defined at " + desiredPos);
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

            if (FKS.Utils.UtilsClass.TestChance(10) && CurrentSpawner != NoSpawn)
            {
                CurrentSpawner = FastEnemySpawn;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        GameManager.Instance.AddToTotalDamage(0);
    }


    public void AddEnemy(Enemy newEnemy)
    {
        enemies.Add(newEnemy);
        newEnemy.transform.parent = transform;
    }

    public void AddEnemySpawner(string prefabID, Vector3 desiredPos, int count, float spawnRate, float spawnDelay)
    {

        Enemy prefab = null;
        if (enemyPrefabs.TryGetValue(prefabID, out prefab))
        {
            EntitySpawner.Create(desiredPos, count, spawnRate, spawnDelay, prefab.gameObject, prefab.GetComponent<SpriteRenderer>().color);
        }
        else
        {
            Debug.LogWarning("Attempted to load '" + prefabID + "' but we couldn't find the prefab. Is it in the Resources\\Prefabs\\Enemies folder?");
        }
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
