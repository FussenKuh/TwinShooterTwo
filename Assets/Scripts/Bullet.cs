using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    SpriteRenderer sr;

    public GameObject explosionPrefab;

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


        if (Weapon.DamageRadius > 0)
        {
            //GameObject explosion = GameObject.Instantiate(explosionPrefab, collision.contacts[0].point, Quaternion.identity);
            //explosion.transform.localScale = new Vector3(Weapon.DamageRadius, Weapon.DamageRadius, 1);
            //Destroy(explosion, 1);
            var hits = Physics2D.OverlapCircleAll(collision.contacts[0].point, Weapon.DamageRadius);
            Enemy tmp = null;
            foreach (var hit in hits)
            {
                tmp = null;
                tmp = hit.GetComponent<Enemy>();
                if (tmp != null /*&& tmp != collision.gameObject.GetComponent<Enemy>()*/)
                {
                    tmp.Hit(collision, Weapon);
                }
            }

        }
        else
        {
            if (collision.gameObject.GetComponent<Enemy>() != null)
            {
                collision.gameObject.GetComponent<Enemy>().Hit(collision, Weapon);
            }
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
