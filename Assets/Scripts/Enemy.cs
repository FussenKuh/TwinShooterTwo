using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Enemy : MonoBehaviour
{
    static int uniqueID = 0;

    int myID = 0;
    int MyID { get; }

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

    // Start is called before the first frame update
    void Start()
    {
        activePlayers = PlayerManager.Instance.Players;
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

        if (TooFarAway())
        {
            Debug.Log(name + " is too far away. Killing it off...");
            EnemyManager.Instance.Kill(gameObject);
            return;
        }

        List<Player> nearMe = activePlayers.Where(p => Vector3.Distance(p.transform.position, gameObject.transform.position) <= detectionDistance)
            .OrderByDescending(p => Vector3.Distance(p.transform.position, gameObject.transform.position)).ToList();

        if (nearMe.Count > 0)
        {
            goal = nearMe[nearMe.Count - 1].transform;
        }
        else
        {
            goal = null;
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

        currentHealth = maxHealth;
    }

    Color UpdateColor(Color tint)
    {
        Color retValue = tint;

        if (currentHealth > 0)
        {
            retValue = new Color((tint.r * (1f / currentHealth)), (tint.g * (1f / currentHealth)), (tint.b * (1f / currentHealth)));
        }
        return retValue;
    }

    public void Hit(GameObject objectHit)
    {
        currentHealth--;

        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();

        sr.color = UpdateColor(objectHit.GetComponent<SpriteRenderer>().color);
        

        if (currentHealth <= 0)
        {
            EnemyManager.Instance.Kill(gameObject);
        }
    }

    private void OnDestroy()
    {
        //Debug.Log(name + " has been killed");
    }

}

