using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DetectDamagingCollision))]
public class Bullet : MonoBehaviour
{
    SpriteRenderer sr;
    DetectDamagingCollision _detectDamagingCollision;

    public GameObject explosionPrefab;

    [SerializeField]
    bool _debug = false;

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

    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();

        _detectDamagingCollision = GetComponent<DetectDamagingCollision>();
        _detectDamagingCollision.OnDamagingCollision += OnDamagingCollision;
    }

    private void OnDestroy()
    {
        _detectDamagingCollision.OnDamagingCollision -= OnDamagingCollision;
    }

    void OnDamagingCollision(bool colliding, DetectDamagingCollision.DamageInfo contactInfo)
    {
        if (colliding)
        {
            EffectsManager.Instance.BulletHit(contactInfo);

            if (Weapon.DamageRadius > 0)
            {
                var hits = Physics2D.OverlapCircleAll(contactInfo.location, Weapon.DamageRadius);

                Damageable tmp = null;
                foreach (var hit in hits)
                {
                    if (hit.isTrigger) { continue; } // Ignore trigger colliders. You can't 'damage' a trigger

                    tmp = null;
                    tmp = hit.GetComponent<Damageable>();
                    if (tmp != null)
                    {
                        if (_debug) { Debug.Log(name + " Bullet Hit Splash Damageable Entity (" + tmp.name + ")"); }
                        tmp.Hit(Weapon);
                    }
                }
            }
            else
            {
                if (_debug) { Debug.Log(name + " Bullet Hit Damageable Entity (" + contactInfo.entity.name + ")"); }

                contactInfo.entity.Hit(Weapon);
            }

            Destroy(gameObject);
        }
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
