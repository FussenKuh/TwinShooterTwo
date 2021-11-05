using UnityEngine;
using System;

public interface IEntityMoveToGoal 
{

    /// <summary>
    /// Move the entity toward the goal
    /// </summary>
    /// <param name="goal">The goal that the transform should move toward</param>
    void MoveToGoal(Vector2 goal);

    Action<GameObject> OnGoalReached { get; set; }
}
