using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPoint : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            Messenger<bool, Player>.Broadcast("PlayerAtGoal", true, collision.GetComponent<Player>(), MessengerMode.DONT_REQUIRE_LISTENER);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            Messenger<bool, Player>.Broadcast("PlayerAtGoal", false, collision.GetComponent<Player>(), MessengerMode.DONT_REQUIRE_LISTENER);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
