using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FKS
{
    namespace Effects
    {
        /// <summary>
        /// Attached to a GameObject with a Rigidbody or Rigidbody2D, this MonoBehavior will alter the Unity TimeScale based on physics interactions
        /// </summary>
        public class AlterTimeViaPhysics : MonoBehaviour
        {
            [Header("TimeScale Configuration Info:")]
            [SerializeField]
            [Tooltip("The collision must register this much force before the TimeScale effect will trigger")]
            float _forceThreshold = 8f;
            [SerializeField]
            [Tooltip("Duration to alter TimeScale after a collision is detected. (TimeScale will return to previously saved value after this duration)")]
            float _duration = 0.3f;
            [SerializeField]
            [Tooltip("TimeScale used for collision-based adjustments")]
            float _collisionTimeScale = 0.5f;
            [SerializeField]
            [Tooltip("TimeScale used for trigger-based adjustments (trigger enter)")]
            float _triggerEnterTimeScale = 0.5f;
            [SerializeField]
            [Tooltip("TimeScale used for trigger-based adjustments (trigger exit)")]
            float _triggerExitTimeScale = 0.5f;
            [SerializeField]
            [Tooltip("TimeScale used for joint breaking-based adjustments")]
            float _jointBreakTimeScale = 0.5f;

            [Header("Collider Configuration Info:")]
            [SerializeField]
            [Tooltip("Should TimeScale be altered upon collisions?")]
            bool _alterOnCollision    = true;
            [SerializeField]
            [Tooltip("Should TimeScale be altered upon entering a trigger collider?")]
            bool _alterOnTriggerEnter = false;
            [SerializeField]
            [Tooltip("Should TimeScale be altered upon exiting a trigger collider?")]
            bool _alterOnTriggerExit  = false;
            [SerializeField]
            [Tooltip("Should TimeScale be altered upon a joint breaking?")]
            bool _alterOnJointBreak   = false;
            [SerializeField]
            [Tooltip("Should TimeScale be restored upon exiting a trigger collider? Note: When enabled, when the object enters a trigger collider, the current Time.TimeScale overrides the triggerExitTimeScale value")]
            bool _restoreTimescaleOnTriggerExit = true;

            [SerializeField]
            [ReadOnly]
            [Tooltip("The calculated force of the most recent collision")]
            float _collisionForce;

            /// <summary>
            /// The collision must register this much force before the TimeScale effect will trigger
            /// </summary>
            public float ForceThreshold { get { return _forceThreshold; } set { _forceThreshold = value; } }
            /// <summary>
            /// Duration to alter TimeScale after a collision is detected
            /// </summary>
            public float Duration { get { return _duration; } set { _duration = value; } }
            /// <summary>
            /// TimeScale used for collision-based adjustments
            /// </summary>
            public float CollisionTimeScale { get { return _collisionTimeScale; } set { _collisionTimeScale = value; } }
            /// <summary>
            /// TimeScale used for trigger-based adjustments (trigger enter)
            /// </summary>
            public float TriggerEnterTimeScale { get { return _triggerEnterTimeScale; } set { _triggerEnterTimeScale = value; } }
            /// <summary>
            /// TimeScale used for trigger-based adjustments (trigger exit)
            /// </summary>
            public float TriggerExitTimeScale { get { return _triggerExitTimeScale; } set { _triggerExitTimeScale = value; } }
            /// <summary>
            /// TimeScale used for joint breaking-based adjustments
            /// </summary>
            public float JointBreakTimeScale { get { return _jointBreakTimeScale; } set { _jointBreakTimeScale = value; } }

            /// <summary>
            /// Should TimeScale be altered upon collisions?
            /// </summary>
            public bool AlterOnCollision { get { return _alterOnCollision; } set { _alterOnCollision = value; } }
            /// <summary>
            /// Should TimeScale be altered upon entering a trigger collider?
            /// </summary>
            public bool AlterOnTriggerEnter { get { return _alterOnTriggerEnter; } set { _alterOnTriggerEnter = value; } }
            /// <summary>
            /// Should TimeScale be altered upon exiting a trigger collider?
            /// </summary>
            public bool AlterOnTriggerExit { get { return _alterOnTriggerExit; } set { _alterOnTriggerExit = value; } }
            /// <summary>
            /// Should TimeScale be altered upon a joint breaking?
            /// </summary>
            public bool AlterOnJointBreak { get { return _alterOnJointBreak; } set { _alterOnJointBreak = value; } }
            /// <summary>
            /// Should TimeScale be restored upon exiting a trigger collider? Note: When enabled, when the object enters a trigger collider, the current Time.TimeScale overrides the triggerExitTimeScale value
            /// </summary>
            public bool RestoreTimeScaleOnTriggerExit { get { return _restoreTimescaleOnTriggerExit; } set { _restoreTimescaleOnTriggerExit = value; } }


            #region Private Functions
            void AlterTimescaleTimeBased(float timeScale, float duration, float forceThreshold)
            {
                if (forceThreshold >= _forceThreshold)
                {
                    FKS.Utils.Time.AdjustTimeScale(timeScale, duration);
                }
            }

            void AlterTimescaleNonTimeBased(float timeScale)
            {
                FKS.Utils.Time.AdjustTimeScale(timeScale);
            }
            #endregion

            #region Physics2D
            public void OnCollisionEnter2D(Collision2D collision)
            {
                if (!_alterOnCollision) { return; }
                _collisionForce = collision.relativeVelocity.magnitude;
                AlterTimescaleTimeBased(_collisionTimeScale, _duration, _collisionForce);
            }

            public void OnTriggerEnter2D(Collider2D collision)
            {
                if (!_alterOnTriggerEnter) { return; }

                if (_restoreTimescaleOnTriggerExit)
                {
                    _triggerExitTimeScale = Time.timeScale;
                }

                AlterTimescaleNonTimeBased(_triggerEnterTimeScale);
            }

            public void OnTriggerExit2D(Collider2D collision)
            {
                if (_alterOnTriggerExit || _restoreTimescaleOnTriggerExit)
                {
                    AlterTimescaleNonTimeBased(_triggerExitTimeScale);
                }
            }

            public void OnJointBreak2D(Joint2D joint)
            {
                if (!_alterOnJointBreak) { return; }
                AlterTimescaleTimeBased(_jointBreakTimeScale, _duration, _forceThreshold);
            }
            #endregion

            #region Physics3D
            private void OnCollisionEnter(Collision collision)
            {
                if (!_alterOnCollision) { return; }
                _collisionForce = collision.relativeVelocity.magnitude;
                AlterTimescaleTimeBased(_collisionTimeScale, _duration, _collisionForce);
            }

            public void OnTriggerEnter(Collider collision)
            {
                if (!_alterOnTriggerEnter) { return; }

                if (_restoreTimescaleOnTriggerExit)
                {
                    _triggerExitTimeScale = Time.timeScale;
                }

                AlterTimescaleNonTimeBased(_triggerEnterTimeScale);
            }

            public void OnTriggerExit(Collider collision)
            {
                if (_alterOnTriggerExit || _restoreTimescaleOnTriggerExit)
                {
                    AlterTimescaleNonTimeBased(_triggerExitTimeScale);
                }
            }

            private void OnJointBreak(float breakForce)
            {
                if (!_alterOnJointBreak) { return; }
                AlterTimescaleTimeBased(_jointBreakTimeScale, _duration, _forceThreshold);
            }
            #endregion

        }
    }
}
