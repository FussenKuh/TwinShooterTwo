using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
[RequireComponent(typeof(Animator))]
public class SpotLight : MonoBehaviour
{
    Light2D _light;
    Animator _animator;

    [SerializeField]
    float _startDelay = 0f;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
        _light.enabled = false;

        _animator = GetComponent<Animator>();
        _animator.enabled = false;
    }


    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(_startDelay);
        _light.enabled = true;
        _animator.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartDelay());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
