using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(SpriteRenderer))]
public class WorldObjectDamage : MonoBehaviour
{
    Damageable _damageable;

    SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _damageable = GetComponent<Damageable>();
        _damageable.OnHit += OnHit;

        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnDestroy()
    {
        if (_damageable != null) { _damageable.OnHit -= OnHit; }
    }

    void OnHit(WeaponData weaponData)
    {
        Debug.Log(name + " was hit");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
