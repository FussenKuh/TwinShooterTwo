using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField]
    BaseEntityMoveToGoal _baseEntityMoveToGoal = null;

    [SerializeField]
    Transform goal = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (goal != null)
        {
            _baseEntityMoveToGoal.MoveToGoal(goal.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
