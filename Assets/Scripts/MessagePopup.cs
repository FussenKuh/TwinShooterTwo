using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessagePopup : MonoBehaviour
{
    TextMeshPro textMesh;
    float startLife;
    float startFontSize;
    Color textColor;

    static int orderInLayer = 100;

    [Tooltip("Lifetime of the popup, in seconds")]
    [SerializeField]
    float lifetime = 0.4f;

    [Header("Movement:")]
    [Tooltip("The popup will move in this direction")]
    [SerializeField]
    Vector3 movementVector = Vector3.up;
    [Tooltip("The popup will move at this speed over the course of its lifetime")]
    [SerializeField]
    AnimationCurve speedCurve = AnimationCurve.Linear(0,1,1,1);

    [Space(10)]
    [Header("Font:")]
    [Tooltip("Default text color")]
    [SerializeField]
    Color defaultColor = Color.black;
    [Tooltip("Critical message text color")]
    [SerializeField]
    Color criticalColor = Color.red;
    [Tooltip("How much larger will a Critical message popup be?")]
    [SerializeField]
    [Range(1, 2)]
    float criticalFontMultiplier = 1.4f;
    [Tooltip("Controls the color alpha channel across the lifetime of the popup")]
    [SerializeField]
    AnimationCurve fadeCurve = AnimationCurve.Linear(0,1,1,0);
    [Tooltip("Controls the text size across the lifetime of the popup")]
    [SerializeField]
    AnimationCurve sizeCurve = AnimationCurve.Linear(0,1,1,1);


    /// <summary>
    /// Static function to instantiate a Damage Popup
    /// </summary>
    /// <param name="position">The location to spawn the popup</param>
    /// <param name="message">The message to display</param>
    /// <param name="critical">Is the message critical?</param>
    /// <returns></returns>
    public static MessagePopup Create(Vector3 position, string message, bool critical=false, float duration=0.4f)
    {
        GameObject go = Resources.Load("Prefabs/" + typeof(MessagePopup).Name) as GameObject;

        if (go == null)
        {
            Debug.LogError(typeof(MessagePopup).Name + ": Couldn't locate prefab (" + typeof(MessagePopup).Name + ")");
            return null;
        }

        MessagePopup retVal = Instantiate(go, position, Quaternion.identity).GetComponent<MessagePopup>();
 
        retVal.Setup(message, critical, duration);

        return retVal;
    }

    /// <summary>
    /// Initial configuration of the message
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="critical">Is the message critical?</param>
    void Setup(string message, bool critical, float duration)
    {
        lifetime = duration;
        startLife = lifetime;
        textMesh.SetText(message);
        textMesh.sortingOrder = orderInLayer;
        orderInLayer++;
        
        if (critical) 
        { 
            textColor = criticalColor; 
            startFontSize = textMesh.fontSize * criticalFontMultiplier; 
        } 
        else 
        { 
            textColor = defaultColor;
            startFontSize = textMesh.fontSize;
        }

        textMesh.color = textColor;
        textMesh.fontSize = startFontSize;
    }

    private void Awake()
    {
        textMesh = gameObject.GetComponent<TextMeshPro>();
    }


    // Update is called once per frame
    void Update()
    {
        // Update the popup's location
        transform.position += ((movementVector * speedCurve.Evaluate((startLife-lifetime).Remap(0,startLife,0,speedCurve[speedCurve.length-1].time))) * Time.deltaTime);

        // If we've expired, then kill the gameobject
        if (lifetime < 0) { Destroy(gameObject); return; }

        // Adjust the message's alpha channel
        textColor.a = fadeCurve.Evaluate((startLife - lifetime).Remap(0, startLife, 0, speedCurve[speedCurve.length - 1].time));
        textMesh.color = textColor;
        // Adjust the message's font size
        textMesh.fontSize = startFontSize * sizeCurve.Evaluate((startLife - lifetime).Remap(0, startLife, 0, speedCurve[speedCurve.length - 1].time));

        lifetime -= Time.deltaTime;
    }
}
