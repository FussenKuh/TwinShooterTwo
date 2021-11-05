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
        StartCoroutine(ManageEnemies());
    }


    IEnumerator ManageEnemies()
    {
        Bounds cameraBounds;

        Debug.Log("Starting to manage the enemies");
        while (true)
        {
            if (enemies.Count < maxNumberOfEnemies && cameraSystem != null)
            {
                cameraBounds = cameraSystem.CalculatedBounds;

                Vector3 desiredPos = new Vector3(Random.Range(-cameraBounds.extents.x, cameraBounds.extents.x), Random.Range(-cameraBounds.extents.y, cameraBounds.extents.y), 0);
                desiredPos += cameraBounds.center;
                desiredPos.z = 0;

                desiredPos.x = (int)desiredPos.x + 0.5f;
                desiredPos.y = (int)desiredPos.y + 0.5f;

                if (level.grid.GetGridObject(desiredPos))
                {

                    AddEnemy(desiredPos);

                }
            }

            yield return new WaitForSeconds(spawnDelay);
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

        level.grid.SetGridObject(desiredPos, false);

    }

    public void RelocateEnemies(Vector3 location)
    {
        //foreach (Player p in players)
        //{
        //    p.transform.position = location;
        //}
    }

    public void RelocateEnemies(Transform trans)
    {
        //foreach (Player p in players)
        //{
        //    Vector3 pos = new Vector3(
        //        Random.Range(-trans.localScale.x * 0.30f, trans.localScale.x * 0.30f), Random.Range(-trans.localScale.y * 0.30f, trans.localScale.y * 0.30f), 0) + trans.position;
        //    p.transform.position = pos;
        //}
    }

    public void SetEnemiesActive(bool status)
    {
        foreach (Enemy e in enemies)
        {
            e.gameObject.SetActive(status);
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
