using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomLayout
{
    public class MovingDemo : MonoBehaviour
    {

        [SerializeField]
        BaseEntityMove _baseEntityMove = null;

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

            _baseEntityMove.Moving = false;

            //if (Input.GetMouseButton(0))
            //{
            //    if (_elapsedTime > _fireRate)
            //    {
            //        _elapsedTime = 0f;
            //        GameObject tmpBullet = FKS.ProjectileUtils2D.SpawnProjectile(_bulletPrefab, _bulletSpawnLocation.position, _turret.right, _speed, _angleVariation, 0);

            //        tmpBullet.transform.parent = _bulletParent.transform;

            //        GameObject tmpShell = FKS.ProjectileUtils2D.SpawnProjectile(_shellPrefab, _shellSpawnLocation.position, FKS.Utils.UtilsClass.ApplyRotationToVector(_turret.right, 90), _shellSpeed, _shellAngleVariation, _shellSpeedVariation);

            //        tmpShell.transform.parent = _shellParent.transform;

            //        if (tmpBullet != null)
            //        {
            //            Destroy(tmpBullet, 2f);
            //        }

            //    }
            //}

            //if (Input.GetMouseButton(1))
            //{
            //    _turretRotationController.RotateToDirection(FKS.Utils.UtilsClass.GetDirToMouse(_turret.position));
            //}

            Vector2 movementDirection = Vector2.zero;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                _baseEntityMove.Moving = true;
                movementDirection.x += -1;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                _baseEntityMove.Moving = true;
                movementDirection.x += 1;
            }
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                _baseEntityMove.Moving = true;
                movementDirection.y += 1;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                _baseEntityMove.Moving = true;
                movementDirection.y += -1;
            }

            _baseEntityMove.MoveInDirection(movementDirection.normalized);
        }
    }
}
