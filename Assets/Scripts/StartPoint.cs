using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MonoBehaviour
{

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            Debug.Log(collision.name + " left the Start Zone.");
            Messenger<Player>.Broadcast("ExitedStartZone", collision.gameObject.GetComponent<Player>(), MessengerMode.DONT_REQUIRE_LISTENER);
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
