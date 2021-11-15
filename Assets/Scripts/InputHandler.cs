using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{

    [SerializeField]
    PlayerInput _playerInput;

    [SerializeField]
    InputData _data;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        if (_playerInput == null)
        {
            Debug.LogError(name + ": Can't find a PlayerInput");
        }
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.started)  { _data.MoveActive = true;  }
        if (context.canceled) { _data.MoveActive = false; }

        _data.MoveVector = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.started) { _data.LookActive = true; }
        if (context.canceled) { _data.LookActive = false; }

        _data.LookVector = context.ReadValue<Vector2>();

        if (_playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            Vector3 tmp = Camera.main.ScreenToWorldPoint(_data.LookVector);
            tmp.z = 0;
            _data.LookVector = (tmp - transform.position).normalized;
        }

    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started) { _data.FireActive = true; }
        if (context.canceled) { _data.FireActive = false; }
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (context.started) { _data.UseActive = true; }
        if (context.canceled) { _data.UseActive = false; }
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
