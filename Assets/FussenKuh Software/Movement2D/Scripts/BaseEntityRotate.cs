using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEntityRotate : MonoBehaviour, IEntityRotate
{
    [SerializeField]
    [Tooltip("It will take this long to rotate the entity 360 degrees")]
    protected float rotationSpeed = 1f;

    protected Vector2 rawLookVector;
    protected Vector3 lookAngle = Vector3.zero;

    protected delegate void LookDelegate(Vector3 direction);
    protected LookDelegate HandleLook;

    #region IEntityRotate

    /// <summary>
    /// Rotate the entity such that transform.right is facing this direction. Direction is assumed to be a normalized vector originating at the entity's position 
    /// </summary>
    /// <param name="direction">The direction that the entity would like to rotate. Recommend that this is a normalized vector</param>
    public virtual void RotateToDirection(Vector2 direction)
    {
        rawLookVector = direction;
        lookAngle = Vector3.forward * Vector2.SignedAngle(Vector2.right, rawLookVector);

        if (lookAngle.z < 0) { lookAngle += Vector3.forward * 360; }
    }

    /// <summary>
    /// Rotate the entity such that transform.right is facing the goal
    /// </summary>
    /// <param name="goal">The goal that the transform should rotate toward</param>
    public void RotateToGoal(Vector2 goal)
    {
        RotateToDirection(Direction(transform.position, goal));
    }

    #endregion

    virtual protected void Awake()
    {
        HandleLook = ApplyConsistentRotation;
    }

    virtual protected void Start()
    {
        StartCoroutine(Rotate());
    }

    IEnumerator Rotate()
    {
        while (true)
        {
            HandleLook(lookAngle);
            yield return new WaitForSeconds(rotationSimulationDelay);
        }
    }

    // How long to wait before the next rotation tick.
    float rotationSimulationDelay = 0.02f;
    // A maxSpeed of 1 implies the user wants to make a 360 degree rotation in 1s. Through experimentation, 
    // setting the 'maxSpeed' argument in Vector3.SmoothDamp to this value (while delaying rotationSimulationDelay
    // seconds per tick) rotates an object 360 deg in 1s. Thus, we want the user to enter values that make sense. 
    // Thus, let the user set a speed in seconds and we'll multiply it accordingly.
    float speedMultiplier = 80000;
    // SmoothDamp requires a reference velocity. This variable will hold that value. It should be only
    // used (and modified) by the SmoothDamp function.
    Vector3 currentVelocity = Vector3.zero;


    protected virtual void ApplyConsistentRotation(Vector3 desiredRotation)
    {
        Vector3 currentRotation;
        //currentVelocity = Vector3.zero;

        currentRotation = transform.rotation.eulerAngles;
        if (currentRotation.z < 0) { currentRotation += Vector3.forward * 360; }

        if (Mathf.Abs(desiredRotation.z - currentRotation.z) > 180)
        {
            if (desiredRotation.z > currentRotation.z)
            {
                desiredRotation -= Vector3.forward * 360;
            }
            else
            {
                currentRotation -= Vector3.forward * 360;
            }
        }

        transform.rotation = Quaternion.Euler(Vector3.SmoothDamp(currentRotation, desiredRotation, ref currentVelocity, 0, speedMultiplier / rotationSpeed));
    }


    /// <summary>
    /// Calculate a normalized direction vector from myPosition to targetPosition
    /// </summary>
    /// <param name="myPosition">My position</param>
    /// <param name="targetPosition">Theh position of the target</param>
    /// <returns></returns>
    static Vector3 Direction(Vector3 myPosition, Vector3 targetPosition)
    {
        // Gets a vector that points from myPosition to targetPosition.
        return (targetPosition - myPosition).normalized;
    }
}
