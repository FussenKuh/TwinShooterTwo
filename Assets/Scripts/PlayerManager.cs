using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{

    [SerializeField]
    List<Player> players = new List<Player>();
    public List<Player> Players
    {
        get { return players; }
    }

    [SerializeField]
    Player playerPrefab;

    [SerializeField]
    CameraSystem cameraSystem;

    public void Initialize()
    {
        GameObject tmp = Resources.Load("Prefabs/Player") as GameObject;
        playerPrefab = tmp.GetComponent<Player>();

        cameraSystem = GameObject.Find("Main Camera System").GetComponent<CameraSystem>();

        if (cameraSystem == null)
        {
            Debug.LogError("PlayerManager: Cannot locate the Main Camera System");
        }
        else
        {
            DontDestroyOnLoad(cameraSystem.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void PlayerJoin()
    {
        if (players.Count == 0)
        {
            Debug.Log("Adding player to game");
            players.Add(GameObject.Instantiate(playerPrefab.gameObject, gameObject.transform).GetComponent<Player>());
            cameraSystem.AddFollowTarget(players[players.Count - 1].transform);
        }
    }

    public void RelocatePlayers(Vector3 location)
    {
        foreach (Player p in players)
        {
            p.transform.position = location;
        }
    }

    public void RelocatePlayers(Transform trans)
    {
        foreach (Player p in players)
        {
            Vector3 pos = new Vector3(
                Random.Range(-trans.localScale.x * 0.30f, trans.localScale.x * 0.30f), Random.Range(-trans.localScale.y * 0.30f, trans.localScale.y * 0.30f), 0) + trans.position;
            p.transform.position = pos;
        }
    }

    public void SetPlayersActive(bool status)
    {
        foreach (Player p in players)
        {
            p.gameObject.SetActive(status);
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
