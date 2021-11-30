using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class WorldObjectData
{
    public enum ObjectType { FLOOR, WALL }

    public ObjectType Type;
    public float Friction;

}
