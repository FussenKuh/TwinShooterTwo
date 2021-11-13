using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Collectible", menuName = "Collectible", order = 1)]
public class Collectible: ScriptableObject
{
    [SerializeField]
    WeaponData _weapon;

    /// <summary>
    /// The value of the collectible
    /// </summary>
    public WeaponData Weapon { get { return _weapon; } }


}
