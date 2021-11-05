using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(FieldOfViewController))]
public class FindTargetsInFoV : MonoBehaviour
{



    // A reference to the field of view Controller that actually calculates the field of view and generates the FoV mesh 
    FieldOfViewController _fieldOfViewController;

    [SerializeField]
    [Tooltip("Frequency in seconds to check for targets in the FoV")]
    float _frequency = 0.2f;

    [SerializeField]
    [Tooltip("Pause the target search")]
    bool _pauseTargetSearch = false;

    [SerializeField]
    [Tooltip("List of targets to search for")]
    List<Transform> _targets = new List<Transform>();

    [SerializeField]
    [ReadOnly]
    [Tooltip("List of targets found during the last search frame")]
    List<Transform> _targetsFound = new List<Transform>();

    /// <summary>
    /// The frequency in seconds to check for targets in the Fov
    /// </summary>
    public float Frequency { get { return _frequency; } set { _frequency = value; } }
    /// <summary>
    /// Pause the target search
    /// </summary>
    public bool PauseTargetSearch { get { return _pauseTargetSearch; } set { _pauseTargetSearch = value; } }
    /// <summary>
    /// The list of targets to search for
    /// </summary>
    public List<Transform> Targets { get { return _targets; } }

    /// <summary>
    /// The list of FOUND targets to search for
    /// </summary>
    public List<Transform> FoundTargets { get { return _targetsFound; } }

    public event EventHandler<FindTargetsResultsEventArgs> OnFindTargets;

    public class FindTargetsResultsEventArgs : EventArgs
    {
        public bool TargetsFound { get; set; }
        public List<Transform> Targets { get; set; }
    }


    /// <summary>
    /// Coroutine that constantly searches for targets from the target list to see if they are in the FoV area
    /// </summary>
    /// <returns>N/A</returns>
    IEnumerator Search()
    {
        while (true)
        {
            // Start with a fresh 'found targets' list
            _targetsFound.Clear();

            if (_pauseTargetSearch)
            {   // If target searching is paused, make sure we send out a message stating that we've not found a target
                OnFindTargets?.Invoke(this, new FindTargetsResultsEventArgs() { TargetsFound = false, Targets = null });
            }

            // Do nothing unless the search is unpaused
            yield return new WaitWhile(() => _pauseTargetSearch);

            // Loop through each of the targets in our list to see if any are in our FoV
            foreach (Transform tmpTarget in _targets)
            {
                // A target *may* have multiple colliders (or no colliders). Since our FoV calculation is ultimately determined by a raycast, we need
                // to search for any colliders our target has and then loop through that list, looking for valid parts of our target
                TransformAndColliders tmpTaC = new TransformAndColliders(tmpTarget);
                foreach (Transform tmp in tmpTaC.Colliders)
                {
                    if (FindTarget(tmp))
                    {
                        // We found a collider from our target. Flag the target as found
                        _targetsFound.Add(tmpTarget);
                        break;
                    }
                }
            }

            // Report our results to anyone that's subscribed to us
            FindTargetsResultsEventArgs results = new FindTargetsResultsEventArgs();
            results.TargetsFound = _targetsFound.Count > 0;
            results.Targets = _targetsFound;
            OnFindTargets?.Invoke(this, results);

            yield return new WaitForSeconds(_frequency);
        }
    }

    /// <summary>
    /// Find a target in the FoV area
    /// </summary>
    /// <param name="target">The target to look for</param>
    /// <returns>True if the target is found in the FoV area, otherwise false</returns>
    bool FindTarget(Transform target)
    {
        bool retVal = false;
        if (Vector3.Distance(transform.position, target.position) < _fieldOfViewController.ViewDistance)
        {
            // Target is inside viewDistance
            Vector3 dirToTarget = FKS.ProjectileUtils2D.Direction(transform.position, target.position);
            if (Vector3.Angle(_fieldOfViewController.AimDirection, dirToTarget) < _fieldOfViewController.FieldOfViewAngle / 2f)
            {
                // Target is inside Field of View
                RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, dirToTarget, _fieldOfViewController.ViewDistance);

                // NOTE: If you notice that the raycast hit is hitting a collider and you're pretty sure it should be hitting the target yet
                //       the below IF check seems to be failng, it's likely that the raycast is hitting the collider on the object this raycast is
                //       eminating from. (i.e. you're hitting yourself because you're shooting the ray from inside your own collider). One way to
                //       avoid that is to head to Edit -> Project Settings -> Physcis 2D -> Raycasts Start In Colliders and uncheck that box.
                //       Unchecking that box ensures that rays that start from inside of a collider won't detect that collider as a hit

                if (raycastHit2D.collider != null)
                {
                    if (raycastHit2D.collider.gameObject == target.gameObject)
                    {
                        // Hit Target
                        retVal = true;
                    }
                    else
                    {
                        // Hit something else. Do nothing
                    }
                }
            }
        }
        return retVal;
    }

    [Serializable]
    class TransformAndColliders
    {
        private TransformAndColliders() { }

        public TransformAndColliders(Transform item)
        {
            Transform = item;
        }

        [SerializeField]
        Transform _transform = null;
        [SerializeField]
        List<Transform> _colliders = new List<Transform>();

        public Transform Transform
        {
            get { return _transform; }

            set
            {
                _transform = value;
                Collider2D[] tmpCol = value.GetComponentsInChildren<Collider2D>();

                if (tmpCol.Length > 0)
                {
                    _colliders = tmpCol.Select(a => a.transform).ToList();
                }
            }
        }

        public List<Transform> Colliders { get { return _colliders; } }

        public bool HasColliders
        {
            get
            {
                return  _colliders.Count > 0;
            }
        }
    }

    

    // Start is called before the first frame update
    void Start()
    {
        _fieldOfViewController = GetComponent<FieldOfViewController>();
        StartCoroutine(Search());
    }

}
