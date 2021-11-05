using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomLayout
{
    public class shellBehavior : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The sprite renderer to modify")]
        SpriteRenderer _spriteRenderer = null;

        [Header("Layer Switching Configuration:")]
        [SerializeField]
        [Tooltip("The name of sprite sorting layer the shell will start on")]
        string _startingLayer = "";
        [SerializeField]
        [Tooltip("The name of sprite sorting layer the shell will end on")]
        string _endingLayer = "";
        [SerializeField]
        [Tooltip("The delay between switching form starting to ending layer")]
        float _layerSwitchDelay = 0.5f;

        [SerializeField]
        [Tooltip("Allows for adjusting the shell's scale while transiting from starting to ending layer. Example Usage: Grow the shell's size early to simulate the shell being 'higher' in the scene")]
        AnimationCurve _shellSizeCurve = null;



        [Header("Physics Options:")]
        [SerializeField]
        [Tooltip("If true, physics interactions will be enabled after a predefined delay")]
        bool _enablePhysicsInteraction = false;
        [SerializeField]
        [Range(0, 100)]
        int _chanceToEnablePhysicsInteractions = 100;
        [SerializeField]
        [Tooltip("Enable physics interactions after this amount of delay")]
        float _physicsInteractionDelay = 1f;


        Vector3 _originalScale;

        IEnumerator SwitchLayers()
        {
            float elapsedTime = 0f;

            _spriteRenderer.sortingLayerName = _startingLayer;
            while (elapsedTime < _layerSwitchDelay)
            {
                float scale = _shellSizeCurve.Evaluate(elapsedTime.Remap(0, _layerSwitchDelay, 0, _shellSizeCurve.keys[_shellSizeCurve.length - 1].time));
                transform.localScale = _originalScale * scale;
                yield return new WaitForEndOfFrame();
                elapsedTime += Time.deltaTime;
            }
            _spriteRenderer.sortingLayerName = _endingLayer;
            transform.localScale = _originalScale;
        }

        IEnumerator EnablePhysics()
        {
            yield return new WaitForSeconds(_physicsInteractionDelay);

            Collider2D tmp = GetComponent<Collider2D>();
            if (tmp != null)
            {
                tmp.enabled = true;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            _originalScale = transform.localScale;

            if (_spriteRenderer != null)
            {
                StartCoroutine(SwitchLayers());
            }

            if (_enablePhysicsInteraction && FKS.Utils.UtilsClass.TestChance(_chanceToEnablePhysicsInteractions))
            {
                StartCoroutine(EnablePhysics());
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
