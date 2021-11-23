using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DetectDamagingCollision : MonoBehaviour
{

    public struct DamageInfo
    {
        public Vector2 location;
        public Vector2 normal;
        public Damageable entity;
    }

    public Action<bool, DamageInfo> OnDamagingCollision;

    List<Damageable> _damageables = new List<Damageable>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Damageable d = collision.collider.GetComponent<Damageable>();
        if (d != null)
        {
            if (!_damageables.Contains(d))
            {
                _damageables.Add(d);
                OnDamagingCollision?.Invoke(true, new DamageInfo() { location = collision.contacts[0].point, normal = collision.contacts[0].normal, entity = d });
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Damageable d = collision.otherCollider.GetComponent<Damageable>();
        if (d != null)
        {
            if (_damageables.Contains(d))
            {
                _damageables.Remove(d);
                OnDamagingCollision?.Invoke(false, new DamageInfo() { location = Vector2.zero, normal = Vector2.zero, entity = d });
            }
        }
    }
}
