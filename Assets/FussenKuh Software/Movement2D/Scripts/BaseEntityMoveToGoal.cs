using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseEntityMoveToGoal : MonoBehaviour, IEntityMoveToGoal
{

    [Header("Movement Configuration:")]
    [SerializeField]
    [Tooltip("The rigidbody to apply force to")]
    protected Rigidbody2D rigidBody;
    [SerializeField]
    [Tooltip("How quickly the entity will accelerate to its max speed")]
    protected float movementAcceleration = 10f;
    [SerializeField]
    [Tooltip("The max speed of the entity")]
    protected float maxMovementSpeed = 10f;
    [SerializeField]
    [Tooltip("The entity's goal position")]
    protected Vector3 goal;


    [SerializeField]
    protected float goalDistanceThreshold = 1f;

    /// <summary>
    /// The entity's goal position
    /// </summary>
    public Vector2 Goal { get { return goal; } }

    /// <summary>
    /// Called when the entity reaches the goal
    /// </summary>
    public Action<GameObject> OnGoalReached { get; set; }


    [Header("Debug Info:")]
    [SerializeField]
    [Tooltip("Draw a ray from the center of the entity in the direction of movement")]
    bool drawDebugGizmo = false;
    [SerializeField]
    [ReadOnly]
    protected bool seekingGoal;
    [SerializeField]
    [ReadOnly]
    float distanceToGoal = int.MaxValue;
    [SerializeField]
    [ReadOnly]
    public float lastDistanceToGoal = int.MaxValue;
    [SerializeField]
    [ReadOnly]
    protected Vector2 rawMoveVector;


    Vector2 CalculateMovementVector(Vector2 argPosition, Vector2 argGoal)
    {
        return (argGoal - argPosition).normalized;
    }

    #region IEntityController 

    /// <summary>
    /// Move the entity toward the goal
    /// </summary>
    /// <param name="argGoal">The goal that the transform should move toward</param>
    public void MoveToGoal(Vector2 argGoal)
    {
        Vector2 position2D = new Vector2(transform.position.x, transform.position.y);

        rawMoveVector = CalculateMovementVector(position2D, argGoal);

        goal = argGoal;
        seekingGoal = true;
    }

    #endregion

    virtual protected void Awake()
    {
        HandleMovement = ApplyConsistentForce;
    }


    // Start is called before the first frame update
    virtual protected void Start()
    {

    }

    virtual protected void FixedUpdate()
    {
        rawMoveVector = CalculateMovementVector(transform.position, goal);
        lastDistanceToGoal = distanceToGoal;
        distanceToGoal = Vector3.Distance(rigidBody.transform.position, goal);
        HandleMovement(rawMoveVector);
    }

    virtual protected void ApplyConsistentForce(Vector2 direction)
    {
        if (rigidBody == null || !seekingGoal) { return; }

        if (distanceToGoal <= goalDistanceThreshold)
        {
            //Debug.Log("Hit dist <= goalThreshold -- " + distanceToGoal + " / " + goalDistanceThreshold);
            distanceToGoal = lastDistanceToGoal = int.MaxValue;
            seekingGoal = false;
            OnGoalReached?.Invoke(gameObject);
            return;
        }

        if (distanceToGoal > lastDistanceToGoal)
        {
            //Debug.Log("Hit dist > lastDist");
            // We've overshot the goal. So, report that we've hit the goal.
            distanceToGoal = lastDistanceToGoal = int.MaxValue;
            seekingGoal = false;
            OnGoalReached?.Invoke(gameObject);
            return;
        }
        
        if (seekingGoal)
        {
            rigidBody.AddForce(direction * movementAcceleration);
            if (rigidBody.velocity.magnitude > maxMovementSpeed)
            {
                rigidBody.velocity = rigidBody.velocity.normalized * maxMovementSpeed;
            }
        }      

        lastGizmoDirection = direction;
    }

    Vector2 lastGizmoDirection;

    private void OnDrawGizmos()
    {
        if (drawDebugGizmo)
        {
            Gizmos.DrawRay(transform.position, lastGizmoDirection);
        }
    }


    protected delegate void MovementDelegate(Vector2 direction);
    protected MovementDelegate HandleMovement;


}
