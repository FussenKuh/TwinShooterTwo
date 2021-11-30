using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : Singleton<EffectsManager>
{
    [SerializeField]
    ParticleSystem bulletHit;
    [SerializeField]
    ParticleSystem bulletShell;
    [SerializeField]
    ParticleSystem enemyDeath;
    [SerializeField]
    ParticleSystem entitySpawn;



    public void Initialize()
    {

    }

    private void Awake()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    public void BulletHit(DetectDamagingCollision.DamageInfo c)
    {
        bulletHit.transform.position = c.location;
        bulletHit.transform.rotation = Quaternion.Euler(c.normal);

        var main = bulletHit.main;
        ParticleSystem.MinMaxGradient tmpColor = bulletHit.main.startColor;

        tmpColor.colorMin = c.entity.GetComponent<SpriteRenderer>().color;
        tmpColor.colorMax = c.entity.GetComponent<SpriteRenderer>().color;
        main.startColor = tmpColor;
        bulletHit.Emit(Random.Range(6, 10));

    }


    public void EnemyDeath(Vector3 location, Color color)
    {
        var main = enemyDeath.main;
        ParticleSystem.MinMaxGradient tmpColor = enemyDeath.main.startColor;

        tmpColor.colorMin = color;
        tmpColor.colorMax = new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f);
        main.startColor = tmpColor;

        enemyDeath.transform.position = location;
        enemyDeath.Emit(Random.Range(25, 35));
    }

    public void EntitySpawn(Vector3 location, Color color)
    {
        var main = entitySpawn.main;
        ParticleSystem.MinMaxGradient tmpColor = entitySpawn.main.startColor;

        tmpColor.colorMin = color;
        tmpColor.colorMax = new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f);
        main.startColor = tmpColor;

        entitySpawn.transform.position = location;
        entitySpawn.Play();
    }

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
