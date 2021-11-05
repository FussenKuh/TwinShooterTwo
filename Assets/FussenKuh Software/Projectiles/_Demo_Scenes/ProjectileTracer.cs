using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FKS;

[RequireComponent(typeof(LineRenderer))]
public class ProjectileTracer : MonoBehaviour {

    LineRenderer lr;

    public float maxTime = 5.0f;
    public float muzzleVelocity;
    public bool traceEnabled;

	// Use this for initialization
	void Start () {
        lr = GetComponent<LineRenderer>();
	}

    List<Vector3> tmpProjectilePoints = new List<Vector3>();

    public void DrawTracer()
    {
        if (traceEnabled)
        {
            tmpProjectilePoints = ProjectileUtils2D.TraceProjectile(muzzleVelocity, transform, maxTime);
            lr.positionCount = tmpProjectilePoints.Count;
            lr.SetPositions(tmpProjectilePoints.ToArray());
        }
        else
        {
            lr.positionCount = 0;
        }
    }

	// Update is called once per frame
	void Update () {
        DrawTracer();
	}
}
