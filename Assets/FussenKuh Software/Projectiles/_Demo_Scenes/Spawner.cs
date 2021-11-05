using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FKS;

public class Spawner : MonoBehaviour {

    public Transform spawnPoint;

    Vector3 mouseWorldLocation;
    public float muzzleVelocity = 10f;
    public float traceDuration = 5f;
    public ProjectileTracer pt;
    public GameObject projectilePrefab;


    public float velocityScaleFactor = 2f;

	// Update is called once per frame
	void Update () {
        pt.maxTime = traceDuration;

        if (Input.GetMouseButton(0))
        {
            var pos = Camera.main.WorldToScreenPoint(transform.position);
            var dir = Input.mousePosition - pos;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            mouseWorldLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldLocation.z = 0;

            pt.traceEnabled = true;
            muzzleVelocity = Vector3.Distance(mouseWorldLocation, transform.position) * velocityScaleFactor;
            pt.muzzleVelocity = muzzleVelocity;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            pt.traceEnabled = false;
            GameObject tmp = GameObject.Instantiate(projectilePrefab,transform);
            tmp.transform.parent = null;
            tmp.transform.localScale = projectilePrefab.transform.localScale;
            tmp.GetComponent<Rigidbody2D>().AddForce(tmp.transform.right * muzzleVelocity,ForceMode2D.Impulse);
            tmp.GetComponent<Rigidbody2D>().AddTorque(10);
            Destroy(tmp, 4f);
        }
	}
}
