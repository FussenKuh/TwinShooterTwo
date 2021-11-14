using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    static int uniqueID = 0;

    public int MyID { get; set; }

    [SerializeField]
    PlayerStats _playerStatsSO;

    PlayerStats.StatData _playerStats;

    public PlayerStats.StatData PlayerStats { get { return _playerStats; } }

    public WeaponData currentWeapon;

    [SerializeField]
    PlayerInputHandler playerInputHandler;

    [SerializeField]
    bool atGoal = false;

    private void Awake()
    {
        Messenger<bool, Player>.AddListener("PlayerAtGoal", OnPlayerAtGoal);

        MyID = uniqueID;
        uniqueID++;

        _playerStats = _playerStatsSO.Data;

    }

    private void OnDestroy()
    {
        Messenger<bool, Player>.RemoveListener("PlayerAtGoal", OnPlayerAtGoal);
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
        if (atGoal && args.Phase == UnityEngine.InputSystem.InputActionPhase.Started)
        {
            atGoal = false;
            GameManager.Instance.LevelCompleted();
        }
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


    public void Hit(WeaponData weaponData)
    {
        if (_playerStats.Alive)
        {
            int damage = weaponData.Damage;
            _playerStats.Health -= damage;
            Debug.Log("Ow! Remaining Health: " + _playerStats.Health);
            MessagePopup.Create(gameObject.transform.position + new Vector3(0, gameObject.transform.localScale.y / 2, 0), damage.ToString(), false);
        }

        if (!_playerStats.Alive)
        {
            _playerStats.Health = 1; // TODO FOR NOW, LETS NEVER LET THE PLAYER DIE!
            MessagePopup.Create(gameObject.transform.position + new Vector3(0, gameObject.transform.localScale.y / 2, 0), "I Should Be Dead, Jim!", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D triggeredThing)
    {
        Debug.Log(_playerStats.Name + "("+ MyID + ") triggered: " + triggeredThing.name);

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
                    currentWeapon = collectible.Item.Weapon;
                    MessagePopup.Create(triggeredThing.transform.position, "Picked up " + collectible.Item.Weapon.Name, false, 1f);
                }
            }
        }
    }
}

