using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraEffects : MonoBehaviour
{

    public VolumeProfile _profile;

    [SerializeField]
    float _chromaticAberrationDuration = 0.1f;

    public AnimationCurve _chromaticAberrationCurve;

    public bool DebugTest = false;

    IEnumerator _ChromaticAberrationShakeV2(float duration)
    {
        if (duration <= 0) { yield break; }


        if (_profile)
        {
            ChromaticAberration ca;
            if (_profile.TryGet(out ca))
            {
                float elapsedTime = 0;

                while (elapsedTime <= duration)
                {
                    ca.intensity.value = _chromaticAberrationCurve.Evaluate(elapsedTime.Remap(0, duration, _chromaticAberrationCurve[0].time, 
                        _chromaticAberrationCurve[_chromaticAberrationCurve.length - 1].time));

                    yield return new WaitForEndOfFrame();
                    elapsedTime += Time.deltaTime;
                }
                ca.intensity.value = 0f;
            }
        }

        yield break;
    }

    [SerializeField]
    float _depthOfFielStart;
    [SerializeField]
    float _depthOfFieldEnd;
    [SerializeField]
    AnimationCurve _depthOfFieldCurve;
    [SerializeField]
    float _depthOfFieldDuration = 0.1f;

    [SerializeField]
    float impulseMult = 0.2f;
    Cinemachine.CinemachineImpulseSource impulseSource;
    IEnumerator _DepthOfFieldShake(float duration)
    {
        if (duration <= 0) { yield break; }


        if (_profile)
        {
            DepthOfField dof;
            if (_profile.TryGet(out dof))
            {
                float elapsedTime = 0;

                while (elapsedTime <= duration)
                {
                    float tmpValue = _depthOfFieldCurve.Evaluate(elapsedTime.Remap(0, duration, _depthOfFieldCurve[0].time,
                        _depthOfFieldCurve[_depthOfFieldCurve.length - 1].time));

                    dof.focusDistance.value = tmpValue.Remap(_depthOfFieldCurve[0].time, _depthOfFieldCurve[_depthOfFieldCurve.length - 1].time, _depthOfFieldEnd, _depthOfFielStart);

                    yield return new WaitForEndOfFrame();
                    elapsedTime += Time.deltaTime;
                }
                dof.focusDistance.value = 10;
            }
        }

        yield break;
    }

    IEnumerator _ChromaticAberrationShake(float duration)
    {
        if (duration <= 0) { yield break; }

        if (_profile)
        {
            ChromaticAberration ca;
            if (_profile.TryGet(out ca))
            {
                float dur = duration / 2;
                float steps = 1 / dur;
                float orig = ca.intensity.value;
                ca.intensity.value = 1f;

                while (dur >= 0)
                {
                    dur -= Time.deltaTime;
                    ca.intensity.value += steps;
                    yield return new WaitForEndOfFrame();
                }
                dur = duration / 2;
                while (dur >= 0)
                {
                    dur -= Time.deltaTime;
                    ca.intensity.value -= steps;
                    yield return new WaitForEndOfFrame();
                }

                ca.intensity.value = 0f;
            }
        }

        yield break;
    }

    void OnPlayerHit()
    {
        StartCoroutine(_ChromaticAberrationShakeV2(_chromaticAberrationDuration));
        //StartCoroutine(_DepthOfFieldShake(_depthOfFieldDuration));
        impulseSource.GenerateImpulseWithForce(impulseMult);
    }


    private void Awake()
    {
        Messenger.AddListener("PlayerHit", OnPlayerHit);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener("PlayerHit", OnPlayerHit);
    }

    // Start is called before the first frame update
    void Start()
    {
        impulseSource = GetComponent<Cinemachine.CinemachineImpulseSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (DebugTest)
        {
            DebugTest = false;
            //StartCoroutine(_ChromaticAberrationShakeV2(_chromaticAberrationDuration));

            StartCoroutine(_DepthOfFieldShake(_depthOfFieldDuration));

            impulseSource.GenerateImpulseWithForce(impulseMult);
        }
    }
}
