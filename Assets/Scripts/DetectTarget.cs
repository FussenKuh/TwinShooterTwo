using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DetectTarget : MonoBehaviour
{
    [SerializeField]
    Transform _target;
    [SerializeField]
    bool _debug = false;
    [SerializeField]
    bool _disabled = false;

    [SerializeField]
    List<string> _possibleTargets = new List<string>() { "Player" };

    public Action<Transform> OnDetectTarget;

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (_disabled) { return; }

        // If we're already tracking a target, don't bother doing anything... we're already hunting
        if (_target == null)
        {
            // Make sure that we've triggered on something we're interested in
            if (_possibleTargets.Contains(otherCollider.tag))
            {
                if (_debug) { Debug.Log(otherCollider.name + " triggered with me (" + name + ")"); }

                _target = otherCollider.transform;
                OnDetectTarget?.Invoke(_target);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (_disabled) { return; }

        // Make sure we've triggered on something we're interested in
        if (_possibleTargets.Contains(otherCollider.tag))
        {
            // Only do something if the player we're currently tracking is leaving our tracking zone
            if (_target == otherCollider.transform)
            {
                if (_debug) { Debug.Log(otherCollider.name + " left me (" + name + ")"); }

                _target = null;
                OnDetectTarget?.Invoke(_target);
            }
        }
    }



}
