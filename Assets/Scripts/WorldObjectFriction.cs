using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WorldObjectID))]
public class WorldObjectFriction : MonoBehaviour
{
    [SerializeField]
    WorldObjectID _worldObjectID;

    #region Collision/Trigger Region
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (_worldObjectID.Data.Type == WorldObjectData.ObjectType.FLOOR)
        {
            collider.attachedRigidbody.drag *= _worldObjectID.Data.Friction;
            //Debug.Log(name + " Entered trigger with " + collider.name + ". Friction multiplier = " + _worldObjectID.Data.Friction.ToString("N2"));
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (_worldObjectID.Data.Type == WorldObjectData.ObjectType.FLOOR)
        {
            collider.attachedRigidbody.drag *= (1 / _worldObjectID.Data.Friction);
            //Debug.Log(name + " Exited trigger with " + collider.name + ". Friction multiplier = " + _worldObjectID.Data.Friction.ToString("N2"));
        }
    }

    #endregion

    private void Awake()
    {
        _worldObjectID = gameObject.GetComponent<WorldObjectID>();
    }

}
