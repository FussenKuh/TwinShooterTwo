using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FKS;

// 11/20/2018 This is a work in progress whose goal was to make a custom editor drawer for audio manager stuff. Unfortunately, making such things in
// the Unity Editor blows. So, this crap never quite worked. For now, I'm keeping it and just commenting it all out on the off chance that I feel the desire
// to revisit this folly in the hopes of making it work properly. In theory, I just need to uncomment the AudClipDrawer class and the Unity inspector will
// start trying to draw based on said class.

//[CustomPropertyDrawer(typeof(AudioItemRecord.AudClip))]
//public class AudClipDrawer : PropertyDrawer
//{

//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        label = EditorGUI.BeginProperty(position, label, property);
//        Rect contentPosition = EditorGUI.PrefixLabel(position, label);
//        contentPosition.width *= 0.50f;
//        EditorGUI.indentLevel = 0;
//        EditorGUIUtility.labelWidth = 90f;
//        //EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("overrideVolume"), new GUIContent("Vol. Override"));
//        //contentPosition.x += contentPosition.width;
//        //contentPosition.width /= 3f;
//        //        contentPosition.y += 18f;
//        //EditorGUILayout.Slider(value, min, max)
//        EditorGUI.Slider(contentPosition, property.FindPropertyRelative("volume"), 0, 1);
//        //        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("volume"));
//        EditorGUI.EndProperty();
//    }

//}

//public class AudClipDrawer : MonoBehaviour {

//	// Use this for initialization
//	void Start () {

//	}

//	// Update is called once per frame
//	void Update () {

//	}
//}
