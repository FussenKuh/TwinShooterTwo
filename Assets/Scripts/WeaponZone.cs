using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponZone : MonoBehaviour
{
    TextMeshPro textMesh;
    CollectibleBehavior cb;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = gameObject.GetComponentInChildren<TextMeshPro>();
        cb = gameObject.GetComponentInChildren<CollectibleBehavior>();

        textMesh.SetText(cb.Item.Weapon.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
