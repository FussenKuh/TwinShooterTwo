using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


[RequireComponent(typeof(AIDestinationSetter))]
[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(DetectIdle))]
[RequireComponent(typeof(DetectTarget))]
[RequireComponent(typeof(DetectDamagingCollision))]
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyBase : MonoBehaviour, IEntity
{
    static int _uniqueIDCounter = 0;

    [SerializeField]
    EntityStats _entityStatsScriptableObject;
    [SerializeField]
    EntityStats.StatData _entityStats;

    [SerializeField]
    bool _invincible = false;

    public EntityStats.StatData EntityInfo { get { return _entityStats; } set { _entityStats = value; } }


    float _timeSinceLastAttack = 100;

    DetectIdle _detectIdle;
    DetectTarget _detectTarget;
    DetectDamagingCollision _detectDamagingCollision;

    Damageable _damageable;

    AIDestinationSetter _aiDestinationSetter;
    AIPath _aiPath;

    SpriteRenderer _spriteRenderer;

    int _uniqueID = 0;
    int UniqueID { get { return _uniqueID; } }

    private void Awake()
    {
        if (_entityStatsScriptableObject == null)
        {
            Debug.LogWarning(name + " has no EntityStats scriptable object. This is going to be a problem");
            _entityStatsScriptableObject = null;
        }

        _uniqueID = _uniqueIDCounter;
        _uniqueIDCounter++;

        gameObject.name = "Enemy - " + _uniqueID.ToString("D3");
        _entityStats = _entityStatsScriptableObject.Data;
    }

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _aiPath = GetComponent<AIPath>();
        _aiDestinationSetter = GetComponent<AIDestinationSetter>();

        _detectIdle = GetComponent<DetectIdle>();
        _detectIdle.OnDetectIdle += OnDetectIdle;

        _detectTarget = GetComponent<DetectTarget>();
        _detectTarget.OnDetectTarget += OnDetectTarget;

        _detectDamagingCollision = GetComponent<DetectDamagingCollision>();
        _detectDamagingCollision.OnDamagingCollision += OnDetectDamagingCollision;

        _damageable = GetComponent<Damageable>();
        _damageable.OnHit += OnHit;

        // Slightly randomize the entity's speed to make it more interesting
        _entityStats.Speed *= Random.Range(1f, 1.3f);
        _aiPath.maxSpeed = _entityStats.Speed;

        _spriteRenderer.color = EntityInfo.StartingColor;

        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        var main = ps.main;
        main.startColor = EntityInfo.StartingColor;
    }


    private void OnDestroy()
    {
        if (_detectIdle != null) { _detectIdle.OnDetectIdle -= OnDetectIdle; }
        if (_detectTarget != null) { _detectTarget.OnDetectTarget -= OnDetectTarget; }
        if (_detectDamagingCollision != null) { _detectDamagingCollision.OnDamagingCollision -= OnDetectDamagingCollision; }
        if (_damageable != null) { _damageable.OnHit -= OnHit; }
    }

    void UpdateColor()
    {
        float newTint = ((float)_entityStats.Health / _entityStats.StartingHealth).Remap(0f, 1f, 0.3f, 0.8f);

        Color tmp = _entityStats.StartingColor * newTint;
        tmp.a = 1;
        _entityStats.Color = tmp;
        _spriteRenderer.color = _entityStats.Color;

        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        var main = ps.main;
        main.startColor = _entityStats.Color;
    }

    void OnHit(WeaponData weaponData)
    {
        // Calculate the damage and present a damage popup
        WeaponData.DamageType damage = weaponData.Damage;
        MessagePopup.Create(gameObject.transform.position + new Vector3(0, gameObject.transform.localScale.y / 2, 0), Mathf.CeilToInt(damage.damage).ToString(), damage.critical);

        // If we're not invicible, then actually take the damage
        if (!_invincible)
        {
            if (damage.damage > EntityInfo.Health) { damage.damage = EntityInfo.Health; }
            GameManager.Instance.AddToTotalDamage((int)damage.damage);
            Messenger<float>.Broadcast("EnemyDamage", (int)damage.damage);

            EntityInfo.Health -= damage.damage;

            UpdateColor();

            if (EntityInfo.Health <= 0)
            {
                FKS.AudioManager.PlayAudio("Die");
                EffectsManager.Instance.EnemyDeath(transform.position, EntityInfo.StartingColor);
                Destroy(gameObject);
                //EnemyManager.Instance.Kill(gameObject);
            }
            else
            {
                FKS.AudioManager.PlayAudio("Hit");
            }
        }
    }

    void OnDetectTarget(Transform target)
    {
        HandleTargetPathfinding(target);
    }



    IEnumerator Attack(DetectDamagingCollision.DamageInfo info)
    {
        while (true)
        {
            if (_timeSinceLastAttack >= EntityInfo.Weapon.FireRate)
            {
                _timeSinceLastAttack = 0;
                info.entity.Hit(EntityInfo.Weapon);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    Coroutine attacking = null;

    void OnDetectDamagingCollision(bool colliding, DetectDamagingCollision.DamageInfo contactInfo)
    {
        if (colliding && attacking == null && contactInfo.entity.CanItHurtMe(EntityInfo.Weapon.Type))
        {
            //Debug.LogFormat("{0} is attacking {1}", name, contactInfo.entity.name);
            attacking = StartCoroutine(Attack(contactInfo));
        }
        else
        {
            if (attacking != null)
            {
                //Debug.LogFormat("{0} is stopping the attack on {1}", name, contactInfo.entity.name);
                StopCoroutine(attacking);
                attacking = null;
            }
        }
    }

    void OnDetectIdle()
    {
        EffectsManager.Instance.EntitySpawn(transform.position, Color.black);// EntityInfo.Color);
        Destroy(gameObject);
        //EnemyManager.Instance.Kill(gameObject);
    }

    private void HandleTargetPathfinding(Transform target)
    {
        if (AstarPath.active != null)
        {
            if (target != null)
            {
                _aiDestinationSetter.target = target;
                _aiPath.canSearch = true;
            }
            else
            {
                _aiDestinationSetter.target = null;
                _aiPath.SetPath(null);
                _aiPath.canSearch = false;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        _timeSinceLastAttack += Time.deltaTime;
    }
}
