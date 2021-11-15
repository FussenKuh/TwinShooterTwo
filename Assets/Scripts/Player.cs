using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{
    static int uniqueID = 0;

    [SerializeField]
    SpriteRenderer lifeBar;

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

    SpriteRenderer sr;

    private void Awake()
    {
        Messenger<bool, Player>.AddListener("PlayerAtGoal", OnPlayerAtGoal);

        MyID = uniqueID;
        uniqueID++;

        _playerStats = _playerStatsSO.Data;


        sr = gameObject.GetComponent<SpriteRenderer>();

        switch (Random.Range(0, 2))
        {
            case 0:
                // Make a blue-ish player
                _playerStats.StartingColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
                break;
            case 1:
                // Make a green-ish player
                _playerStats.StartingColor = new Color(Random.Range(0f, 1f), 1, Random.Range(0f, 1f));
                break;
        }

        _playerStats.Color = _playerStats.StartingColor;
        sr.color = _playerStats.Color;

        EffectsManager.Instance.EntitySpawn(transform.position, _playerStats.Color);
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


    Color UpdateColor()
    {
        float newTint = ((float)_playerStats.Health / _playerStats.StartingHealth).Remap(0f, 1f, 0.3f, 0.8f);
        return new Color(newTint, newTint, newTint);
    }

    public void Hit(WeaponData weaponData)
    {
        if (!_playerStats.Alive) { return; }

        if (_playerStats.Alive)
        {
            int damage = weaponData.Damage;
            _playerStats.Health -= damage;
//            Debug.Log("Ow! Remaining Health: " + _playerStats.Health);
            MessagePopup.Create(gameObject.transform.position + new Vector3(0, gameObject.transform.localScale.y / 2, 0), damage.ToString(), false);
            _playerStats.Color = _playerStats.StartingColor * UpdateColor();
            sr.color = _playerStats.Color;

            float tmpHealth = _playerStats.Health;
            Vector3 tmpScale = lifeBar.transform.localScale;
            tmpScale.x = tmpHealth.Remap(0f, _playerStats.StartingHealth, 0f, 1f);
            lifeBar.transform.localScale = tmpScale;

        }

        if (!_playerStats.Alive)
        {
            //_playerStats.Health = 1; // TODO FOR NOW, LETS NEVER LET THE PLAYER DIE!
            MessagePopup.Create(gameObject.transform.position + new Vector3(0, gameObject.transform.localScale.y / 2, 0), "I Should Be Dead, Jim!", false);
            if (PlayerManager.Instance.Players.Where(p => p.PlayerStats.Alive).ToArray().Length == 0)
            {
                StatsOverlay.Instance.UpdateBottomText
                    (
                    "<size=300%><color=green>g</color><color=blue>a</color><color=red>m</color><color=yellow>e</color> <color=orange>O</color><color=#7f00ff>v</color><color=#0083FF>e</color><color=#ff07ff>r</color>\n\n\n\n\n\n\n</size>"
                    );
            }
        }
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
                    currentWeapon = collectible.Item.Weapon;
                    MessagePopup.Create(triggeredThing.transform.position, "Picked up " + collectible.Item.Weapon.Name, false, 1f);
                }
            }
        }
    }
}

