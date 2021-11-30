using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTester : MonoBehaviour
{

    public WorldObjectSize _floor;

    public Vector2 _dimensions;
    public bool _update;

    public Color _color;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_update)
        {
            // do stuff
            _floor.UpdateObject(WorldObjectData.ObjectType.FLOOR, _dimensions);
            _floor.Color = _color;
        }
    }
}
