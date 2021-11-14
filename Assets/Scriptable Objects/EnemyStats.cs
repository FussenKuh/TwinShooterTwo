using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[CreateAssetMenu(fileName = "New EnemyStats", menuName = "Enemy Stats", order = 1)]
public class EnemyStats : ScriptableObject, ISerializationCallbackReceiver
{
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
		public WeaponData Weapon { get; set; }
	}

	[SerializeField]
	string _name = "Enemy Unknown";
	[SerializeField]
	int _health = 30;
	[SerializeField]
	float _speed = 3;
	[SerializeField]
	Color _color = Color.white;
	[SerializeField]
	WeaponData _weapon;

	public StatData Stats 
	{ 
		get 
		{
			return new StatData()
			{ Health = _health, Name = _name, Color = _color, Speed = _speed, StartingColor = _color, StartingHealth = _health, Weapon = _weapon };
		} 
	}

	public void OnAfterDeserialize() { }

	public void OnBeforeSerialize() { }
}