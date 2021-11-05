using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace FKS
{
    namespace Utils
    {
        public static class UtilsClass
        {

            #region Mouse Stuff 
            // Get Mouse Position in World with Z = 0f
            public static Vector3 GetMouseWorldPosition()
            {
                Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
                vec.z = 0f;
                return vec;
            }

            public static Vector3 GetMouseWorldPositionWithZ()
            {
                return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            }

            public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
            {
                return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
            }

            public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
            {
                Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
                return worldPosition;
            }

            public static Vector3 GetDirToMouse(Vector3 fromPosition)
            {
                Vector3 mouseWorldPosition = GetMouseWorldPosition();
                return (mouseWorldPosition - fromPosition).normalized;
            }

            // Is Mouse over a UI Element? Used for ignoring World clicks through UI
            public static bool IsPointerOverUI()
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return true;
                }
                else
                {
                    PointerEventData pe = new PointerEventData(EventSystem.current);
                    pe.position = Input.mousePosition;
                    List<RaycastResult> hits = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(pe, hits);
                    return hits.Count > 0;
                }
            }

            #endregion


            #region Hex and Colors
            // Returns 00-FF, value 0->255
            public static string Dec_to_Hex(int value)
            {
                return value.ToString("X2");
            }

            // Returns 0-255
            public static int Hex_to_Dec(string hex)
            {
                return Convert.ToInt32(hex, 16);
            }

            // Returns a hex string based on a number between 0->1
            public static string Dec01_to_Hex(float value)
            {
                return Dec_to_Hex((int)Mathf.Round(value * 255f));
            }

            // Returns a float between 0->1
            public static float Hex_to_Dec01(string hex)
            {
                return Hex_to_Dec(hex) / 255f;
            }

            // Get Hex Color FF00FF
            public static string GetStringFromColor(Color color)
            {
                string red = Dec01_to_Hex(color.r);
                string green = Dec01_to_Hex(color.g);
                string blue = Dec01_to_Hex(color.b);
                return red + green + blue;
            }

            // Get Hex Color FF00FFAA
            public static string GetStringFromColorWithAlpha(Color color)
            {
                string alpha = Dec01_to_Hex(color.a);
                return GetStringFromColor(color) + alpha;
            }

            // Sets out values to Hex String 'FF'
            public static void GetStringFromColor(Color color, out string red, out string green, out string blue, out string alpha)
            {
                red = Dec01_to_Hex(color.r);
                green = Dec01_to_Hex(color.g);
                blue = Dec01_to_Hex(color.b);
                alpha = Dec01_to_Hex(color.a);
            }

            // Get Hex Color FF00FF
            public static string GetStringFromColor(float r, float g, float b)
            {
                string red = Dec01_to_Hex(r);
                string green = Dec01_to_Hex(g);
                string blue = Dec01_to_Hex(b);
                return red + green + blue;
            }

            // Get Hex Color FF00FFAA
            public static string GetStringFromColor(float r, float g, float b, float a)
            {
                string alpha = Dec01_to_Hex(a);
                return GetStringFromColor(r, g, b) + alpha;
            }

            // Get Color from Hex string FF00FFAA
            public static Color GetColorFromString(string color)
            {
                float red = Hex_to_Dec01(color.Substring(0, 2));
                float green = Hex_to_Dec01(color.Substring(2, 2));
                float blue = Hex_to_Dec01(color.Substring(4, 2));
                float alpha = 1f;
                if (color.Length >= 8)
                {
                    // Color string contains alpha
                    alpha = Hex_to_Dec01(color.Substring(6, 2));
                }
                return new Color(red, green, blue, alpha);
            }

            // Return a color going from Red to Yellow to Green, like a heat map
            public static Color GetRedGreenColor(float value)
            {
                float r = 0f;
                float g = 0f;
                if (value <= .5f)
                {
                    r = 1f;
                    g = value * 2f;
                }
                else
                {
                    g = 1f;
                    r = 1f - (value - .5f) * 2f;
                }
                return new Color(r, g, 0f, 1f);
            }


            public static Color GetRandomColor()
            {
                return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);
            }

            private static int sequencialColorIndex = -1;
            private static Color[] sequencialColors = new[] {
            GetColorFromString("26a6d5"),
            GetColorFromString("41d344"),
            GetColorFromString("e6e843"),
            GetColorFromString("e89543"),
            GetColorFromString("0f6ad0"),//("d34141"),
		    GetColorFromString("b35db6"),
            GetColorFromString("c45947"),
            GetColorFromString("9447c4"),
            GetColorFromString("4756c4"),
        };

            public static void ResetSequencialColors()
            {
                sequencialColorIndex = -1;
            }

            public static Color GetSequencialColor()
            {
                sequencialColorIndex = (sequencialColorIndex + 1) % sequencialColors.Length;
                return sequencialColors[sequencialColorIndex];
            }

            public static Color GetColor255(float red, float green, float blue, float alpha = 255f)
            {
                return new Color(red / 255f, green / 255f, blue / 255f, alpha / 255f);
            }
            #endregion


            #region Directions and Angles
            // Generate random normalized direction
            public static Vector3 GetRandomDir()
            {
                return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
            }

            // Generate random normalized direction
            public static Vector3 GetRandomDirXZ()
            {
                return new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
            }


            public static Vector3 GetVectorFromAngle(int angle)
            {
                // angle = 0 -> 360
                float angleRad = angle * (Mathf.PI / 180f);
                return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            }

            public static Vector3 GetVectorFromAngle(float angle)
            {
                // angle = 0 -> 360
                float angleRad = angle * (Mathf.PI / 180f);
                return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            }

            public static Vector3 GetVectorFromAngleInt(int angle)
            {
                // angle = 0 -> 360
                float angleRad = angle * (Mathf.PI / 180f);
                return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            }

            public static float GetAngleFromVectorFloat(Vector3 dir)
            {
                dir = dir.normalized;
                float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                if (n < 0) n += 360;

                return n;
            }

            public static float GetAngleFromVectorFloatXZ(Vector3 dir)
            {
                dir = dir.normalized;
                float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
                if (n < 0) n += 360;

                return n;
            }

            public static int GetAngleFromVector(Vector3 dir)
            {
                dir = dir.normalized;
                float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                if (n < 0) n += 360;
                int angle = Mathf.RoundToInt(n);

                return angle;
            }

            public static int GetAngleFromVector180(Vector3 dir)
            {
                dir = dir.normalized;
                float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                int angle = Mathf.RoundToInt(n);

                return angle;
            }

            public static Vector3 ApplyRotationToVector(Vector3 vec, Vector3 vecRotation)
            {
                return ApplyRotationToVector(vec, GetAngleFromVectorFloat(vecRotation));
            }

            public static Vector3 ApplyRotationToVector(Vector3 vec, float angle)
            {
                return Quaternion.Euler(0, 0, angle) * vec;
            }

            public static Vector3 ApplyRotationToVectorXZ(Vector3 vec, float angle)
            {
                return Quaternion.Euler(0, angle, 0) * vec;
            }

            #endregion


            #region Parent/Child Manipulation
            /// <summary>
            /// Destroy all children of this parent
            /// </summary>
            /// <param name="parent">The parent of the children to destroy</param>
            public static void DestroyChildren(Transform parent)
            {
                foreach (Transform transform in parent)
                    GameObject.Destroy(transform.gameObject);
            }

            /// <summary>
            /// Destroy all children and randomize their names, useful if you want to do a Find() after calling destroy, since they only really get destroyed at the end of the frame
            /// </summary>
            /// <param name="parent">The parent of the children to destroy</param>
            public static void DestroyChildrenRandomizeNames(Transform parent)
            {
                foreach (Transform transform in parent)
                {
                    transform.name = "" + UnityEngine.Random.Range(10000, 99999);
                    GameObject.Destroy(transform.gameObject);
                }
            }

            /// <summary>
            /// Destroy all children except the ones with these names
            /// </summary>
            /// <param name="parent">The parent of the children to destroy</param>
            /// <param name="ignoreArr">The array of names to ignore</param>
            public static void DestroyChildren(Transform parent, params string[] ignoreArr)
            {
                foreach (Transform transform in parent)
                {
                    if (System.Array.IndexOf(ignoreArr, transform.name) == -1) // Don't ignore
                        GameObject.Destroy(transform.gameObject);
                }
            }


            /// <summary>
            /// Set all parent and all children to this layer
            /// </summary>
            /// <param name="parent">The parent object</param>
            /// <param name="layer">The layer to move the object to</param>
            public static void SetAllChildrenLayer(Transform parent, int layer)
            {
                parent.gameObject.layer = layer;
                foreach (Transform trans in parent)
                {
                    SetAllChildrenLayer(trans, layer);
                }
            }

            #endregion


            #region Get Position Information
            public static List<Vector3> GetPositionListAround(Vector3 position, float distance, int positionCount)
            {
                List<Vector3> ret = new List<Vector3>();
                for (int i = 0; i < positionCount; i++)
                {
                    int angle = i * (360 / positionCount);
                    Vector3 dir = UtilsClass.ApplyRotationToVector(new Vector3(0, 1), angle);
                    Vector3 pos = position + dir * distance;
                    ret.Add(pos);
                }
                return ret;
            }

            public static List<Vector3> GetPositionListAround(Vector3 position, float[] ringDistance, int[] ringPositionCount)
            {
                List<Vector3> ret = new List<Vector3>();
                for (int ring = 0; ring < ringPositionCount.Length; ring++)
                {
                    List<Vector3> ringPositionList = GetPositionListAround(position, ringDistance[ring], ringPositionCount[ring]);
                    ret.AddRange(ringPositionList);
                }
                return ret;
            }

            public static List<Vector3> GetPositionListAround(Vector3 position, float distance, int positionCount, Vector3 direction, int angleStart, int angleIncrease)
            {
                List<Vector3> ret = new List<Vector3>();
                for (int i = 0; i < positionCount; i++)
                {
                    int angle = angleStart + angleIncrease * i;
                    Vector3 dir = UtilsClass.ApplyRotationToVector(direction, angle);
                    Vector3 pos = position + dir * distance;
                    ret.Add(pos);
                }
                return ret;
            }

            public static List<Vector3> GetPositionListAlongDirection(Vector3 position, Vector3 direction, float distancePerPosition, int positionCount)
            {
                List<Vector3> ret = new List<Vector3>();
                for (int i = 0; i < positionCount; i++)
                {
                    Vector3 pos = position + direction * (distancePerPosition * i);
                    ret.Add(pos);
                }
                return ret;
            }

            public static List<Vector3> GetPositionListAlongAxis(Vector3 positionStart, Vector3 positionEnd, int positionCount)
            {
                Vector3 direction = (positionEnd - positionStart).normalized;
                float distancePerPosition = (positionEnd - positionStart).magnitude / positionCount;
                return GetPositionListAlongDirection(positionStart + direction * (distancePerPosition / 2f), direction, distancePerPosition, positionCount);
            }

            public static List<Vector3> GetPositionListWithinRect(Vector3 lowerLeft, Vector3 upperRight, int positionCount)
            {
                List<Vector3> ret = new List<Vector3>();
                float width = upperRight.x - lowerLeft.x;
                float height = upperRight.y - lowerLeft.y;
                float area = width * height;
                float areaPerPosition = area / positionCount;
                float positionSquareSize = Mathf.Sqrt(areaPerPosition);
                Vector3 rowLeft, rowRight;
                rowLeft = new Vector3(lowerLeft.x, lowerLeft.y);
                rowRight = new Vector3(upperRight.x, lowerLeft.y);
                int rowsTotal = Mathf.RoundToInt(height / positionSquareSize);
                float increaseY = height / rowsTotal;
                rowLeft.y += increaseY / 2f;
                rowRight.y += increaseY / 2f;
                int positionsPerRow = Mathf.RoundToInt(width / positionSquareSize);
                for (int i = 0; i < rowsTotal; i++)
                {
                    ret.AddRange(GetPositionListAlongAxis(rowLeft, rowRight, positionsPerRow));
                    rowLeft.y += increaseY;
                    rowRight.y += increaseY;
                }
                int missingPositions = positionCount - ret.Count;
                Vector3 angleDir = (upperRight - lowerLeft) / missingPositions;
                for (int i = 0; i < missingPositions; i++)
                {
                    ret.Add(lowerLeft + (angleDir / 2f) + angleDir * i);
                }
                while (ret.Count > positionCount)
                {
                    ret.RemoveAt(UnityEngine.Random.Range(0, ret.Count));
                }
                return ret;
            }



            public static List<Vector2Int> GetPosXYListDiamond(int size)
            {
                List<Vector2Int> list = new List<Vector2Int>();
                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size - x; y++)
                    {
                        list.Add(new Vector2Int(x, y));
                        list.Add(new Vector2Int(-x, y));
                        list.Add(new Vector2Int(x, -y));
                        list.Add(new Vector2Int(-x, -y));
                    }
                }
                return list;
            }

            public static List<Vector2Int> GetPosXYListOblong(int width, int dropXamount, int increaseDropXamount, Vector3 dir)
            {
                List<Vector2Int> list = GetPosXYListOblong(width, dropXamount, increaseDropXamount);
                list = RotatePosXYList(list, UtilsClass.GetAngleFromVector(dir));
                return list;
            }

            public static List<Vector2Int> GetPosXYListOblong(int width, int dropXamount, int increaseDropXamount)
            {
                List<Vector2Int> triangle = GetPosXYListTriangle(width, dropXamount, increaseDropXamount);
                List<Vector2Int> list = new List<Vector2Int>(triangle);
                foreach (Vector2Int posXY in triangle)
                {
                    if (posXY.y == 0) continue;
                    list.Add(new Vector2Int(posXY.x, -posXY.y));
                }
                foreach (Vector2Int posXY in new List<Vector2Int>(list))
                {
                    if (posXY.x == 0) continue;
                    list.Add(new Vector2Int(-posXY.x, posXY.y));
                }
                return list;
            }

            public static List<Vector2Int> GetPosXYListTriangle(int width, int dropXamount, int increaseDropXamount)
            {
                List<Vector2Int> list = new List<Vector2Int>();
                for (int i = 0; i > -999; i--)
                {
                    for (int j = 0; j < width; j++)
                    {
                        list.Add(new Vector2Int(j, i));
                    }
                    width -= dropXamount;
                    dropXamount += increaseDropXamount;
                    if (width <= 0) break;
                }
                return list;
            }

            public static List<Vector2Int> RotatePosXYList(List<Vector2Int> list, int angle)
            {
                List<Vector2Int> ret = new List<Vector2Int>();
                for (int i = 0; i < list.Count; i++)
                {
                    Vector2Int posXY = list[i];
                    Vector3 vec = UtilsClass.ApplyRotationToVector(new Vector3(posXY.x, posXY.y), angle);
                    ret.Add(new Vector2Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y)));
                }
                return ret;
            }
            #endregion

            #region Clone Transform
            public static Transform CloneTransform(Transform transform, string name = null)
            {
                Transform clone = GameObject.Instantiate(transform, transform.parent);

                if (name != null)
                    clone.name = name;
                else
                    clone.name = transform.name;

                return clone;
            }

            public static Transform CloneTransform(Transform transform, string name, Vector2 newAnchoredPosition, bool setActiveTrue = false)
            {
                Transform clone = CloneTransform(transform, name);
                clone.GetComponent<RectTransform>().anchoredPosition = newAnchoredPosition;
                if (setActiveTrue)
                {
                    clone.gameObject.SetActive(true);
                }
                return clone;
            }

            public static Transform CloneTransform(Transform transform, Transform newParent, string name = null)
            {
                Transform clone = GameObject.Instantiate(transform, newParent);

                if (name != null)
                    clone.name = name;
                else
                    clone.name = transform.name;

                return clone;
            }

            public static Transform CloneTransform(Transform transform, Transform newParent, string name, Vector2 newAnchoredPosition, bool setActiveTrue = false)
            {
                Transform clone = CloneTransform(transform, newParent, name);
                clone.GetComponent<RectTransform>().anchoredPosition = newAnchoredPosition;
                if (setActiveTrue)
                {
                    clone.gameObject.SetActive(true);
                }
                return clone;
            }

            public static Transform CloneTransformWorld(Transform transform, Transform newParent, string name, Vector3 newLocalPosition)
            {
                Transform clone = CloneTransform(transform, newParent, name);
                clone.localPosition = newLocalPosition;
                return clone;
            }
            #endregion

            #region Debug Drawing
            public static void DebugDrawCircle(Vector3 center, float radius, Color color, float duration, int divisions)
            {
                for (int i = 0; i <= divisions; i++)
                {
                    Vector3 vec1 = center + UtilsClass.ApplyRotationToVector(new Vector3(0, 1) * radius, (360f / divisions) * i);
                    Vector3 vec2 = center + UtilsClass.ApplyRotationToVector(new Vector3(0, 1) * radius, (360f / divisions) * (i + 1));
                    Debug.DrawLine(vec1, vec2, color, duration);
                }
            }

            public static void DebugDrawRectangle(Vector3 minXY, Vector3 maxXY, Color color, float duration)
            {
                Debug.DrawLine(new Vector3(minXY.x, minXY.y), new Vector3(maxXY.x, minXY.y), color, duration);
                Debug.DrawLine(new Vector3(minXY.x, minXY.y), new Vector3(minXY.x, maxXY.y), color, duration);
                Debug.DrawLine(new Vector3(minXY.x, maxXY.y), new Vector3(maxXY.x, maxXY.y), color, duration);
                Debug.DrawLine(new Vector3(maxXY.x, minXY.y), new Vector3(maxXY.x, maxXY.y), color, duration);
            }

            public static void DebugDrawText(string text, Vector3 position, Color color, float size, float duration)
            {
                text = text.ToUpper();
                float kerningSize = size * 0.6f;
                Vector3 basePosition = position;
                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];
                    switch (c)
                    {
                        case '\n':
                            // Newline
                            position.x = basePosition.x;
                            position.y += size;
                            break;
                        case ' ':
                            position.x += kerningSize;
                            break;
                        default:
                            DebugDrawChar(c, position, color, size, duration);
                            position.x += kerningSize;
                            break;
                    }
                }
            }

            // Draw Characters using Debug DrawLine Gizmos
            public static void DebugDrawChar(char c, Vector3 position, Color color, float size, float duration)
            {
                switch (c)
                {
                    default:
                    case 'A':
                        DebugDrawLines(position, color, size, duration, new[] {
                0.317f,0.041f, 0.5f,0.98f, 0.749f,0.062f, 0.625f,0.501f, 0.408f,0.507f }); break;
                    case 'B':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.289f,0.069f, 0.274f,0.937f, 0.609f,0.937f, 0.801f,0.879f, 0.829f,0.708f, 0.756f,0.538f, 0.655f,0.492f, 0.442f,0.495f, 0.271f,0.495f, 0.567f,0.474f, 0.676f,0.465f, 0.722f,0.385f, 0.719f,0.181f, 0.664f,0.087f, 0.527f,0.053f, 0.396f,0.05f, 0.271f,0.078f }); break;
                    case 'C':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.695f,0.946f, 0.561f,0.949f, 0.426f,0.937f, 0.317f,0.867f, 0.265f,0.733f, 0.262f,0.553f, 0.292f,0.27f, 0.323f,0.172f, 0.417f,0.12f, 0.512f,0.096f, 0.637f,0.093f, 0.743f,0.117f, }); break;
                    case 'D':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.314f,0.909f, 0.329f,0.096f, 0.53f,0.123f, 0.594f,0.197f, 0.673f,0.334f, 0.716f,0.498f, 0.692f,0.666f, 0.609f,0.806f, 0.457f,0.891f, 0.323f,0.919f }); break;
                    case 'E':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.344f,0.919f, 0.363f,0.078f, 0.713f,0.096f, 0.359f,0.096f, 0.347f,0.48f, 0.53f,0.492f, 0.356f,0.489f, 0.338f,0.913f, 0.625f,0.919f }); break;
                    case 'F':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.682f,0.916f, 0.329f,0.909f, 0.341f,0.66f, 0.503f,0.669f, 0.341f,0.669f, 0.317f,0.087f }); break;
                    case 'G':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.618f,0.867f, 0.399f,0.849f, 0.292f,0.654f, 0.241f,0.404f, 0.253f,0.178f, 0.481f,0.075f, 0.612f,0.078f, 0.725f,0.169f, 0.728f,0.334f, 0.71f,0.437f, 0.609f,0.462f, 0.463f,0.462f }); break;
                    case 'H':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.277f,0.876f, 0.305f,0.133f, 0.295f,0.507f, 0.628f,0.501f, 0.643f,0.139f, 0.637f,0.873f }); break;
                    case 'I':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.487f,0.906f, 0.484f,0.096f }); break;
                    case 'J':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.628f,0.882f, 0.679f,0.242f, 0.603f,0.114f, 0.445f,0.066f, 0.317f,0.114f, 0.262f,0.209f, 0.253f,0.3f, 0.259f,0.367f }); break;
                    case 'K':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.292f,0.879f, 0.311f,0.111f, 0.305f,0.498f, 0.594f,0.876f, 0.305f,0.516f, 0.573f,0.154f,  }); break;
                    case 'L':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.311f,0.879f, 0.308f,0.133f, 0.682f,0.148f,  }); break;
                    case 'M':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.262f,0.12f, 0.265f,0.909f, 0.509f,0.608f, 0.71f,0.919f, 0.713f,0.151f,  }); break;
                    case 'N':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.737f,0.885f, 0.679f,0.114f, 0.335f,0.845f, 0.353f,0.175f,  }); break;
                    case 'O':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.609f,0.906f, 0.396f,0.894f, 0.271f,0.687f, 0.232f,0.474f, 0.241f,0.282f, 0.356f,0.142f, 0.527f,0.087f, 0.655f,0.09f, 0.719f,0.181f, 0.737f,0.379f, 0.737f,0.638f, 0.71f,0.836f, 0.628f,0.919f, 0.582f,0.919f, }); break;
                    case 'P':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.314f,0.129f, 0.311f,0.873f, 0.658f,0.906f, 0.746f,0.8f, 0.746f,0.66f, 0.673f,0.544f, 0.509f,0.51f, 0.359f,0.51f, 0.311f,0.516f,  }); break;
                    case 'Q':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.497f,0.925f, 0.335f,0.894f, 0.228f,0.681f, 0.213f,0.379f, 0.25f,0.145f, 0.396f,0.096f, 0.573f,0.105f, 0.631f,0.166f, 0.542f,0.245f, 0.752f,0.108f, 0.628f,0.187f, 0.685f,0.261f, 0.728f,0.398f, 0.759f,0.605f, 0.722f,0.794f, 0.64f,0.916f, 0.475f,0.946f,  }); break;
                    case 'R':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.347f,0.142f, 0.332f,0.9f, 0.667f,0.897f, 0.698f,0.699f, 0.655f,0.58f, 0.521f,0.553f, 0.396f,0.553f, 0.344f,0.553f, 0.564f,0.37f, 0.655f,0.206f, 0.71f,0.169f }); break;
                    case 'S':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.695f,0.842f, 0.576f,0.882f, 0.439f,0.885f, 0.329f,0.8f, 0.289f,0.626f, 0.317f,0.489f, 0.439f,0.44f, 0.621f,0.434f, 0.695f,0.358f, 0.713f,0.224f, 0.646f,0.111f, 0.494f,0.093f, 0.338f,0.105f, 0.289f,0.151f, }); break;
                    case 'T':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.497f,0.172f, 0.5f,0.864f, 0.286f,0.858f, 0.719f,0.852f,  }); break;
                    case 'U':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.232f,0.858f, 0.247f,0.251f, 0.366f,0.105f, 0.466f,0.078f, 0.615f,0.084f, 0.704f,0.123f, 0.746f,0.276f, 0.74f,0.559f, 0.737f,0.806f, 0.722f,0.864f, }); break;
                    case 'V':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.238f,0.855f, 0.494f,0.105f, 0.707f,0.855f,  }); break;
                    case 'X':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.783f,0.852f, 0.256f,0.133f, 0.503f,0.498f, 0.305f,0.824f, 0.789f,0.117f,  }); break;
                    case 'Y':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.299f,0.842f, 0.497f,0.529f, 0.646f,0.842f, 0.49f,0.541f, 0.487f,0.105f, }); break;
                    case 'W':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.228f,0.815f, 0.381f,0.093f, 0.503f,0.434f, 0.615f,0.151f, 0.722f,0.818f,  }); break;
                    case 'Z':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.25f,0.87f, 0.795f,0.842f, 0.274f,0.133f, 0.716f,0.142f }); break;


                    case '0':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.536f,0.891f, 0.509f,0.891f, 0.42f,0.809f, 0.378f,0.523f, 0.372f,0.215f, 0.448f,0.087f, 0.539f,0.069f, 0.609f,0.099f, 0.637f,0.242f, 0.646f,0.416f, 0.646f,0.608f, 0.631f,0.809f, 0.554f,0.888f, 0.527f,0.894f,  }); break;
                    case '1':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.652f,0.108f, 0.341f,0.114f, 0.497f,0.12f, 0.497f,0.855f, 0.378f,0.623f,  }); break;
                    case '2':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.311f,0.714f, 0.375f,0.83f, 0.564f,0.894f, 0.722f,0.839f, 0.765f,0.681f, 0.634f,0.483f, 0.5f,0.331f, 0.366f,0.245f, 0.299f,0.126f, 0.426f,0.126f, 0.621f,0.136f, 0.679f,0.136f, 0.737f,0.139f,  }); break;
                    case '3':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.289f,0.855f, 0.454f,0.876f, 0.606f,0.818f, 0.685f,0.702f, 0.664f,0.547f, 0.564f,0.459f, 0.484f,0.449f, 0.417f,0.455f, 0.53f,0.434f, 0.655f,0.355f, 0.664f,0.233f, 0.591f,0.105f, 0.466f,0.075f, 0.335f,0.084f, 0.259f,0.142f,  }); break;
                    case '4':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.353f,0.836f, 0.262f,0.349f, 0.579f,0.367f, 0.5f,0.376f, 0.49f,0.471f, 0.509f,0.069f,  }); break;
                    case '5':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.67f,0.852f, 0.335f,0.858f, 0.347f,0.596f, 0.582f,0.602f, 0.698f,0.513f, 0.749f,0.343f, 0.719f,0.187f, 0.561f,0.133f, 0.363f,0.151f,  }); break;
                    case '6':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.567f,0.888f, 0.442f,0.782f, 0.35f,0.544f, 0.326f,0.288f, 0.39f,0.157f, 0.615f,0.142f, 0.679f,0.245f, 0.676f,0.37f, 0.573f,0.48f, 0.454f,0.48f, 0.378f,0.41f, 0.335f,0.367f,  }); break;
                    case '7':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.286f,0.852f, 0.731f,0.864f, 0.417f,0.117f, 0.57f,0.498f, 0.451f,0.483f, 0.688f,0.501f, }); break;
                    case '8':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.518f,0.541f, 0.603f,0.623f, 0.649f,0.748f, 0.612f,0.858f, 0.497f,0.888f, 0.375f,0.824f, 0.341f,0.708f, 0.381f,0.611f, 0.494f,0.55f, 0.557f,0.513f, 0.6f,0.416f, 0.631f,0.312f, 0.579f,0.178f, 0.509f,0.108f, 0.436f,0.102f, 0.335f,0.181f, 0.308f,0.279f, 0.347f,0.401f, 0.423f,0.486f, 0.497f,0.547f,  }); break;
                    case '9':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.475f,0.129f, 0.573f,0.495f, 0.646f,0.824f, 0.509f,0.97f, 0.28f,0.94f, 0.189f,0.827f, 0.262f,0.708f, 0.396f,0.69f, 0.564f,0.745f, 0.646f,0.83f,  }); break;


                    case '.':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.515f,0.157f, 0.469f,0.148f, 0.469f,0.117f, 0.515f,0.123f, 0.503f,0.169f }); break;
                    case ':':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.515f,0.157f, 0.469f,0.148f, 0.469f,0.117f, 0.515f,0.123f, 0.503f,0.169f });
                        DebugDrawLines(position, color, size, duration, new[] {
                0.515f,.5f+0.157f, 0.469f,.5f+0.148f, 0.469f,.5f+0.117f, 0.515f,.5f+0.123f, 0.503f,.5f+0.169f });
                        break;
                    case '-':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.277f,0.51f, 0.716f,0.51f,  }); break;
                    case '+':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.265f,0.513f, 0.676f,0.516f, 0.497f,0.529f, 0.49f,0.699f, 0.497f,0.27f,  }); break;

                    case '(':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.542f,0.934f, 0.411f,0.797f, 0.344f,0.587f, 0.341f,0.434f, 0.375f,0.257f, 0.457f,0.12f, 0.567f,0.075f, }); break;
                    case ')':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.442f,0.94f, 0.548f,0.757f, 0.625f,0.568f, 0.64f,0.392f, 0.554f,0.129f, 0.472f,0.056f,  }); break;
                    case ';':
                    case ',':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.533f,0.239f, 0.527f,0.154f, 0.487f,0.099f, 0.451f,0.062f,  }); break;
                    case '_':
                        DebugDrawLines(position, color, size, duration, new[] {
               0.274f,0.133f, 0.716f,0.142f }); break;

                        /*
                 case ':': DebugDrawLines(position, color, size, duration, new [] {
                        0f }); break;
                        */
                }
            }

            public static void DebugDrawLines(Vector3 position, Color color, float size, float duration, Vector3[] points)
            {
                for (int i = 0; i < points.Length - 1; i++)
                {
                    Debug.DrawLine(position + points[i] * size, position + points[i + 1] * size, color, duration);
                }
            }

            public static void DebugDrawLines(Vector3 position, Color color, float size, float duration, float[] points)
            {
                List<Vector3> vecList = new List<Vector3>();
                for (int i = 0; i < points.Length; i += 2)
                {
                    Vector3 vec = new Vector3(points[i + 0], points[i + 1]);
                    vecList.Add(vec);
                }
                DebugDrawLines(position, color, size, duration, vecList.ToArray());
            }
            #endregion


            public static class ReflectionTools
            {

                public static object CallMethod(string typeName, string methodName)
                {
                    return System.Type.GetType(typeName).GetMethod(methodName).Invoke(null, null);
                }
                public static object GetField(string typeName, string fieldName)
                {
                    System.Reflection.FieldInfo fieldInfo = System.Type.GetType(typeName).GetField(fieldName);
                    return fieldInfo.GetValue(null);
                }
                public static System.Type GetNestedType(string typeName, string nestedTypeName)
                {
                    return System.Type.GetType(typeName).GetNestedType(nestedTypeName);
                }

            }



            public static bool TestChance(int chance, int chanceMax = 100)
            {
                return UnityEngine.Random.Range(0, chanceMax) < chance;
            }

            public static T[] RemoveDuplicates<T>(T[] arr)
            {
                List<T> list = new List<T>();
                foreach (T t in arr)
                {
                    if (!list.Contains(t))
                    {
                        list.Add(t);
                    }
                }
                return list.ToArray();
            }

            public static List<T> RemoveDuplicates<T>(List<T> arr)
            {
                List<T> list = new List<T>();
                foreach (T t in arr)
                {
                    if (!list.Contains(t))
                    {
                        list.Add(t);
                    }
                }
                return list;
            }


            // Return random element from array
            public static T GetRandom<T>(T[] array)
            {
                return array[UnityEngine.Random.Range(0, array.Length)];
            }

            public static Vector3 GetRandomPositionWithinRectangle(float xMin, float xMax, float yMin, float yMax)
            {
                return new Vector3(UnityEngine.Random.Range(xMin, xMax), UnityEngine.Random.Range(yMin, yMax));
            }

            public static Vector3 GetRandomPositionWithinRectangle(Vector3 lowerLeft, Vector3 upperRight)
            {
                return new Vector3(UnityEngine.Random.Range(lowerLeft.x, upperRight.x), UnityEngine.Random.Range(lowerLeft.y, upperRight.y));
            }


        }
    }
}

