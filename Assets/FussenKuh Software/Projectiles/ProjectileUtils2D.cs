using UnityEngine;
using System;
using System.Collections.Generic;

namespace FKS
{
    public static class ProjectileUtils2D
    {

        /// <summary>
        /// Calculates the firing angle needed for a projectile with the given speed to hit a target from the given firing position.
        /// </summary>
        /// <param name="solution">The returned solution angle. Will be 0 if a valid solution can't be calculated</param>
        /// <param name="firingPosition">The start position of the projectile</param>
        /// <param name="targetPosition">The position of the target</param>
        /// <param name="speed">The speed of the projectile</param>
        /// <param name="lobbed">Set to 'True' for an angle that produces a solution that's more lobbed</param>
        /// <returns>True if a solution is found, otherwise, false</returns>
        public static bool CalculateProjectileFiringSolution(out float solution, Vector3 firingPosition, Vector3 targetPosition, float speed, bool lobbed=false)
        {
            // We're going to use the standard Projectile Motion formula ( https://en.wikipedia.org/wiki/Projectile_motion#Angle_.CE.B8_required_to_hit_coordinate_.28x.2Cy.29 )
            // to figure out our Angle (theta) required to hit coordinate. The key thing to remember is that our firing position must be reletave to (0,0,0)
            Vector3 targetTransform = targetPosition - firingPosition;
            Vector3 barrelTransform = Vector3.zero;

            float y = barrelTransform.y - targetTransform.y;
            targetTransform.y = barrelTransform.y = 0;
            float x = (targetTransform - barrelTransform).magnitude;
            float v = speed;
            float g = Physics2D.gravity.y;
            float sqrt = (v * v * v * v) - (g * ((g * (x * x)) + (2 * y * (v * v))));

            // Not enough range
            if (sqrt < 0)
            {
                //Muzzle Velocity too slow. Firing Solution can't be calculated
                solution = 0; // Configure a pointless angle as the out argument
                return false; // Return 'False' since we can't hit the target since our muzzle velocity is too slow
            }

            sqrt = Mathf.Sqrt(sqrt);

            if (targetTransform.x < barrelTransform.x)
            {
                // Target is to the left of the origin, flip the calculation to account for going left
                if (!lobbed)
                {
                    solution = 180 - ((Mathf.Rad2Deg * Mathf.Atan(((v * v) - sqrt) / (g * x))) * -1);
                    return true;
                }
                else
                {
                    solution = 180 - ((Mathf.Rad2Deg * Mathf.Atan(((v * v) + sqrt) / (g * x))) * -1);
                    return true;
                    // If you're looking for a much more lobbed shot, use the '+' version instead.
                }
            }
            else
            {
                if (!lobbed)
                {
                    solution = (Mathf.Rad2Deg * Mathf.Atan(((v * v) - sqrt) / (g * x))) * -1;
                    return true;
                }
                else
                {
                    solution = (Mathf.Rad2Deg * Mathf.Atan(((v * v) + sqrt) / (g * x))) * -1;
                    return true;
                    // If you're looking for a much more lobbed shot, use the '+' version instead.
                }
            }

        }


        /// <summary>
        /// Given a projectile's initial position and rotation, create a list of Vector3 of points that trace out 
        /// the projectile's trajectory -- Useful for rendering flight paths with a LineRenderer component.
        /// 
        /// This function assumes that 'right' is considered 'forward.'
        /// </summary>
        /// <param name="speed">The speed of the projectile to trace</param>
        /// <param name="startPositionandRotation"></param>
        /// <param name="maxTime">The lenght of time to perform the trace</param>
        /// <returns>a list of Vector3 representing the projectile's trajctory</returns>
        public static List<Vector3> TraceProjectile(float speed, Transform startPositionandRotation, float maxTime)
        {
            List<Vector3> retVal = new List<Vector3>();
            float timeResolution = Time.fixedDeltaTime;
            Vector3 velocityVector = startPositionandRotation.right * speed;

            Vector3 currentPosition = startPositionandRotation.position;

            Vector3 gravity = Physics2D.gravity;

            for (float t = 0.0f; t < maxTime; t += timeResolution)
            {
                retVal.Add(currentPosition);
                currentPosition += velocityVector * timeResolution;
                velocityVector += gravity * timeResolution;
            }

            return retVal;
        }

