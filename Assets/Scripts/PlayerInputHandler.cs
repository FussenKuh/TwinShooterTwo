using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{

    [SerializeField]
    BaseEntityMove _baseEntityMove = null;
    [SerializeField]
    Player _player = null;

    [SerializeField]
    Transform _turret = null;
    [SerializeField]
    BaseEntityRotate _turretRotationController = null;

    [SerializeField]
    Transform _bulletSpawnLocation = null;
    [SerializeField]
    GameObject _bulletPrefab = null;
    [SerializeField]
    Transform _shellSpawnLocation = null;
    [SerializeField]
    GameObject _shellPrefab = null;

    [SerializeField]
    float _speed = 5f;

    [SerializeField]
    float _shellSpeed = .5f;
    [SerializeField]
    float _shellSpeedVariation = 10f;

    [SerializeField]
    float _angleVariation = 3f;
    [SerializeField]
    float _shellAngleVariation = 10f;

    [SerializeField]
    float _fireRate = 0.5f;

    [SerializeField]
    [ReadOnly]
    float _elapsedTime = 0f;

    [SerializeField]
    int maxNumberOfBullets = 20;

    [SerializeField]
    int currentNumberOfBullets = 20;


    GameObject _shellParent;

    GameObject _bulletParent;

    [SerializeField]
    PlayerInput playerInput;

    private void Awake()
    {
        InitializeBulletAndShellContainers();

        currentNumberOfBullets = maxNumberOfBullets;
    }

    void InitializeBulletAndShellContainers()
    {
        _shellParent = GameObject.Find("Shell Parent");
        if (_shellParent == null)
        {
            _shellParent = new GameObject("Shell Parent");
        }
        _bulletParent = GameObject.Find("Bullet Parent");
        if (_bulletParent == null)
        {
            _bulletParent = new GameObject("Bullet Parent");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        impulseSource = GetComponent<Cinemachine.CinemachineImpulseSource>();

        this.MoveEvent += OnMoveEvent;
        this.LookEvent += OnLookEvent;

        //playerInput = gameObject.GetComponent<PlayerInput>();
        
        //playerInput.onControlsChanged += OnControlsChanged;
        //playerInput.onDeviceLost += OnDeviceLost;
        //playerInput.onDeviceRegained += OnDeviceRegained;

        //Debug.Log("Scheme: " + playerInput.currentControlScheme);

    }

    public void OnControlsChanged(PlayerInput pi)
    {
        Debug.Log("OnControlsChanged: " + pi.currentControlScheme);
    }

    public void OnDeviceLost(PlayerInput pi)
    {
        Debug.Log("OnDeviceLost: " + pi.currentControlScheme);
    }

    public void OnDeviceRegained(PlayerInput pi)
    {
        Debug.Log("OnDeviceRegained: " + pi.currentControlScheme);
    }

    private void OnDestroy()
    {
        this.MoveEvent -= OnMoveEvent;
        this.LookEvent -= OnLookEvent;

        //playerInput.onControlsChanged -= OnControlsChanged;
        //playerInput.onDeviceLost -= OnDeviceLost;
        //playerInput.onDeviceRegained -= OnDeviceRegained;
    }


    void UpdateBulletCount(int additionalBullets)
    {
        currentNumberOfBullets += additionalBullets;
    }

    // Update is called once per frame
    void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (firing)
        {
            triggerDown += Time.deltaTime;
            triggerDown = Mathf.Min(5, triggerDown);

            if (_elapsedTime > _fireRate  && currentNumberOfBullets > 0)
            {
                //TODO How does the player get bullets back? UpdateBulletCount(-1);
                _elapsedTime = 0f;
                GameObject tmpBullet = FKS.ProjectileUtils2D.SpawnProjectile(_bulletPrefab, _bulletSpawnLocation.position, _turret.right, _speed, _angleVariation, 0);
                tmpBullet.GetComponent<SpriteRenderer>().color = gameObject.GetComponent<SpriteRenderer>().color;
                try
                {
                    tmpBullet.transform.parent = _bulletParent.transform;
                }
                catch
                {
                    InitializeBulletAndShellContainers();
                    tmpBullet.transform.parent = _bulletParent.transform;
                }

                //                EffectsManager.Instance.BulletShell(_shellSpawnLocation.position, _turret.rotation);


                impulseMult = 0.2f;//triggerDown.Remap(0f, 5f, 0.1f, 0.5f);

                impulseSource.GenerateImpulse(_turret.right * impulseMult);

            }
        } 
        else
        {
            triggerDown -= (Time.deltaTime * 2f);
            triggerDown = Mathf.Max(0, triggerDown);
        }
    }

    Cinemachine.CinemachineImpulseSource impulseSource;

    public float impulseMult = 1.0f;

    public float triggerDown = 0f;


    void OnMoveEvent(object sender, OnDirectionArgs args)
    {
        if (args.Context.canceled)
        {
            _baseEntityMove.Moving = false;
            _baseEntityMove.MoveInDirection(Vector2.zero);
        }
        else
        {
            _baseEntityMove.Moving = true;
            _baseEntityMove.MoveInDirection(args.Direction.normalized);
        }
    }

    void OnLookEvent(object sender, OnDirectionArgs args)
    {
        if (args.Context.performed)
        {
            _turretRotationController.RotateToDirection(args.Direction.normalized);
        }
    }

    public event System.EventHandler<OnButtonArgs> FireEvent;
    public event System.EventHandler<OnDirectionArgs> MoveEvent;
    public event System.EventHandler<OnDirectionArgs> LookEvent;


    [SerializeField]
    Vector2 moveVector;
    [SerializeField]
    Vector2 lookVector;

    [SerializeField]
    bool firing = false;

    public class OnDirectionArgs : System.EventArgs
    {
        public Vector2 Direction { get; set; }
        public InputAction.CallbackContext Context { get; set; }
    }

    public class OnButtonArgs : System.EventArgs
    {
        public InputActionPhase Phase { get; set; }
        public InputAction.CallbackContext Context { get; set; }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //Debug.Log(context);
        moveVector = context.ReadValue<Vector2>();

        OnDirectionArgs args = new OnDirectionArgs();
        args.Direction = context.ReadValue<Vector2>(); ;
        args.Context = context;
        MoveEvent?.Invoke(this, args);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        //Debug.Log(context);
        lookVector = context.ReadValue<Vector2>();

        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            Vector3 tmp = Camera.main.ScreenToWorldPoint(lookVector);
            tmp.z = 0;
            lookVector = FKS.ProjectileUtils3D.Direction(transform.position, tmp);
        }

        OnDirectionArgs args = new OnDirectionArgs();
        args.Direction = lookVector;
        args.Context = context;
        LookEvent?.Invoke(this, args);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            firing = true;
        }
        else if (context.canceled)
        {
            firing = false;
        }

        OnButtonArgs args = new OnButtonArgs();
        args.Phase = context.phase;
        args.Context = context;
        FireEvent?.Invoke(this, args);
    }

}

