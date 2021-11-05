using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    static int uniqueID = 0;

    int myID = 0;
    int MyID { get; }

    [SerializeField]
    bool atGoal = false;

    private void Awake()
    {
        Messenger<bool, Player>.AddListener("PlayerAtGoal", OnPlayerAtGoal);

        myID = uniqueID;
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
            if (goalReached)
            {
                atGoal = true;
                Debug.Log("I'm at the goal! (" + atGoal + ")");
            }
            else
            {
                Debug.Log("I've left the goal :(");
                atGoal = false;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (atGoal & Input.GetKeyDown(KeyCode.G))
        {

            atGoal = false;

            GameManager.Instance.LevelCompleted();
//            Messenger<WorldSpawner.WorldSettings>.Broadcast("SpawnWorld", new WorldSpawner.WorldSettings());
        }
        
    }
}
