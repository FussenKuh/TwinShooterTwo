using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(WorldObjectID))]
public class WorldObjectSize : MonoBehaviour
{
    [SerializeField]
    Vector2 _size = Vector2.one;
    [SerializeField]
    [Range(0f, 1f)]
    public float _floorColliderOffset = 0.5f;


    [Header("Debug Info")]
    [SerializeField]
    [ReadOnly]
    public int _collisionStayCount = 0;
    [SerializeField]
    [ReadOnly]
    public int _triggerStayCount = 0;

    SpriteRenderer _spriteRenderer;
    BoxCollider2D _boxCollider2D;
    WorldObjectID _worldObjectID;

    public Color Color { get { return _spriteRenderer.color; } set { _spriteRenderer.color = value; } }
    public Vector2 Size 
    { 
        get 
        {
            //if (_size != _spriteRenderer.size) 
            //{ 
            //    Debug.LogWarning(name + " Sprite Renderer size does not match WorldObject size. Updating WorldObject size to match Sprite Renderer size.");
            //    _size = _spriteRenderer.size;
            //} 
            //return _spriteRenderer.size; 
            if (new Vector3(_size.x, _size.y) != transform.localScale)
            {
                Debug.LogWarning(name + " transform.localScale does not match WorldObject size. Updating WorldObject size to match transform.localScale.");
                _size = transform.localScale;
            }
            return _size;
        } 
    }
    public Vector2 ColliderSize 
    { 
        get 
        { 
            return _boxCollider2D.size; 
        } 
    }

    public void UpdateObject(WorldObjectData.ObjectType newType, Vector2 newDimension)
    {
        _size = newDimension;
        _worldObjectID.Data.Type = newType;

        // Ensure that the gameObject's scale didn't get accidentally altered
        transform.localScale = _size;

        // Update the visuals
        _spriteRenderer.drawMode = SpriteDrawMode.Simple;
        //_spriteRenderer.size = _size;

        // Update the collision detection based on the type of object we are
        switch (_worldObjectID.Data.Type)
        {
            case WorldObjectData.ObjectType.FLOOR:
                _boxCollider2D.isTrigger = true;
                //_boxCollider2D.size = _size - new Vector2(_floorColliderOffset, _floorColliderOffset);
                break;
            case WorldObjectData.ObjectType.WALL:
                _boxCollider2D.isTrigger = false;
                //_boxCollider2D.size = _size;
                break;
            default:
                Debug.LogWarning(name + ": We don't know how to process type (" + _worldObjectID.Data.Type + "). Treating it as a WALL");
                break;
        }
    }


    //public void UpdateObject(WorldObjectData.ObjectType newType, Vector2 newDimension)
    //{
    //    _size = newDimension;
    //    _worldObjectID.Data.Type = newType;

    //    // Ensure that the gameObject's scale didn't get accidentally altered
    //    transform.localScale = Vector3.one;

    //    // Update the visuals
    //    _spriteRenderer.drawMode = SpriteDrawMode.Sliced;
    //    _spriteRenderer.size = _size;

    //    // Update the collision detection based on the type of object we are
    //    switch (_worldObjectID.Data.Type)
    //    {
    //        case WorldObjectData.ObjectType.FLOOR:
    //            _boxCollider2D.isTrigger = true;
    //            _boxCollider2D.size = _size - new Vector2(_floorColliderOffset, _floorColliderOffset);
    //            break;
    //        case WorldObjectData.ObjectType.WALL:
    //            _boxCollider2D.isTrigger = false;
    //            _boxCollider2D.size = _size;
    //            break;
    //        default:
    //            Debug.LogWarning(name + ": We don't know how to process type (" + _worldObjectID.Data.Type + "). Treating it as a WALL");
    //            break;
    //    }
    //}

    #region Collision/Trigger Region
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log(name + " Entered collision with " + collision.gameObject.name);
    //    _collisionStayCount = 0;
    //}
    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    Debug.Log(name + " Exited collision with " + collision.gameObject.name);
    //}

    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    _collisionStayCount++;
    //}

    //private void OnTriggerEnter2D(Collider2D collider)
    //{
    //    Debug.Log(name + " Entered trigger with " + collider.name);
    //    _triggerStayCount = 0;
    //}

    //private void OnTriggerExit2D(Collider2D collider)
    //{
    //    Debug.Log(name + " Exited trigger with " + collider.name);
    //}

    //private void OnTriggerStay2D(Collider2D collider)
    //{
    //    _triggerStayCount++;
    //}
    #endregion

    private void Start()
    {
        UpdateObject(_worldObjectID.Data.Type, _size);
    }

    private void Awake()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        _worldObjectID = gameObject.GetComponent<WorldObjectID>();
    }

}
