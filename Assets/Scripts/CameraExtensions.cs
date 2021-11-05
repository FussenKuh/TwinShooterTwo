using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraExtensions
{

    //******Orthographic Camera Only******//

    /// <summary>
    /// Returns the bounds of the orthographic camera. DON'T USE THIS IF YOU'RE USING A PERSPECTIVE CAMERA.
    /// </summary>
    /// <param name="camera"></param>
    /// <returns></returns>
    public static Bounds OrthographicBounds(this Camera camera)
    {
        if (camera.orthographic != true)
        {
            Debug.LogWarning("Calculating OrthographicBounds for a non-orthographic camera. Things are going to end poorly.");
        }

        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(
            camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }

    public static Vector2 BoundsMin(this Camera camera)
    {
        return (Vector2)camera.transform.position - camera.Extents();
    }

    public static Vector2 BoundsMax(this Camera camera)
    {
        return (Vector2)camera.transform.position + camera.Extents();
    }

    public static Vector2 Extents(this Camera camera)
    {
        if (camera.orthographic)
            return new Vector2(camera.orthographicSize * Screen.width / Screen.height, camera.orthographicSize);
        else
        {
            Debug.LogError("Camera is not orthographic!", camera);
            return new Vector2();
        }
    }
    //*****End of Orthographic Only*****//
}
