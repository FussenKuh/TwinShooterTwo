using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FKS;

public class LineOfSight : MonoBehaviour {

    public Transform spawnPoint;
    public Transform target;
    public GameObject projectilePrefab;

    [Header("Projectile Parameters")]
    public float muzzleVelocity = 10f;
    public float traceDuration = 5f;
    public bool lobbed = false;

    [Header("Final Solution")]
    public float calculatedSolution = 0;

    LineRenderer lr;

    Vector3 mouseWorldLocation;

    Vector3[] points = { Vector3.zero, Vector3.zero };

    // Use this for initialization
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    Vector2 hitLoc;

    public void DrawLOS()
    {
        if (ProjectileUtils2D.LOS(spawnPoint,target, out hitLoc ))
        {
            points[0] = spawnPoint.transform.position;
            points[1] = hitLoc;
            lr.positionCount = points.Length;
            lr.SetPositions(points);
        }
        else
        {
            lr.positionCount = 0;
        }
    }

    // Update is called once per frame
    void Update () {

        DrawLOS();

        if (Input.GetMouseButton(0))
        {

            mouseWorldLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldLocation.z = 0;

            target.position = mouseWorldLocation;

            //if (ProjectileUtils2D.CalculateProjectileFiringSolution(out calculatedSolution, transform.position, mouseWorldLocation, muzzleVelocity, lobbed))
            //{

            //    transform.rotation = Quaternion.AngleAxis(calculatedSolution, Vector3.forward);

            //    pt.traceEnabled = true;
            //    pt.muzzleVelocity = muzzleVelocity;
            //}
            //else
            //{
            //    pt.traceEnabled = false;
            //}
        }
        //else if (Input.GetMouseButtonUp(0))
        //{
        //    if (ProjectileUtils2D.CalculateProjectileFiringSolution(out calculatedSolution, transform.position, mouseWorldLocation, muzzleVelocity, lobbed))
        //    {
        //        pt.traceEnabled = false;
        //        GameObject tmp = GameObject.Instantiate(projectilePrefab, transform);
        //        tmp.transform.parent = null;
        //        tmp.transform.localScale = projectilePrefab.transform.localScale;
        //        tmp.GetComponent<Rigidbody2D>().AddForce(tmp.transform.right * muzzleVelocity, ForceMode2D.Impulse);
        //        tmp.GetComponent<Rigidbody2D>().AddTorque(10);
        //        Destroy(tmp, 4f);
        //    }
        //}
	}
}
