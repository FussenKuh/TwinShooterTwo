using UnityEngine;
using System;
using System.Collections.Generic;

namespace FKS
{
    public static class ProjectileUtils3D
    {
        public static float CalculateProjectileFiringSolution(Vector3 firingPosition, Vector3 targetPosition, float speed)
        {
            // We're going to use the standard Projectile Motion formula ( https://en.wikipedia.org/wiki/Projectile_motion#Angle_.CE.B8_required_to_hit_coordinate_.28x.2Cy.29 )
            // to figure out our Angle (theta) required to hit coordinate. The key thing to remember is that our firing position must be reletave to (0,0,0)
            Vector3 targetTransform = targetPosition - firingPosition;
            Vector3 barrelTransform = Vector3.zero;

            float y = barrelTransform.y - targetTransform.y;
            targetTransform.y = barrelTransform.y = 0;
            float x = (targetTransform - barrelTransform).magnitude;
            float v = speed;
            float g = Physics.gravity.y;
            float sqrt = (v * v * v * v) - (g * ((g * (x * x)) + (2 * y * (v * v))));

            // Not enough range
            if (sqrt < 0)
            {
                Debug.LogWarning("Muzzle Velocity too slow. Firing Solution can't be calculated.");//haveFiringSolution = false;
                return -45.0f; // Return the angle that ought to get us the best distance
            }

            sqrt = Mathf.Sqrt(sqrt);
            return Mathf.Rad2Deg * Mathf.Atan(((v * v) - sqrt) / (g * x));
            // return Mathf.Rad2Deg * Mathf.Atan(((v * v) + sqrt) / (g * x)); // <-- If you're looking for a much more lobbed shot, use the '+' version instead.
        }

        public static List<Vector3> TraceProjectile(float argMuzzleVelocity, Transform argStartPositionandRotation, float argMaxTime)
        {
            List<Vector3> retVal = new List<Vector3>();
            float timeResolution = Time.fixedDeltaTime;
            Vector3 velocityVector = argStartPositionandRotation.forward * argMuzzleVelocity;

            Vector3 currentPosition = argStartPositionandRotation.position;

            for (float t = 0.0f; t < argMaxTime; t += timeResolution)
            {
                retVal.Add(currentPosition);
                currentPosition += velocityVector * timeResolution;
                velocityVector += Physics.gravity * timeResolution;
            }

            return retVal;
        }

        public static bool LOS(Transform me, Transform target, out Vector3 hitLoc)
        {
            bool retVal = false;
            RaycastHit rayHit;

            hitLoc = me.position;

            if (Physics.Raycast(me.position, ProjectileUtils3D.Direction(me.position, target.position), out rayHit))
            { // We hit something. Now, make sure it's the target. If not, then the target is NOT in our Line Of Sight
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

        public static Vector3 Direction(Vector3 myPosition, Vector3 targetPosition)
        {
            // Gets a vector that points from myPosition to targetPosition.
            return (targetPosition - myPosition).normalized;
        }



    }
}

