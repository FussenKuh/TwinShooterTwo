using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MyColor : MonoBehaviour
{
    // Start is called before the first frame update

    SpriteRenderer sr;

    [SerializeField]
    [ColorUsage(true,true)]
    Color _color;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = _color;
    }

    // Update is called once per frame
    void Update()
    {
        sr.color = _color;
    }
}
