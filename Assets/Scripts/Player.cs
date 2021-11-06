using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    static int uniqueID = 0;

    public int MyID { get; set; }

    [SerializeField]
    PlayerInputHandler playerInputHandler;

    [SerializeField]
    bool atGoal = false;

    private void Awake()
    {
        Messenger<bool, Player>.AddListener("PlayerAtGoal", OnPlayerAtGoal);

        MyID = uniqueID;
        uniqueID++;
    }

    private void OnDestroy()
    {
        Messenger<bool, Player>.RemoveListener("PlayerAtGoal", OnPlayerAtGoal);
    }

    void OnPlayerAtGoal(bool goalReached, Player playerID)
    {
        if (playerID.MyID == MyID)
        {
            atGoal = goalReached;
        }
    }


    void OnFire(object sender, PlayerInputHandler.OnButtonArgs args)
    {
        if (atGoal && args.Phase == UnityEngine.InputSystem.InputActionPhase.Started)
        {
            atGoal = false;
            GameManager.Instance.LevelCompleted();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        playerInputHandler = GetComponent<PlayerInputHandler>();
        if (playerInputHandler == null)
        {
            Debug.LogWarning("Player - Couldn't find a PlayerInputHandler reference");
            playerInputHandler = null;
        }
        else
        {
            playerInputHandler.FireEvent += OnFire;
        }



    }

    // Update is called once per frame
    void Update()
    {
        //if (atGoal & Input.GetKeyDown(KeyCode.G))
        //{

        //    atGoal = false;

        //    GameManager.Instance.LevelCompleted();
        //}
        
    }
}

