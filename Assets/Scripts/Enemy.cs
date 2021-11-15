using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class Enemy : MonoBehaviour
{
    public bool EnableDebugText = false;
    public TextMeshPro debugText;
    [SerializeField]
    float elapsedTime = 0f;
    [SerializeField]
    float idleDurration = 3f;
    [SerializeField]
    Vector3 startLocation;
    [SerializeField]
    float distanceThreshold = 0.5f;

    [SerializeField]
    EnemyStats _enemyStatsSO;
    [SerializeField]
    EnemyStats.StatData _enemyStats;
    [SerializeField]
    float timeSinceLastAttack = 0;

    public EnemyStats.StatData EnemyStats { get { return _enemyStats; } }

    void AmIStuck()
    {
        if (elapsedTime > idleDurration)
        {
            if (Vector3.Distance(startLocation, transform.position) >= distanceThreshold)
            {
                startLocation = transform.position;
                elapsedTime = 0;
            }
            else
            {
                EffectsManager.Instance.EntitySpawn(transform.position, origColor);
                //Debug.Log(name + " has been idle too long. We're killing it off");
                EnemyManager.Instance.Kill(gameObject);
            }
        }
    }


    static int uniqueID = 0;

    int myID = 0;
    int MyID { get { return myID; } }

    [SerializeField]
    BaseEntityMoveToGoal _baseEntityMoveToGoal = null;

    [SerializeField]
    Transform _turret = null;
    [SerializeField]
    BaseEntityRotate _turretRotationController = null;

    [SerializeField]
    Transform goal = null;

    [SerializeField]
    float detectionDistance = 5f;

    [SerializeField]
    List<Player> activePlayers = new List<Player>();

    [SerializeField]
    int maxHealth = 3;
    [SerializeField]
    int currentHealth = 3;

    Rigidbody2D rb;
    Color origColor;

    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        // Let the manager know that we exist
        EnemyManager.Instance.AddEnemy(this);

        origColor = gameObject.GetComponent<SpriteRenderer>().color;
        rb = gameObject.GetComponent<Rigidbody2D>();
        activePlayers = PlayerManager.Instance.Players;

        sr = gameObject.GetComponent<SpriteRenderer>();

        if (_enemyStats.Weapon.FireRate >= idleDurration)
        {
            // We want to ensure that our idle duration is longer than our fire rate
            idleDurration = _enemyStats.Weapon.FireRate * (1.5f);
        }

        _baseEntityMoveToGoal.MaxMovementSpeed *= (Random.Range(1f, 2f));

        StartCoroutine(Attack());
    }

    Vector3 searchGoal = Vector3.zero;


    bool TooFarAway()
    {
        bool retVal = false;

        List<Player> nearMe = activePlayers.Where(p => Vector3.Distance(p.transform.position, gameObject.transform.position) <= (detectionDistance * 2))
            .OrderByDescending(p => Vector3.Distance(p.transform.position, gameObject.transform.position)).ToList();

        if (nearMe.Count == 0)
        {
            retVal = true;
        }

        return retVal;
    }

    private void FixedUpdate()
    {
        timeSinceLastAttack += Time.fixedDeltaTime;
        elapsedTime += Time.fixedDeltaTime;
        debugText.SetText(rb.velocity.magnitude.ToString("N1") + (goal != null).ToString());
        if (EnableDebugText)
        {
            debugText.enabled = true;
        }
        else
        {
            debugText.enabled = false;
        }
        AmIStuck();

        if (TooFarAway())
        {
            EffectsManager.Instance.EntitySpawn(transform.position, origColor);
            EnemyManager.Instance.Kill(gameObject);
            return;
        }

        List<Player> nearMe = activePlayers.Where(p => Vector3.Distance(p.transform.position, gameObject.transform.position) <= detectionDistance).Where(p => p.PlayerStats.Alive)
            .OrderByDescending(p => Vector3.Distance(p.transform.position, gameObject.transform.position)).ToList();

        if (nearMe.Count > 0)
        {
            goal = nearMe[nearMe.Count - 1].transform;
        }
        else
        {
            goal = null;
            _baseEntityMoveToGoal.CancelMovement();
//            Debug.Log(name + " Lost track of player");

            //if (Vector3.Distance(searchGoal, transform.position) > 10 || Vector3.Distance(searchGoal, transform.position) <= 1)
            //{
            //    searchGoal = FKS.Utils.Rand.RandomPointOnXYCircle(transform.position, 2);
            //}
            //_baseEntityMoveToGoal.MoveToGoal(searchGoal);
            //_turretRotationController.RotateToGoal(searchGoal);
        }

        if (goal != null)
        {
            _baseEntityMoveToGoal.MoveToGoal(goal.position);
            _turretRotationController.RotateToGoal(goal.position);
        }
    }

    private void Awake()
    {
        myID = uniqueID;
        uniqueID++;

        gameObject.name = "Enemy - " + myID.ToString("D3");

        _enemyStats = _enemyStatsSO.Stats;
        currentHealth = _enemyStats.StartingHealth;
    }

    Color UpdateColor()
    {
        float newTint = ((float)currentHealth / _enemyStats.StartingHealth).Remap(0f, 1f, 0.3f, 0.8f);
        return new Color(newTint, newTint, newTint); 
    }

    [SerializeField]
    Player attackThisPlayer = null;
    IEnumerator Attack()
    {
        while (true)
        {
            if (attackThisPlayer != null)
            {
                if (timeSinceLastAttack >= _enemyStats.Weapon.FireRate)
                {
                    timeSinceLastAttack = 0;
                    elapsedTime = 0; // We're attacking something, so we're obviously not idle.
                    attackThisPlayer.Hit(_enemyStats.Weapon);
                }
                
            }
            yield return new WaitForEndOfFrame();
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        

        HandlePlayer(collision, true);
    }

    private void HandlePlayer(Collision2D collision, bool entering)
    {
        var player = collision.gameObject.GetComponent<Player>();
        
        if (entering)
        {
            if (player != null)
            {
                // We found a player component, so cache it away so we can attack it
//                Debug.Log(_enemyStats.Name + "(" + MyID + ") collided with " + collision.gameObject.name);
                attackThisPlayer = player;
            }
        }
        else
        {
            if (player == attackThisPlayer)
            {
                // The player we were attacking has left. Delete him from our cache so we don't continue to attack him
                attackThisPlayer = null;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        HandlePlayer(collision, false);
    }

    public void Hit(Collision2D collision, WeaponData weaponData)
    {
        int damage = weaponData.Damage;
        bool crit = FKS.Utils.UtilsClass.TestChance(30);
        if (crit)
        {
            damage = Mathf.RoundToInt(damage * 1.5f);
        }

        int tmpHealth = currentHealth;

        MessagePopup.Create(gameObject.transform.position + new Vector3(0, gameObject.transform.localScale.y / 2, 0), damage.ToString(), crit);

        if (damage > currentHealth) { damage = currentHealth; }
        GameManager.Instance.AddToTotalDamage(damage);

        currentHealth -= damage;

        sr.color = origColor * UpdateColor();

        if (currentHealth <= 0)
        {
            EffectsManager.Instance.EnemyDeath(transform.position, origColor);
            EnemyManager.Instance.Kill(gameObject);
        }
    }

    private void OnDestroy()
    {
        //Debug.Log(name + " has been killed");
    }

}

