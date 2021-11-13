using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    SpriteRenderer sr;

    public WeaponData Weapon { get; set; }

    bool configured = false;

    public Color BulletTint
    {
        get
        {
            return sr.color;
        }
        set
        {
            sr.color = value;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EffectsManager.Instance.BulletHit(collision);
        if (collision.gameObject.GetComponent<Enemy>() != null)
        {
            collision.gameObject.GetComponent<Enemy>().Hit(collision, Weapon);
        }



        Destroy(gameObject);
    }

    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!configured)
        {
            configured = true;
            Destroy(gameObject, Weapon.Range / Weapon.Speed);
        }
    }
}
