using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{

    // Pause game
    // Fade out camera
    // Spawn world (objects, collectables, enemies)
    // Spawn player(s) at start point
    // Fade in camera
    // Unpause game


    public bool testSpawnWorld = false;

    public WorldSpawner worldSpawner = null;

    public GameObject playerPrefab;
    public Cinemachine.CinemachineVirtualCamera cm;

    public GameObject activePlayer;

    public Follow miniMap;

    private void Awake()
    {
        Messenger<WorldSpawner.WorldSettings>.AddListener("SpawnWorld", OnSpawnWorld);

    }


    void OnSpawnWorld(WorldSpawner.WorldSettings settings)
    {
        if (worldSpawner == null) { return; }

        if (activePlayer != null)
        {
            activePlayer.SetActive(false);
        }

        worldSpawner.SpawnWorld(settings);
    }


    private void OnLevelReadyEvent(object sender, WorldSpawner.LevelReadyArgs e)
    {
        if (e.Error)
        {

            Debug.LogError("There was a problem loading the world");
        }
        else
        {
            Messenger<WorldSpawner>.Broadcast("WorldSpawned", e.World, MessengerMode.DONT_REQUIRE_LISTENER);


            if (activePlayer == null)
            {
                activePlayer = GameObject.Instantiate(playerPrefab, worldSpawner.StartPoint.transform.position, playerPrefab.transform.rotation);
                cm.Follow = activePlayer.transform;
                miniMap.itemToFollow = activePlayer.transform;
            }
            else
            {
                activePlayer.transform.position = worldSpawner.StartPoint.transform.position;
                activePlayer.SetActive(true);
            }


        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (worldSpawner == null)
        {
            Debug.LogError("WorldController: worldSpawner reference is not defined. No levels will be spawned!");
            worldSpawner = null;
            return;
        }

        worldSpawner.OnLevelReadyEvent += OnLevelReadyEvent;
    }

    private void OnDestroy()
    {
        if (worldSpawner != null)
        {
            worldSpawner.OnLevelReadyEvent -= OnLevelReadyEvent;
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            testSpawnWorld = true;
        }

        if (testSpawnWorld)
        {
            testSpawnWorld = false;
            Messenger<WorldSpawner.WorldSettings>.Broadcast("SpawnWorld", new WorldSpawner.WorldSettings(), MessengerMode.DONT_REQUIRE_LISTENER);
        }
    }
}
