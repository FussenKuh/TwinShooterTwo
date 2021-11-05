using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RoomLayout
{
    public class EnemyBehavior : MonoBehaviour
    {
        [SerializeField]
        BaseEntityMoveToGoal _baseEntityMoveToGoal = null;

        [SerializeField]
        Transform goal = null;

        [SerializeField]
        float detectionDistance = 5f;

        [SerializeField]
        List<Player> activePlayers = new List<Player>();


        // Start is called before the first frame update
        void Start()
        {
            activePlayers = PlayerManager.Instance.Players;
        }

        private void FixedUpdate()
        {

            List<Player> nearMe = activePlayers.Where(p => Vector3.Distance(p.transform.position, gameObject.transform.position) <= detectionDistance).OrderByDescending(p => Vector3.Distance(p.transform.position, gameObject.transform.position)).ToList();
            if ( nearMe.Count > 0)
            {
                goal = nearMe[nearMe.Count - 1].transform;
            }
            else
            {
                goal = null;
            }

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
}