        /// <summary>
        /// Returns whether or not the 'origin' entity can see the 'target' entity.
        /// If so, return 'True' and update the out argument 'hitLoc' with the position the LOS ray intersected with the 'target' entity.
        /// Otherwise, return 'False' and return the position of the 'me' entity. 
        /// </summary>
        /// <param name="origin">The entity to cast the LOS ray from</param>
        /// <param name="target">The entity you're trying to see</param>
        /// <param name="hitLoc">The LOS ray intersection point if the target is seen, otherwise, the 'origin' entity</param>
        /// <returns>"True' if the target entity can be seen, otherwise, false.</returns>
        public static bool LOS(Transform origin, Transform target, out Vector2 hitLoc)
        {
            bool retVal = false;
            RaycastHit2D rayHit;

            hitLoc = origin.position;

            rayHit = Physics2D.Raycast(origin.position, ProjectileUtils2D.Direction(origin.position, target.position));
            if (rayHit.collider != null)
            { // We hit something. Now, make sure it's the target. If not, then the target is NOT in our Line Of Sight
//                Debug.Log("We hit: " + rayHit.collider.name);
                if (rayHit.transform.name == target.name)
                {
                    hitLoc = rayHit.point;
                    retVal = true;
                }
                else
                {
                    retVal = false;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Calculates a vector normalized with a magnitude of 1 pointing in the direction of the target.
        /// </summary>
        /// <param name="myPosition">The origin position</param>
        /// <param name="targetPosition">The target position </param>
        /// <returns>The normalized direction of the target</returns>
        public static Vector3 Direction(Vector3 myPosition, Vector3 targetPosition)
        {
            // Gets a vector that points from myPosition to targetPosition.
            return (targetPosition - myPosition).normalized;
        }

        /// <summary>
        /// Spawns an object applying a velocity in the provided direction
        /// </summary>
        /// <param name="prefab">The object to spawn</param>
        /// <param name="origin">The location to spawn the object</param>
        /// <param name="direction">The direction to "fire" the object</param>
        /// <param name="speed">The desires speed of the projectile (units/second)</param>
        /// <returns>The spawned GameObject or null if the object could not be spawned</returns>
        public static GameObject SpawnProjectile(GameObject prefab, Vector3 origin, Vector2 direction, float speed)
        {
            GameObject retVal = GameObject.Instantiate(prefab, origin, prefab.transform.rotation);

            // The prefab had no rigidbody. Exit.
            Rigidbody2D rb = retVal.GetComponent<Rigidbody2D>();
            if (rb == null) { GameObject.Destroy(retVal); return null; }

            rb.velocity = direction * speed;

            return retVal;
        }

        /// <summary>
        /// Spawns an object applying a velocity in the provided direction
        /// </summary>
        /// <param name="prefab">The object to spawn</param>
        /// <param name="origin">The location to spawn the object</param>
        /// <param name="direction">The direction to "fire" the object</param>
        /// <param name="speed">The desires speed of the projectile</param>
        /// <param name="directionVariation">Randomly vary the direction by this many degrees in either direction (must be positive)</param>
        /// <param name="speedVariation">Randomly vary the final speed by this percentage of provided speed. (must be positive)</param>
        /// <returns></returns>
        public static GameObject SpawnProjectile(GameObject prefab, Vector3 origin, Vector2 direction, float speed, float directionVariation, float speedVariation)
        {
            // Calculate a random angle and define a new direction vector
            float randomAngle = UnityEngine.Random.Range(-Mathf.Abs(directionVariation), Mathf.Abs(directionVariation));
            Vector3 newDirection = FKS.Utils.UtilsClass.ApplyRotationToVector(direction, randomAngle);
            // Calculate a random speed adjustment percentage
            float randomSpeedAdjust = UnityEngine.Random.Range(-Mathf.Abs(speedVariation), Mathf.Abs(speedVariation)) / 100;

            // Spawn the projectile using the newly calculated values
            return SpawnProjectile(prefab, origin, newDirection, speed + (speed * randomSpeedAdjust));

        }


    }
}

