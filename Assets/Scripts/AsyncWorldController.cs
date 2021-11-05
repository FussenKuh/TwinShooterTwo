using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsyncWorldController : MonoBehaviour
{

    // Pause game
    // Fade out camera
    // Spawn world (objects, collectables, enemies)
    // Spawn player(s) at start point
    // Fade in camera
    // Unpause game


    public bool testSpawnWorld = false;

    public WorldSpawner worldSpawner = null;

    [SerializeField]
    CameraSystem cameraSystem;

    private void Awake()
    {
        Messenger<WorldSpawner.WorldSettings>.AddListener("SpawnWorld", OnSpawnWorld);
    }


    void OnSpawnWorld(WorldSpawner.WorldSettings settings)
    {
        if (worldSpawner == null) { return; }

        Debug.Log(settings.ToString());
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
            if (cameraSystem != null)
            {
                cameraSystem.SetCameraBounds(e.World.Center, e.World.WorldDimensions);
            }

            GameManager.Instance.LevelLoaded(e.World);
            Messenger<WorldSpawner>.Broadcast("WorldSpawned", e.World, MessengerMode.DONT_REQUIRE_LISTENER);
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

        cameraSystem = GameObject.Find("Main Camera System").GetComponent<CameraSystem>();

        if (cameraSystem == null)
        {
            Debug.LogError("AsyncWorldController: Cannot locate the Main Camera System");
        }


        worldSpawner.OnLevelReadyEvent += OnLevelReadyEvent;

        OnSpawnWorld(GameManager.Instance.LevelSettings);

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

    }
}
