using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{

    [SerializeField]
    List<EnemyBase> enemies = new List<EnemyBase>();

    [SerializeField]
    WorldSpawner level;

    //[SerializeField]
    //Enemy enemyPrefab;

    [SerializeField]
    CameraSystem cameraSystem;

    [SerializeField]
    int maxNumberOfEnemies = 1;

    Dictionary<string, EnemyBase> enemyPrefabs = new Dictionary<string, EnemyBase>();

    public void Initialize()
    {
        

        var prefabs = Resources.LoadAll("Prefabs/Enemies",typeof(GameObject));

        Debug.Log("Loaded " + prefabs.Length + " Enemy prefabs");
        foreach (GameObject g in prefabs)
        {
            if (g.GetComponent<EnemyBase>() != null)
            {
                Debug.Log(g.name + " Loaded");
                enemyPrefabs.Add(g.name, g.GetComponent<EnemyBase>());
            }
        }

        //GameObject tmp = Resources.Load("Prefabs/Enemies/Enemy") as GameObject;
        //enemyPrefab = tmp.GetComponent<Enemy>();

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
//        CurrentSpawner = SpawnALot;
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

    //float FastEnemySpawn()
    //{
    //    if (enemies.Count < maxNumberOfEnemies)
    //    {
    //        Vector3 desiredPos = RandomPointOnXYCircle(PlayerManager.Instance.AveragePlayersLocation, spawnDist);
    //        GridObject go = level.grid.GetGridObject(desiredPos);

    //        int tries = 0;
    //        while (go == null && tries < 5000)
    //        {
    //            tries++;
    //            desiredPos = RandomPointOnXYCircle(PlayerManager.Instance.AveragePlayersLocation, spawnDist);
    //            go = level.grid.GetGridObject(desiredPos);
    //        }

    //        if (go != null)
    //        {
    //            desiredPos = level.grid.GetWorldCenterPosition(desiredPos);
    //            if (level.grid.GetGridObject(desiredPos).Walkable)
    //            {
    //                AddEnemySpawner("Enemy - Fast (New)", desiredPos, 1, 0.1f, 1f); // Enemy - Fast
    //                CurrentSpawner = RandomSpawn;
    //            }
    //        }
    //        else
    //        {
    //            // We've randomly chosen a place outside of the level boundary. Don't do anything
    //            //Debug.LogError("No grid object defined at " + desiredPos);
    //        }
    //    }

    //    float retVal = 0.75f;
    //    return retVal;
    //}


    int _spawnAlotCounter = 0;

    float SpawnALot()
    {
        float retVal = 0f;

        if (_spawnAlotCounter < 6)
        {

            if (FKS.Utils.UtilsClass.TestChance(15))
            {
                RandomSpawn("Enemy - Fast (New)", 1);
            }
            else
            {
                RandomSpawn("Enemy - Normal (New)", Random.Range(2,6));
            }

            retVal = 0.1f;
        }
        else
        {
            _spawnAlotCounter = 0;
            retVal = 15f;
        }

        _spawnAlotCounter++;

        //Debug.LogFormat("SpawnALot: count={0} delay={1}", _spawnAlotCounter, retVal );

        return retVal;

    }


    void RandomSpawn(string enemyType, int numberOfEnemies)
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
                int index = Random.Range(0, availableSpots.Count);
                desiredPos = level.grid.GetWorldCenterPosition(availableSpots[index].X, availableSpots[index].Y);
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
                    //int numOfEnemies = Random.Range(2, 5);
                    AddEnemySpawner(enemyType, desiredPos, numberOfEnemies, 0.1f, 1f);
                }
            }
            else
            {
                // We've randomly chosen a place outside of the level boundary. Don't do anything
                Debug.LogError("No grid object defined at " + desiredPos);
            }
        }
    }

    delegate float SpawnerType();
    SpawnerType CurrentSpawner;

    IEnumerator ManageEnemies()
    {
        Debug.Log("Starting to manage the enemies");
        while (true)
        {
            yield return new WaitForSeconds(CurrentSpawner());

            //if (FKS.Utils.UtilsClass.TestChance(10) && CurrentSpawner != NoSpawn)
            //{
            //    CurrentSpawner = SpawnALot; //FastEnemySpawn;
            //}
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        GameManager.Instance.AddToTotalDamage(0);
    }


    public void AddEnemy(EnemyBase newEnemy)
    {
        enemies.Add(newEnemy);
        newEnemy.transform.parent = transform;
    }

    public void AddEnemySpawner(string prefabID, Vector3 desiredPos, int count, float spawnRate, float spawnDelay)
    {

        EnemyBase prefab = null;
        if (enemyPrefabs.TryGetValue(prefabID, out prefab))
        {
            EntitySpawner.Create(desiredPos, count, spawnRate, spawnDelay, prefab.gameObject, prefab.gameObject.GetComponent<SpriteRenderer>().color);
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
