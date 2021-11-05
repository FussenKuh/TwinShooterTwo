using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseEntityMove : MonoBehaviour, IEntityMove
{

    [Header("Movement Configuration:")]
    [SerializeField]
    [Tooltip("The rigidbody to apply force to")]
    protected Rigidbody2D _rigidBody;
    [SerializeField]
    [Tooltip("How quickly the entity will accelerate to its max speed")]
    protected float _movementAcceleration = 10f;
    [SerializeField]
    [Tooltip("The max speed of the entity")]
    protected float _maxMovementSpeed = 10f;
    [SerializeField]
    [Tooltip("Should the entity be moving?")]
    protected bool _moving;

    [Header("Debug Info:")]
    [SerializeField]
    [Tooltip("Draw a ray from the center of the entity in the direction of movement")]
    bool _drawDebugGizmo = false;

    protected Vector2 _rawMoveVector;

    [ReadOnly]
    [SerializeField]
    float currentSpeed;

    #region IEntityController 

    /// <summary>
    /// Move the entity in this direction. Direction is assumed to be a normalized vector originating at the entity's position 
    /// </summary>
    /// <param name="direction">The direction that the entity would like to move. Recommend that this is a normalized vector</param>
    public void MoveInDirection(Vector2 direction)
    {
        _rawMoveVector = direction.normalized;
    }

    /// <summary>
    /// Should the entity be moving?
    /// </summary>
    public bool Moving { get { return _moving; } set { _moving = value; } }

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
        currentSpeed = _rigidBody.velocity.magnitude;
        if (_moving)
        {
            HandleMovement(_rawMoveVector);
        }
    }

    virtual protected void ApplyConsistentForce(Vector2 direction)
    {
        if (_rigidBody == null) { return; }

        _rigidBody.AddForce(direction * _movementAcceleration);
        if (_rigidBody.velocity.magnitude > _maxMovementSpeed)
        {
            _rigidBody.velocity = _rigidBody.velocity.normalized * _maxMovementSpeed;
        }

        _lastGizmoDirection = direction;
    }

    Vector2 _lastGizmoDirection;
    private void OnDrawGizmos()
    {
        if (_drawDebugGizmo)
        {
            Gizmos.DrawRay(transform.position, _lastGizmoDirection);
        }
    }


    protected delegate void MovementDelegate(Vector2 direction);
    protected MovementDelegate HandleMovement;


}
