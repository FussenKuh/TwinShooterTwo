using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{

    static bool _exists = false;

    [SerializeField]
    Camera _camera;

    [SerializeField]
    Cinemachine.CinemachineVirtualCamera _virtualCamera;

    [SerializeField]
    Cinemachine.CinemachineTargetGroup _targetGroup;

    [SerializeField]
    Transform _cameraBounds;


    [SerializeField]
    Bounds calculatedBoundsV1;
    [SerializeField]
    Vector2 screenSize = Vector2.zero;

    [SerializeField]
    Vector3 _initialCameraBoundsPosition = new Vector3(0,29.5f,0);
    [SerializeField]
    Vector3 _initialCameraBoundsScale = new Vector3(45, 87, 1);

    public Bounds CalculatedBounds { get { return _camera.OrthographicBounds(); } }

    //public Vector3 CameraBounds
    //{
    //    set
    //    {
    //        _cameraBounds.localScale = value;
    //    }
    //}

    public void SetCameraBounds(Vector3 center, Vector2 dimensions)
    {
        _cameraBounds.localScale = dimensions;
        _cameraBounds.position = center;
    }


    public Camera SystemCamera { get { return _camera; } }
    public Cinemachine.CinemachineVirtualCamera SystemVirtualCamera { get { return _virtualCamera; } }


    public bool AddFollowTarget(Transform target)
    {
        bool retVal = false;
        if (_targetGroup.FindMember(target) == -1)
        {
            _targetGroup.AddMember(target, 1, 1);
            retVal = true;
        }

        return retVal;
    }

    public bool RemoveFollowTarget(Transform target)
    {
        bool retVal = false;
        if (_targetGroup.FindMember(target) >= 0)
        {
            _targetGroup.RemoveMember(target);
            retVal = true;
        }

        return retVal;
    }

    public bool AddFollowTargets(List<Transform> targets)
    {  
        bool retVal = false;

        foreach (Transform target in targets)
        {
            if (_targetGroup.FindMember(target) == -1)
            {
                _targetGroup.AddMember(target, 1, 1);
                retVal = true; // As long as at least one target from the group is added, we consider the whole operation a success

            }
        }

        return retVal;
    }

    public void Reset()
    {
        //SetCameraBounds(new Vector3(0, 6, 0), new Vector2(45, 40));
        SetCameraBounds(_initialCameraBoundsPosition, new Vector2(_initialCameraBoundsScale.x, _initialCameraBoundsScale.y));
        _virtualCamera.transform.position = new Vector3(0, 0, -10);
    }

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponentInChildren<Camera>();
        _virtualCamera = GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
        _targetGroup = GetComponentInChildren<Cinemachine.CinemachineTargetGroup>();

        _cameraBounds = gameObject.transform.Find("Camera Bounds");

        _initialCameraBoundsPosition = _cameraBounds.transform.position;
        _initialCameraBoundsScale = _cameraBounds.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        calculatedBoundsV1 = _camera.OrthographicBounds();
        screenSize.x = Screen.width;
        screenSize.y = Screen.height;
    }

    private void Awake()
    {
        if (_exists)
        {
            Destroy(gameObject);
        }
        else
        {
            _exists = true;
        }
    }
}
