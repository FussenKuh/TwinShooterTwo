using UnityEngine;

public interface IEntityRotate
{
    /// <summary>
    /// Rotate the entity such that transform.right is facing this direction. Direction is assumed to be a normalized vector originating at the entity's position 
    /// </summary>
    /// <param name="direction">The direction that the entity would like to rotate. Recommend that this is a normalized vector</param>
    void RotateToDirection(Vector2 direction);

    /// <summary>
    /// Rotate the entity such that transform.right is facing the goal
    /// </summary>
    /// <param name="goal">The goal that the transform should rotate toward</param>
    void RotateToGoal(Vector2 goal);
}
