using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewController : MonoBehaviour
{
    // A reference to the field of view object that actually calculates the field of view and generates the FoV mesh 
    FieldOfView _fieldOfView;

    [SerializeField]
    [Tooltip("Layers that the FoV will interact with")]
    private LayerMask layerMask = new LayerMask();

    [SerializeField]
    [Tooltip("The FoV angle (How wide is the field of view?)")]
    float _fovAngle = 45;
    [SerializeField]
    [Tooltip("The view distance (How far away does the field of view stretch from the origin?")]
    float _viewDistance = 5;
    [SerializeField]
    [Tooltip("The direction to look")]
    Vector3 _aimDirection = Vector3.right;

    [SerializeField]
    [Tooltip("The Field of View prefab to spawn")]
    GameObject _fieldOfViewElementPrefab = null;

    /// <summary>
    /// The Field of View angle
    /// </summary>
    public float FieldOfViewAngle { get { return _fovAngle; } set { _fovAngle = value; _fieldOfView.SetFoV(_fovAngle); } }

    /// <summary>
    /// The sight distance
    /// </summary>
    public float ViewDistance { get { return _viewDistance; } set { _viewDistance = value; _fieldOfView.SetViewDistance(_viewDistance); } }

    /// <summary>
    /// The aim direction
    /// </summary>
    public Vector3 AimDirection { get { return _aimDirection; } set { _aimDirection = value; _fieldOfView.SetAimDirection(_aimDirection); } }

    private void Awake()
    {
        _fieldOfView = Instantiate(_fieldOfViewElementPrefab).GetComponent<FieldOfView>();
        _fieldOfView.name = transform.name + " - " + _fieldOfView.name;
        _fieldOfView.SetLayerMask(layerMask);

        _fieldOfView.SetFoV(_fovAngle);
        _fieldOfView.SetViewDistance(_viewDistance);
        _fieldOfView.SetAimDirection(_aimDirection);
    }

    // Start is called before the first frame update
    void Start()
    {
        FieldOfViewAngle = _fovAngle;
        ViewDistance = _viewDistance;
        AimDirection = _aimDirection;
    }

    // Update is called once per frame
    void Update()
    {
        _fieldOfView.SetOrigin(transform.position);
    }
}
