using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    public enum GameStates { PAUSED, GAMEOVER, PLAYING, LOADING };

    //static bool managerCreated = false;

    [SerializeField]
    GameStates gameState = GameStates.GAMEOVER;

    [SerializeField]
    WorldSpawner.WorldSettings levelSettings = new WorldSpawner.WorldSettings();
    public WorldSpawner.WorldSettings LevelSettings { get { return levelSettings; } }

    [SerializeField]
    public WorldSpawner CurrentLevel { get; set; }

    [SerializeField]
    CameraSystem cameraSystem;

    public void Initialize()
    {
        cameraSystem = GameObject.Find("Main Camera System").GetComponent<CameraSystem>();
        PlayerManager.Instance.Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void LevelCompleted()
    {
        LoadLevel();
    }


    public void LevelLoaded(WorldSpawner level)
    {
        CurrentLevel = level;

        PlayerManager.Instance.RelocatePlayers(CurrentLevel.StartPoint.transform);
        
        //PlayerManager.Instance.SetPlayersActive(true);

    }

    public void LoadLevel()
    {
        //LevelSettings.worldWidth = Random.Range(50, 75);
        //LevelSettings.worldHeight = Random.Range(25, 50);
        LevelSettings.worldWidth = Random.Range(300, 400);
        LevelSettings.worldHeight = Random.Range(300, 400);
        //        cameraSystem.CameraBounds = new Vector3(levelSettings.worldWidth + levelSettings.borderWallThickness, levelSettings.worldHeight + levelSettings.borderWallThickness, 1);

        //PlayerManager.Instance.SetPlayersActive(false);

        FKS.SceneUtils.LoadScene("Level Scene");
    }

    // Update is called once per frame
    void Update()
    {

    }


}
