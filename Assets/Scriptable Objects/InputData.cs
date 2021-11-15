using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New InputData", menuName = "Input Data", order = 1)]
public class InputData : ScriptableObject, ISerializationCallbackReceiver
{
	bool moveActive = false;
	Vector2 moveVector = Vector2.zero;
	bool lookActive = false;
	Vector2 lookVector = Vector2.zero;
	bool fireActive = false;
	bool useActive = false;

	public bool MoveActive;// { get; set; }
	public Vector2 MoveVector;// { get; set; }
	public bool LookActive;// { get; set; }
	public Vector2 LookVector;// { get; set; }
	public bool FireActive;// { get; set; }
	public bool UseActive;// { get; set; }

	public void OnAfterDeserialize() 
	{
		MoveActive = moveActive;
		MoveVector = moveVector;
		LookActive = lookActive;
		LookVector = lookVector;
		FireActive = fireActive;
		UseActive = useActive;
	}

	public void OnBeforeSerialize() { }
}