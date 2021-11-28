using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMonitor : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D _rigidBody;

    [SerializeField]
    float _lastReceivedInputTime;

    [SerializeField]
    int _numberOfPlayers;

    [SerializeField]
    PlayerInputHandler _playerInputHandler;

    [SerializeField]
    bool _playerNotMoving = true;

    [SerializeField]
    float _playerNotMovingThreshold = 1f;

    void OnMove(object sender, PlayerInputHandler.OnDirectionArgs args)
    {
        if (args.Context.canceled)
        {
            _lastReceivedInputTime = Time.time;
            StartCoroutine(CountDown());
        }
        else if (args.Context.started)
        {
            _playerNotMoving = false;
            StopAllCoroutines();
        }
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(_playerNotMovingThreshold);

        _playerNotMoving = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerInputHandler.MoveEvent += OnMove;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
