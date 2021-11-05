using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{

    [SerializeField]
    Camera mainCamera;

    [SerializeField]
    Camera miniMapCamera;

    [SerializeField]
    CameraSystem cameraSystem;

    [SerializeField]
    GameObject miniMapCanvas;

    // Start is called before the first frame update
    void Start()
    {
        cameraSystem = GameObject.FindObjectOfType<CameraSystem>();

        mainCamera = Camera.main;
        miniMapCamera = GetComponentInChildren<Camera>();
    }

    public float Zoom { get { return miniMapCamera.orthographicSize; } set { miniMapCamera.orthographicSize = value; } }

    // Update is called once per frame
    void Update()
    {
        if (cameraSystem != null)
        {
            transform.position = cameraSystem.SystemVirtualCamera.transform.position;
        }
        else
        {
            transform.position = mainCamera.transform.position;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (miniMapCanvas != null)
            {
                miniMapCanvas.SetActive(!miniMapCanvas.activeSelf);
            }
        }

    }
}
