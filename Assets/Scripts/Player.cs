using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Damageable))]
public class Player : MonoBehaviour, IEntity
{
    static int uniqueID = 0;

    [SerializeField]
    SpriteRenderer lifeBar;
    [SerializeField]
    SpriteRenderer energyBar;


    Rigidbody2D _rigidBody;
    float initialDrag;

    public int MyID { get; set; }

    [SerializeField]
    EntityStats _entityStatsSO;
    EntityStats.StatData _entityStats;
    public EntityStats.StatData EntityInfo { get { return _entityStats; } set { _entityStats = value; } }

    Damageable _damageable;

    [SerializeField]
    PlayerInputHandler playerInputHandler;

    [SerializeField]
    bool atGoal = false;

    SpriteRenderer sr;

    public bool UsingKeyboardAndMouse { get { return GetComponent<PlayerInput>().currentControlScheme == "Keyboard&Mouse"; } }

    private void Awake()
    {
        Messenger<bool, Player>.AddListener("PlayerAtGoal", OnPlayerAtGoal);

        _damageable = GetComponent<Damageable>();
        _damageable.OnHit += OnHit;

        _rigidBody = GetComponent<Rigidbody2D>();
        initialDrag = _rigidBody.drag;

        MyID = uniqueID;
        uniqueID++;

        _entityStats = _entityStatsSO.Data;


        sr = gameObject.GetComponent<SpriteRenderer>();

        switch (Random.Range(0, 2))
        {
            case 0:
                // Make a blue-ish player
                _entityStats.StartingColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
                break;
            case 1:
                // Make a green-ish player
                _entityStats.StartingColor = new Color(Random.Range(0f, 1f), 1, Random.Range(0f, 1f));
                break;
        }

        _entityStats.Color = _entityStats.StartingColor;
        sr.color = _entityStats.Color;

        EffectsManager.Instance.EntitySpawn(transform.position, _entityStats.Color);
    }

    private void OnDestroy()
    {
        Messenger<bool, Player>.RemoveListener("PlayerAtGoal", OnPlayerAtGoal);

        _damageable.OnHit -= OnHit;
    }

    void OnPlayerAtGoal(bool goalReached, Player playerID)
    {
        if (playerID.MyID == MyID)
        {
            atGoal = goalReached;
        }
    }


    void OnUse(object sender, PlayerInputHandler.OnButtonArgs args)
    {
        if (atGoal && args.Phase == UnityEngine.InputSystem.InputActionPhase.Started && GameManager.Instance.DamageToClearLevel <= 0)
        {
            atGoal = false;
            GameManager.Instance.LevelCompleted();
        }
    }

    
    public void ResetPlayer()
    {
        atGoal = false;
        _rigidBody.drag = initialDrag;
    }


    // Start is called before the first frame update
    void Start()
    {
        playerInputHandler = GetComponent<PlayerInputHandler>();
        if (playerInputHandler == null)
        {
            Debug.LogWarning("Player - Couldn't find a PlayerInputHandler reference");
            playerInputHandler = null;
        }
        else
        {
            playerInputHandler.UseEvent += OnUse;
        }
    }


    private void Update()
    {
        //UpdateEnergy();
        //CheckAlive();
    }


    void CheckAlive()
    {
        if (!_entityStats.Alive)
        {
            //_entityStats.Health = 1; // TODO FOR NOW, LETS NEVER LET THE PLAYER DIE!
            MessagePopup.Create(gameObject.transform.position + new Vector3(0, gameObject.transform.localScale.y / 2, 0), "I Should Be Dead, Jim!", false);
            if (PlayerManager.Instance.Players.Where(p => p.EntityInfo.Alive).ToArray().Length == 0)
            {
                StatsOverlay.Instance.UpdateBottomText
                    (
                    "<size=300%><color=green>g</color><color=blue>a</color><color=red>m</color><color=yellow>e</color> <color=orange>O</color><color=#7f00ff>v</color><color=#0083FF>e</color><color=#ff07ff>r</color>\n\n\n\n\n\n\n</size>"
                    );
            }
        }
    }

    public float _currentEnergy = 0;

    public void UpdateEnergy()
    {
        _entityStats.Energy += _entityStats.EnergyGainRate * Time.deltaTime;
        _entityStats.Energy = Mathf.Min(_entityStats.Energy, _entityStats.StartingEnergy);

        UpdateEnergyBar();
    }

    public void UpdateEnergyBar()
    {
        float tmpEnergy = _entityStats.Energy;
        Vector3 tmpScale = energyBar.transform.localScale;
        tmpScale.x = tmpEnergy.Remap(0f, _entityStats.StartingEnergy, 0f, 1f);
        energyBar.transform.localScale = tmpScale;
    }

    Color UpdateColor()
    {
        float newTint = ((float)_entityStats.Health / _entityStats.StartingHealth).Remap(0f, 1f, 0.3f, 0.8f);
        return new Color(newTint, newTint, newTint);
    }

    public void OnHit(WeaponData weaponData)
    {
        if (!_entityStats.Alive) { return; }

        if (_entityStats.Alive)
        {
            WeaponData.DamageType damage = weaponData.Damage;

            _entityStats.Health -= damage.damage;
            MessagePopup.Create(gameObject.transform.position + new Vector3(0, gameObject.transform.localScale.y / 2, 0), Mathf.CeilToInt(damage.damage).ToString(), damage.critical);
            _entityStats.Color = _entityStats.StartingColor * UpdateColor();
            sr.color = _entityStats.Color;

            UpdateHealthBar();
            CheckAlive();
        }


    }

    public void UpdateHealthBar()
    {
        float tmpHealth = _entityStats.Health;
        Vector3 tmpScale = lifeBar.transform.localScale;
        tmpScale.x = tmpHealth.Remap(0f, _entityStats.StartingHealth, 0f, 1f);
        lifeBar.transform.localScale = tmpScale;
    }

    private void OnTriggerEnter2D(Collider2D triggeredThing)
    {
//        Debug.Log(_playerStats.Name + "("+ MyID + ") triggered: " + triggeredThing.name);

        HandleCollectible(triggeredThing);

    }

    private void HandleCollectible(Collider2D triggeredThing)
    {
        var collectible = triggeredThing.gameObject.GetComponent<CollectibleBehavior>();
        if (collectible != null)
        {
            if (collectible.Item != null)
            {
                if (collectible.Item.Weapon != null)
                {
                    EntityInfo.Weapon = collectible.Item.Weapon;
                    MessagePopup.Create(triggeredThing.transform.position, "Picked up " + collectible.Item.Weapon.Name, false, 1f);
                }
            }
        }
    }
}

