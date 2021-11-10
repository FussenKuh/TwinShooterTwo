using UnityEngine;
using System;

namespace FKS
{
    namespace Utils
    {
        public static class Rand
        {
            #region Private
            private static System.Random random;

            private static void Init()
            {
                if (random == null)
                {
                    Debug.Log("[" + UnityEngine.Time.time + "]" + "[FKS] Initializing new Random generator...");
                    random = new System.Random();
                }
            }
            #endregion


            public static Vector3 RandomPointOnXYCircle(Vector3 center, float radius)
            {
                float angle = UnityEngine.Random.Range(0, 2f * Mathf.PI);
                return center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            }

            /// <summary>
            /// Sets the random number generator's seed
            /// </summary>
            /// <param name="seed">The seed for the generator</param>
            public static void RandomSeed(int seed)
            {
                Debug.Log("[" + UnityEngine.Time.time + "]" + "[FKS] Initializing new Random generator with seed " + seed + "...");
                random = new System.Random(seed);
            }

            /// <summary>
            /// Generates a random integer between 'min' (inclusively) and 'max' (exclusively)
            /// </summary>
            /// <param name="min">The inclusive minimum value of the random number</param>
            /// <param name="max">The exclusive maximum value of the random number</param>
            /// <returns></returns>
            public static int Random(int min, int max)
            {
                Init();
                return random.Next(min, max);
            }

            /// <summary>
            /// Generates a random float between 'min' (inclusively) and 'max' (inclusively)
            /// </summary>
            /// <param name="min">The inclusive minimum value of the random number</param>
            /// <param name="max">The inclusive maximum value of the random number</param>
            /// <returns></returns>
            public static float Random(float min, float max)
            {
                Init();

                // Perform arithmetic in double type to avoid overflowing
                double range = (double)float.MaxValue - (double)float.MinValue;
                double sample = random.NextDouble();
                double scaled = (sample * range) + float.MinValue;
                float f = (float)scaled;

                return f.Remap(float.MinValue, float.MaxValue, min, max);
            }

            /// <summary>
            /// Generates a random enumeration value. Example Usage: RandomEnumValue<System.DayOfWeek>() --> Might return "Tuesday"
            /// </summary>
            /// <typeparam name="T">The Enumeration Type</typeparam>
            /// <returns>Random value from the enumeration</returns>
            public static T RandomEnumValue<T>()
            {
                var v = System.Enum.GetValues(typeof(T));
                return (T)v.GetValue(new System.Random().Next(v.Length));
            }

        }
    }
}

