using System.Diagnostics;
using UnityEngine;

public static class ExtensionMethods
{

    #region Number Helpers
    /// <summary>
    /// Remaps the float from one range to the equivalent value in a different range
    /// </summary>
    /// <param name="value">The value to map</param>
    /// <param name="originlRangeStart">The starting point of the original range</param>
    /// <param name="originlRangeEnd">The ending point of the original range</param>
    /// <param name="newRangeStart">The starting point of the new range</param>
    /// <param name="newRangeEnd">The ending point of the new range</param>
    /// <param name="clampResults">If true, ensures that the new value will not exceed the start/end points of the new range</param>
    /// <returns></returns>
    public static float Remap(this float value, float originlRangeStart, float originlRangeEnd, float newRangeStart, float newRangeEnd, bool clampResults=true)
    {
        float retVal = (value - originlRangeStart) / (originlRangeEnd - originlRangeStart) * (newRangeEnd - newRangeStart) + newRangeStart;

        if (clampResults)
        {
            if (retVal < newRangeStart)
            {
                retVal = newRangeStart;
            }
            else if (retVal > newRangeEnd)
            {
                retVal = newRangeEnd;
            }
        }

        return retVal;
    }

    /// <summary>
    /// Remaps the integer from one range to the equivalent value in a different range. Ensures that the new value will not exceed the start/end points of the new range
    /// </summary>
    /// <param name="value">The value to map</param>
    /// <param name="originlRangeStart">The starting point of the original range</param>
    /// <param name="originlRangeEnd">The ending point of the original range</param>
    /// <param name="newRangeStart">The starting point of the new range</param>
    /// <param name="newRangeEnd">The ending point of the new range</param>
    /// <param name="clampResults">If true, ensures that the new value will not exceed the start/end points of the new range</param>
    /// <returns></returns>
    public static float Remap(this int value, float originlRangeStart, float originlRangeEnd, float newRangeStart, float newRangeEnd, bool clampResults = true)
    {
        float retVal = (value - originlRangeStart) / (originlRangeEnd - originlRangeStart) * (newRangeEnd - newRangeStart) + newRangeStart;

        if (clampResults)
        {
            if (retVal < newRangeStart)
            {
                retVal = newRangeStart;
            }
            else if (retVal > newRangeEnd)
            {
                retVal = newRangeEnd;
            }
        }

        return retVal;
    }
    #endregion


    #region Printing Helpers
    /// <summary>
    /// Returns the name of the class that calls this function
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string ClassName(this Object obj)
    {
#if UNITY_WEBGL
        return "WebGL Unsupported";
#else
        return obj.GetType().Name;
#endif
    }

    public static string DebugTime()
    {
        return "[" + Time.time + "]";
    }

    /// <summary>
    /// Returns a pretty-printed name of the class
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string DebugClassName(this Object obj)
    {
#if UNITY_WEBGL
        return DebugTime() + "[WebGL Unsupported ]";
#else
        return DebugTime() + " [" + obj.GetType().Name + "." + new StackFrame(1).GetMethod().Name + "] ";
#endif
    }

    /// <summary>
    /// Returns a pretty-printed name of the class prepended with passed in argument
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="argPrepend">The string to prepend to the class</param>
    /// <returns></returns>
    public static string DebugClassName(this Object obj, string argPrepend)
    {
#if UNITY_WEBGL
        return DebugTime() + " [" + argPrepend + ".WebGL Unsupported] ";// + obj.GetType().Name + "." + new StackFrame(1).GetMethod().Name + "] ";
#else
        return DebugTime() + " [" + argPrepend + "." + obj.GetType().Name + "." + new StackFrame(1).GetMethod().Name + "] ";

#endif
    }
    #endregion


    #region Angle Helpers
    /// <summary>
    /// Rotates a 2D vector by 'amount' degrees
    /// </summary>
    /// <param name="v">The vector to rotate</param>
    /// <param name="amount">The amount to rotate in degrees</param>
    /// <returns>The new vector</returns>
    public static Vector2 Rotate(this Vector2 v, float amount)
    {
        float sin = Mathf.Sin(amount * Mathf.Deg2Rad);
        float cos = Mathf.Cos(amount * Mathf.Deg2Rad);
        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    /// <summary>
    /// Normalizes the value to be between 'max' and 'max - 360'. For example, if max is 360, the result will be a normalized number between 0 and 360.
    /// </summary>
    /// <param name="f">The value to normalize</param>
    /// <param name="max">The maximum value you want the degrees to reach. The default of 180 means the value will be normalized from -180 to 180</param>
    /// <returns></returns>
    public static float NormalizeDeg(this float f, float max=180)
    {
        float min = max - 360f;
        f = f % 360f;
        if (f < min)
        {
            f += 360f;
        }
        else if (f > max)
        {
            f -= 360f;
        }
        return f;
    }
    #endregion

}
