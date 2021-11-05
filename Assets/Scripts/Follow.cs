using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{

    public List<int> zoomLevels = new List<int>() { 100, 60, 40 };
    public int zoomLevel = 0;

    public Transform itemToFollow;

    public Vector3 followPosition;

    public Camera c;

    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<Camera>();

        AdjustZoom();
    }

    void AdjustZoom()
    {
        zoomLevel = zoomLevel % zoomLevels.Count;

        c.orthographicSize = zoomLevels[zoomLevel];
    }

    // Update is called once per frame
    void Update()
    {
        if (itemToFollow != null)
        {
            followPosition = itemToFollow.position;
            followPosition.z = -10;
            transform.position = followPosition;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            zoomLevel++;
            AdjustZoom();
        }
        
    }
}
