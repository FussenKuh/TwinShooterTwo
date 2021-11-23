using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DetectIdle : MonoBehaviour
{
    [SerializeField]
    float _idleDurration = 3f;
    [SerializeField]
    Vector3 _startLocation;
    [SerializeField]
    float _distanceThreshold = 0.5f;

    [SerializeField]
    bool _debug = false;
    [SerializeField]
    bool _disabled = false;

    public Action OnDetectIdle;


    IEnumerator _DetectIdle()
    {
        while (true)
        {
            yield return new WaitForSeconds(_idleDurration);

            if (Vector3.Distance(_startLocation, transform.position) < _distanceThreshold)
            {
                if (_debug) { Debug.LogFormat("{0} has been idle for {1} seconds. Report it!", name, _idleDurration.ToString("N2")); }
                OnDetectIdle?.Invoke();
            }

            _startLocation = transform.position;
        }
    }


    private void OnEnable()
    {
        if (!_disabled)
        {
            _startLocation = transform.position;
            StartCoroutine(_DetectIdle());
        }
    }

    private void OnDisable()
    {
        if (_disabled)
        {
            StopAllCoroutines();
        }
    }

}
