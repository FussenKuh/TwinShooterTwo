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
	float _health = 100;
	[SerializeField]
	float _speed = 15;
	[SerializeField]
	Color _color = Color.blue;
	[SerializeField]
	float _energy = 100f;
	[SerializeField]
	float _energyLossRate = 1f;
	[SerializeField]
	float _energyGainRate = 2f;
	[SerializeField]
	int _floorsCount = 1;
	
	public StatData Data 
	{ 
		get 
		{ 
			return new StatData() 
			{
				Health = _health, Name = _name, Color = _color, Speed = _speed, 
				StartingColor = _color, StartingHealth = _health, Energy = _energy, FloorsCount = _floorsCount,
				EnergyGainRate = _energyGainRate, EnergyLossRate = _energyLossRate, StartingEnergy = _energy
			}; 
		} 
	}


	[Serializable]
	public class StatData
	{
		public float Health { get; set; }
		public float Speed { get; set; }
		public string Name { get; set; }
		public Color Color { get; set; }
		public float Energy { get; set; }
		public float EnergyLossRate { get; set; }
		public float EnergyGainRate { get; set; }
		public int FloorsCount { get; set; }
		public bool OnFloor { get { return FloorsCount > 0; } }
		public bool Alive { get { return Health > 0; } }
		public Color StartingColor { get; set; }
		public float StartingHealth { get; set; }
		public float StartingEnergy { get; set; }
	}

	public void OnAfterDeserialize() { }

	public void OnBeforeSerialize() { }
}