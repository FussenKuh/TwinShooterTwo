using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FKS;

public class CalcSpawner : MonoBehaviour {

    public Transform spawnPoint;
    public ProjectileTracer pt;
    public GameObject projectilePrefab;

    [Header("Projectile Parameters")]
    public float muzzleVelocity = 10f;
    public float traceDuration = 5f;
    public bool lobbed = false;

    [Header("Final Solution")]
    public float calculatedSolution = 0;




    Vector3 mouseWorldLocation;

    // Update is called once per frame
    void Update () {
        pt.maxTime = traceDuration;

        if (Input.GetMouseButton(0))
        {

            mouseWorldLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldLocation.z = 0;

            if (ProjectileUtils2D.CalculateProjectileFiringSolution(out calculatedSolution, transform.position, mouseWorldLocation, muzzleVelocity, lobbed))
            {

                transform.rotation = Quaternion.AngleAxis(calculatedSolution, Vector3.forward);

                pt.traceEnabled = true;
                pt.muzzleVelocity = muzzleVelocity;
            }
            else
            {
                pt.traceEnabled = false;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (ProjectileUtils2D.CalculateProjectileFiringSolution(out calculatedSolution, transform.position, mouseWorldLocation, muzzleVelocity, lobbed))
            {
                pt.traceEnabled = false;
                GameObject tmp = GameObject.Instantiate(projectilePrefab, transform);
                tmp.transform.parent = null;
                tmp.transform.localScale = projectilePrefab.transform.localScale;
                tmp.GetComponent<Rigidbody2D>().AddForce(tmp.transform.right * muzzleVelocity, ForceMode2D.Impulse);
                tmp.GetComponent<Rigidbody2D>().AddTorque(10);
                Destroy(tmp, 4f);
            }
        }
	}
}
