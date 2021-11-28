using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public string guid;
    public string uniqueName;
    public int onlineGameID = 1000;


    public int highScore = 0;
    public int currentScore = 0;

    public int highDamage = 0;
    public int currentDamage = 0;

    [SerializeField]
    WorldSpawner.WorldSettings levelSettings = new WorldSpawner.WorldSettings();
    public WorldSpawner.WorldSettings LevelSettings { get { return levelSettings; } }

    float baseDamageToClearLevel = 300f;
    float remainingDamageToClearLevel;
    public float DamageToClearLevel { get { return remainingDamageToClearLevel; } set { remainingDamageToClearLevel = value; remainingDamageToClearLevel = Mathf.Max(0, remainingDamageToClearLevel); } }

    [SerializeField]
    public WorldSpawner CurrentLevel { get; set; }

    [SerializeField]
    CameraSystem cameraSystem;

    void LoadPlayerInfo()
    {
        guid = PlayerPrefs.GetString("GUID", System.Guid.NewGuid().ToString());
        guid = guid.Replace("-", "");
        uniqueName = PlayerPrefs.GetString("UniqueName", UniqueNameGenerator.GenerateString(4));
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highDamage = PlayerPrefs.GetInt("HighDamage", 0);
    }

    void SavePlayerInfo()
    {
        PlayerPrefs.SetString("GUID", guid);
        PlayerPrefs.SetString("UniqueName", uniqueName);
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.SetInt("HighDamage", highDamage);

    }

    public void Initialize()
    {
        LoadPlayerInfo();
        SavePlayerInfo();

        //uniqueName = UniqueNameGenerator.GenerateString(4);

        cameraSystem = GameObject.Find("Main Camera System").GetComponent<CameraSystem>();
        PlayerManager.Instance.Initialize();

        StartCoroutine(StartMusic());
    }

    IEnumerator StartMusic()
    {
        yield return new WaitForSeconds(0.2f);
        if (!_musicPlaying)
        {
            _musicID = FKS.AudioManager.PlayMusic("Background Normal");
            _musicPlaying = true;
        }
    }


    public void ChangeMusicTempo(float tempo)
    {
        if (tempo > 1)
        {
            FKS.AudioManager.AdjustMusic(_musicID, 0.65f);
        }
        else
        {
            FKS.AudioManager.AdjustMusic(_musicID, 0.35f);
        }

        FKS.AudioManager.AdjustMusicTempo(_musicID, tempo);
    }

    public bool _musicPlaying = false;
    [Range(.8f,2f)]
    public float _musicTempo = 1;
    public int _musicID = -1;

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
        LevelSettings.objectColors.Add(Color.black);
        //LevelSettings.objectColors.Add(new Color(1, 1, 1));
        //LevelSettings.objectColors.Add(new Color(.8f, .8f, .8f));
        //LevelSettings.objectColors.Add(new Color(.6f, .6f, .6f));
        //LevelSettings.objectColors.Add(new Color(.4f, .4f, .4f));
        //LevelSettings.objectColors.Add(new Color(.2f, .2f, .2f));

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
            ChangeMusicTempo(1f);

            if (currentScore > highScore)
            {
                highScore = currentScore;
                FKS.OnlineScoreBoard _onlineScoreBoard = GameObject.FindObjectOfType<FKS.OnlineScoreBoard>();
                if (_onlineScoreBoard != null)
                {
                    _onlineScoreBoard.PostScore(new FKS.Score() { name = Instance.guid, score = highScore, data=Instance.uniqueName }, Instance.onlineGameID);
                }
            }

            if (currentDamage > highDamage)
            {
                highDamage = currentDamage;
            }

            currentScore = 0;
            currentDamage = 0;
            remainingDamageToClearLevel = 0;

            SavePlayerInfo();
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

    float _previousTempo = 1;

    // Update is called once per frame
    void Update()
    {
        if (_musicPlaying)
        {
            if (_previousTempo != _musicTempo)
            {
                _previousTempo = _musicTempo;
                ChangeMusicTempo(_musicTempo);
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        Debug.Log("Applicaiton Paused");
        FKS.AudioManager.PauseAllMusic();
    }

    private void OnApplicationFocus(bool focus)
    {
        Debug.Log("Application Resumed");
        FKS.AudioManager.UnpauseAllMusic();
    }
}
