using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FKS
{
    namespace Utils
    {
        /// <summary>
        /// Utility class of time-based functions
        /// </summary>
        public class Time : Singleton<Time>
        {
            #region Private
            private static float fixedDeltaTime;
            private static Coroutine activeTimeAdjustment = null;
            private static float previousTimescale = 1f;

            private void Awake()
            {
                fixedDeltaTime = UnityEngine.Time.fixedDeltaTime;
            }

            private IEnumerator _Adjust(float timeScale, float duration)
            {
                previousTimescale = UnityEngine.Time.timeScale;

                UnityEngine.Time.timeScale = timeScale;
                UnityEngine.Time.fixedDeltaTime = fixedDeltaTime * UnityEngine.Time.timeScale; // Adjust fixed delta time according to the new timescale
                yield return new WaitForSecondsRealtime(duration);

                UnityEngine.Time.timeScale = previousTimescale;
                UnityEngine.Time.fixedDeltaTime = fixedDeltaTime * UnityEngine.Time.timeScale; // Adjust fixed delta time according to the new timescale

            }

            #endregion

            /// <summary>
            /// Adjusts the game's timescale for a given duration. Then, restores the previous timescale
            /// </summary>
            /// <param name="timeScale">The new timescale. timeScale must be greater than or equal to zero (0)</param>
            /// <param name="duration">The duration before returning to the previous time scale. Duration must be greater than or equal to zero (0)</param>
            /// <returns>True, if timeScale and duration are greater than or equal to 0, false otherwise</returns>
            public static bool AdjustTimeScale(float timeScale, float duration)
            {
                if (timeScale <= 0f || duration < 0f) { return false; }

                if (activeTimeAdjustment != null)
                {
                    // Restore the previous timescale
                    UnityEngine.Time.timeScale = previousTimescale;
                    UnityEngine.Time.fixedDeltaTime = fixedDeltaTime * UnityEngine.Time.timeScale; // Adjust fixed delta time according to the new timescale
                    // Stop the currently running timescale adjustment coroutine
                    Instance.StopCoroutine(activeTimeAdjustment);
                }

                // Kick off a coroutine to adjust the game's timescale
                activeTimeAdjustment = Instance.StartCoroutine(Instance._Adjust(timeScale, duration));

                return true;
            }

            /// <summary>
            /// Adjusts the game's timescale
            /// </summary>
            /// <param name="timeScale">The new timescale. timeScale must be greater than or equal to zero (0)</param>
            /// <returns>True, if timeScale is greater than or equal to 0, false otherwise</returns>
            public static bool AdjustTimeScale(float timeScale)
            {
                if (timeScale <= 0f) { return false; }

                if (activeTimeAdjustment != null)
                {
                    // set the previous timescale
                    previousTimescale = timeScale;
                    // Stop the currently running timescale adjustment coroutine
                    Instance.StopCoroutine(activeTimeAdjustment);
                }

                UnityEngine.Time.timeScale = timeScale;
                UnityEngine.Time.fixedDeltaTime = fixedDeltaTime * UnityEngine.Time.timeScale; // Adjust fixed delta time according to the new timescale
                return true;
            }

            /// <summary>
            /// Cancel any current timescale adjustment and return the timescale to its previous state
            /// </summary>
            public static void RestoreTimeScale()
            {
                // Restore the previous time scale
                UnityEngine.Time.timeScale = previousTimescale;
                UnityEngine.Time.fixedDeltaTime = fixedDeltaTime * UnityEngine.Time.timeScale; // Adjust fixed delta time according to the new timescale
                if (activeTimeAdjustment != null)
                {
                    // Stop the currently running timescale adjustment coroutine
                    Instance.StopCoroutine(activeTimeAdjustment);
                }
            }

        }
    }
}

