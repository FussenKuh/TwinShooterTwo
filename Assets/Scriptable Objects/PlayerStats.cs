using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerStats", menuName = "Player Stats", order = 1)]
public class PlayerStats : ScriptableObject, ISerializationCallbackReceiver
{
	[SerializeField]
	string _name = "The Unknown One";
	[SerializeField]
	int _health = 100;
	[SerializeField]
	float _speed = 15;
	[SerializeField]
	Color _color = Color.blue;
	
	public StatData Data 
	{ 
		get 
		{ 
			return new StatData() 
			{Health = _health, Name = _name, Color = _color, Speed = _speed, StartingColor = _color, StartingHealth = _health  }; 
		} 
	}


	[Serializable]
	public class StatData
	{
		public int Health { get; set; }
		public float Speed { get; set; }
		public string Name { get; set; }
		public Color Color { get; set; }
		public bool Alive { get { return Health > 0; } }
		public Color StartingColor { get; set; }
		public int StartingHealth { get; set; }
	}

	public void OnAfterDeserialize() { }

	public void OnBeforeSerialize() { }
}