using UnityEngine;
using System;

public interface IEntityMove
{
    /// <summary>
    /// Move the entity in this direction. Direction is assumed to be a normalized vector originating at the entity's position 
    /// </summary>
    /// <param name="direction">The direction that the entity would like to move. Recommend that this is a normalized vector</param>
    void MoveInDirection(Vector2 direction);

    bool Moving { get; set; }

}
