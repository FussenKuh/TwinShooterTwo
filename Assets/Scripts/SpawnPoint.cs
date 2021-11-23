using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{

    [SerializeField]
    float _spawnRate = 0.75f;

    [SerializeField]
    bool _targetDetected = false;

    [SerializeField]
    float _timeSinceLastSpawn = 0;

    Dictionary<string, EnemyBase> _enemyPrefabs = new Dictionary<string, EnemyBase>();

    DetectTarget _detectTarget;


    [SerializeField]
    ParticleSystem _spawnParticles;
    ParticleSystem.EmissionModule _emissionSystem;
    float _emissionRate;

    void Initialize()
    {
        _spawnParticles = GetComponentInChildren<ParticleSystem>();
        _emissionSystem = _spawnParticles.emission;
        _emissionRate = _emissionSystem.rateOverTimeMultiplier;
        _emissionSystem.rateOverTimeMultiplier = 0;
        
        _detectTarget = GetComponentInChildren<DetectTarget>();
        _detectTarget.OnDetectTarget += OnDetectTarget;

        var prefabs = Resources.LoadAll("Prefabs/Enemies", typeof(GameObject));

        //Debug.Log("Loaded " + prefabs.Length + " Enemy prefabs");
        foreach (GameObject g in prefabs)
        {
            if (g.GetComponent<EnemyBase>() != null)
            {
                //Debug.Log(g.name + " Loaded");
                _enemyPrefabs.Add(g.name, g.GetComponent<EnemyBase>());
            }
        }
    }

    void OnDetectTarget(Transform t)
    {
        string n = "null";
        if (t != null) { n = t.name; }
        Debug.Log(n + " got a detected target: " + n);

        if (t == null)
        {
            // No target in our zone. Stop spawning
            _targetDetected = false;
        }
        else
        {
            // Target is in our zone. Start spawning
            _targetDetected = true;

            _emissionSystem.rateOverTimeMultiplier = _emissionRate;

            //if (_spawnParticles.is)
        }
    }

    public void AddEnemySpawner()
    {
        string prefabID;
        int count;
        Vector3 desiredPos = transform.position;
        float spawnRate = 0.1f;
        float spawnDelay = 0.5f;

        if (FKS.Utils.UtilsClass.TestChance(10))
        {
            // 10% of the time, spawn a "Fast" enemy
            prefabID = "Enemy - Fast (New)";
            count = 1;
        }
        else
        {
            // 90% of the time, spawn some "normal" enemies
            prefabID = "Enemy - Normal (New)";
            count = Random.Range(2, 6);
        }

        EnemyBase prefab = null;
        if (_enemyPrefabs.TryGetValue(prefabID, out prefab))
        {
            EntitySpawner es = EntitySpawner.Create(desiredPos, count, spawnRate, spawnDelay, prefab.gameObject, prefab.gameObject.GetComponent<SpriteRenderer>().color);
            Destroy(es, spawnDelay + (count * spawnRate) + 1); // Destroy the spawner after it's spewed all of its minions
        }
        else
        {
            Debug.LogWarning("Attempted to load '" + prefabID + "' but we couldn't find the prefab. Is it in the Resources\\Prefabs\\Enemies folder?");
        }
    }

    private void OnDestroy()
    {
        if (_detectTarget != null) { _detectTarget.OnDetectTarget -= OnDetectTarget; }
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        _timeSinceLastSpawn += Time.deltaTime;

        if (_targetDetected && (_timeSinceLastSpawn >= _spawnRate))
        {
            // Spawn some stuff
            _timeSinceLastSpawn = 0;
            AddEnemySpawner();
        }


    }
}
