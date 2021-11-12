using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : Singleton<EffectsManager>
{
    [SerializeField]
    ParticleSystem bulletHit;
    [SerializeField]
    ParticleSystem bulletShell;



    public void Initialize()
    {

    }

    private void Awake()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    public void BulletHit(Collision2D c)
    {
        //var main = bulletHit.main; 
        //ParticleSystem.MinMaxGradient tmpColor = bulletHit.main.startColor;
        //tmpColor.colorMin = c.otherCollider.GetComponent<SpriteRenderer>().color;
        //tmpColor.colorMax = c.collider.GetComponent<SpriteRenderer>().color;
        //main.startColor = tmpColor;

        bulletHit.transform.position = c.contacts[0].point;
        bulletHit.transform.rotation = Quaternion.Euler(c.contacts[0].normal);

        var main = bulletHit.main;
        ParticleSystem.MinMaxGradient tmpColor = bulletHit.main.startColor;
        tmpColor.colorMin = c.otherCollider.GetComponent<SpriteRenderer>().color;
        tmpColor.colorMax = c.otherCollider.GetComponent<SpriteRenderer>().color;
        main.startColor = tmpColor;
        bulletHit.Emit(1);
        tmpColor.colorMin = c.collider.GetComponent<SpriteRenderer>().color;
        tmpColor.colorMax = c.collider.GetComponent<SpriteRenderer>().color;
        main.startColor = tmpColor;
        bulletHit.Emit(Random.Range(3, 6));

    }

    public float offset = 0;
    public void BulletShell(Vector3 location, Quaternion rotation)
    {

        var main = bulletShell.main;
        main.startRotation = (-rotation.eulerAngles.z) * Mathf.Deg2Rad;

        bulletShell.transform.position = location;
        bulletShell.transform.rotation = rotation;

        bulletShell.Emit(1);
    }


    Vector3 RandomPointOnXYCircle(Vector3 center, float radius)
    {
        float angle = Random.Range(0, 2f * Mathf.PI);
        return center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
    }

    


    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }



    // Update is called once per frame
    void Update()
    {

    }
}
