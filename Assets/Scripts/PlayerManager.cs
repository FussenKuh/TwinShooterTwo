using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    PlayerInputManager pim;

    [SerializeField]
    int maxPlayers = 2;

    /// <summary>
    /// The average location of all players. i.e. Add all player position vectors and divide by number of players
    /// </summary>
    public Vector3 AveragePlayersLocation
    {
        get 
        {
            if (players.Count == 0)
            {
                Debug.LogWarning("PlayerManager - Calling AveragePlayerLocation with no players. Returning Vector.zero");
                return Vector3.zero;
            }

            Vector3 retVal = Vector3.zero;
            foreach (Player p in players)
            {
                retVal += p.transform.position;
            }

            retVal /= players.Count;
            return retVal; 
        }
    }


    void OnPlayerJoined(PlayerInput playerInput)
    {

        //pim.DisableJoining();

        Debug.Log("Player " + playerInput.playerIndex + " Joined.");

        playerInput.transform.parent = gameObject.transform;
        playerInput.transform.localPosition = Vector3.zero;
        playerInput.name = "Player " + playerInput.playerIndex;

        players.Add(playerInput.gameObject.GetComponent<Player>());
        players[players.Count - 1].MyID = playerInput.playerIndex;

        Vector3 pos = cameraSystem.SystemCamera.transform.position;
        pos.z = 0;
        if (players.Count > 1)
        {
            pos = players[0].transform.position + Vector3.up;
        }

        players[players.Count - 1].transform.position = pos;
        cameraSystem.AddFollowTarget(players[players.Count - 1].transform);

        EffectsManager.Instance.EntitySpawn(pos, playerInput.GetComponent<SpriteRenderer>().color);

        StartCoroutine(DelayWelcome(pos, "Welcome " + playerInput.name, false, 0f));

        if (players.Count >= maxPlayers)
        {
            pim.DisableJoining();
        }
    }

    void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.LogWarning("Player " + playerInput.playerIndex + " Left. We're currently not doing anything about this!");

        //players.Add(playerInput.gameObject.GetComponent<Player>());
        //cameraSystem.AddFollowTarget(players[players.Count - 1].transform);
    }


    public void Initialize()
    {
        Debug.Log("PlayerManager - Calling Initialize");

        cameraSystem = GameObject.Find("Main Camera System").GetComponent<CameraSystem>();
        if (cameraSystem == null)
        {
            Debug.LogError("PlayerManager: Cannot locate the Main Camera System");
        }
        else
        {
            DontDestroyOnLoad(cameraSystem.gameObject);
        }

        GameObject tmp = Resources.Load("Prefabs/Player") as GameObject;
        playerPrefab = tmp.GetComponent<Player>();

        pim = gameObject.AddComponent<PlayerInputManager>();
        pim.playerPrefab = Resources.Load("Prefabs/Player") as GameObject;
        pim.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
        pim.onPlayerJoined += OnPlayerJoined;
        pim.onPlayerLeft += OnPlayerLeft;

    }

    private void OnDestroy()
    {
        pim.onPlayerJoined -= OnPlayerJoined;
        pim.onPlayerLeft -= OnPlayerLeft;
    }

    // Start is called before the first frame update
    void Start()
    {

    }


    IEnumerator DelayWelcome(Vector3 position, string message, bool critical, float delay = 1f)
    {
        Debug.Log("Delaying welcome");
        yield return new WaitForSeconds(delay);
        MessagePopup.Create(position, message, critical: false, duration: 1f);
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
                UnityEngine.Random.Range(-trans.localScale.x * 0.30f, trans.localScale.x * 0.30f), UnityEngine.Random.Range(-trans.localScale.y * 0.30f, trans.localScale.y * 0.30f), 0) + trans.position;
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
