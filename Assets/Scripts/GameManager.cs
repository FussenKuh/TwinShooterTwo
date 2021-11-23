using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    public int highScore = 0;
    public int currentScore = 0;

    public int highDamage = 0;
    public int currentDamage = 0;

    public enum GameStates { PAUSED, GAMEOVER, PLAYING, LOADING };

    //static bool managerCreated = false;

    [SerializeField]
    GameStates gameState = GameStates.GAMEOVER;

    [SerializeField]
    WorldSpawner.WorldSettings levelSettings = new WorldSpawner.WorldSettings();
    public WorldSpawner.WorldSettings LevelSettings { get { return levelSettings; } }

    float baseDamageToClearLevel = 500f;
    float remainingDamageToClearLevel;
    public float DamageToClearLevel { get { return remainingDamageToClearLevel; } set { remainingDamageToClearLevel = value; remainingDamageToClearLevel = Mathf.Max(0, remainingDamageToClearLevel); } }

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
        currentScore++;
        LoadLevel();
    }


    public void LevelLoaded(WorldSpawner level)
    {

        string upperLeftText = "Level <color=yellow>"
            + Instance.currentScore.ToString()
            + "</color>\nDamage <color=yellow>"
            + Instance.DamageToClearLevel.ToString() + "</color>";

        StatsOverlay.Instance.UpdateUpperLeftText(upperLeftText);
        StatsOverlay.Instance.UpdateMiddleText("");

        Instance.AddToTotalDamage(0);

        CurrentLevel = level;
        PlayerManager.Instance.ResetPlayers();
        PlayerManager.Instance.RelocatePlayers(CurrentLevel.StartPoint.transform);
    }

    public void LoadLevel()
    {
        //LevelSettings.worldWidth = Random.Range(50, 75);
        //LevelSettings.worldHeight = Random.Range(25, 50);
        LevelSettings.objectColors.Clear();
        LevelSettings.objectColors.Add(new Color(1, 1, 1));
        LevelSettings.objectColors.Add(new Color(.8f, .8f, .8f));
        LevelSettings.objectColors.Add(new Color(.6f, .6f, .6f));
        LevelSettings.objectColors.Add(new Color(.4f, .4f, .4f));
        LevelSettings.objectColors.Add(new Color(.2f, .2f, .2f));

        LevelSettings.worldWidth = Random.Range(75, 150);
        LevelSettings.worldHeight = Random.Range(75, 150);
        LevelSettings.objectPercentDesired = 25f;
        levelSettings.maxObjectSize = 20;
        levelSettings.minObjectSize = 5;

        remainingDamageToClearLevel =  baseDamageToClearLevel * (1 + currentScore/10f);

        PlayerManager.Instance.SetPlayerJoin(false);

        FKS.SceneUtilsVisuals.LoadScene("Level Scene");
    }

    public void GameOver()
    {
        if (PlayerManager.Instance.Players.Where(p => p.EntityInfo.Alive).ToArray().Length == 0)
        {
            if (currentScore > highScore)
            {
                highScore = currentScore;
            }

            if (currentDamage > highDamage)
            {
                highDamage = currentDamage;
            }

            currentScore = 0;
            currentDamage = 0;
            remainingDamageToClearLevel = 0;

            FKS.SceneUtilsVisuals.LoadScene("Title Scene");
        }
    }


    public void AddToTotalDamage(int dmg)
    {
        currentDamage += dmg;
        DamageToClearLevel -= dmg;

        string upperLeftText = "Level <color=yellow>"
            + Instance.currentScore.ToString()
            + "</color>\nDamage Needed <color=yellow>"
            + Instance.DamageToClearLevel.ToString() + "</color>";

        StatsOverlay.Instance.UpdateUpperLeftText(upperLeftText);
//        Debug.Log("Total Dmg: " + currentDamage);
    }

    // Update is called once per frame
    void Update()
    {

    }


}
