using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingDemo : MonoBehaviour
{

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


    GameObject _shellParent;

    GameObject _bulletParent;

    // Start is called before the first frame update
    void Start()
    {
        _shellParent = new GameObject("Shell Parent");
        _bulletParent = new GameObject("Bullet Parent");
    }




    // Update is called once per frame
    void Update()
    {
        _elapsedTime += Time.deltaTime;


        if (Input.GetMouseButton(0))
        {
            if (_elapsedTime > _fireRate)
            {
                _elapsedTime = 0f;
                GameObject tmpBullet = FKS.ProjectileUtils2D.SpawnProjectile(_bulletPrefab, _bulletSpawnLocation.position, _turret.right, _speed, _angleVariation,0);

                tmpBullet.transform.parent = _bulletParent.transform;

                GameObject tmpShell = FKS.ProjectileUtils2D.SpawnProjectile(_shellPrefab, _shellSpawnLocation.position, FKS.Utils.UtilsClass.ApplyRotationToVector(_turret.right,90), _shellSpeed, _shellAngleVariation, _shellSpeedVariation);

                tmpShell.transform.parent = _shellParent.transform;

                if (tmpBullet != null)
                {
                    Destroy(tmpBullet, 2f);
                }

            }
        }
        

        if (Input.GetMouseButton(1))
        {
//            _turret.rotation = Quaternion.Euler(0, 0, FKS.Utils.UtilsClass.GetAngleFromVector(FKS.Utils.UtilsClass.GetDirToMouse(_turret.position)));

            _turretRotationController.RotateToDirection(FKS.Utils.UtilsClass.GetDirToMouse(_turret.position));
        }
    }
}
