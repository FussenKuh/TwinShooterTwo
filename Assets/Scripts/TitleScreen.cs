using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.Initialize();

//        Debug.Log(GameManager.Instance.LevelSettings.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Loading game...");
            GameManager.Instance.LoadLevel();

        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            PlayerManager.Instance.PlayerJoin();
        }
    }
}
