using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBox : MonoBehaviour {

    public Transform box;

    Vector3 startPosition = Vector3.zero;
    Vector3 endPosition = Vector3.zero;
    Vector3 tmpPosition = Vector3.zero;

    // Use this for initialization
    void Start () {
        startPosition = box.position;
        endPosition = startPosition;
        endPosition.x = -endPosition.x; 
	}

    float mult = 1;

	// Update is called once per frame
	void Update () {
        tmpPosition = box.position;

        if (tmpPosition.x > endPosition.x)   { mult = -1; }
        if (tmpPosition.x < startPosition.x) { mult =  1; }
        tmpPosition.x += (Time.deltaTime * mult);

        box.position = tmpPosition;

	}
}
