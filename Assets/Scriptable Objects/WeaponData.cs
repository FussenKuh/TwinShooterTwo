using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponData", menuName = "Weapon Data", order = 1)]
public class WeaponData : ScriptableObject, ISerializationCallbackReceiver
{
	[Tooltip("The name of the weapon")]
	[SerializeField]
	string weaponName = "Unnamed";

	[Tooltip("The prefab to spawn when the weapon is fired")]
	[SerializeField]
	GameObject projectilePrefab;

	[Tooltip("The speed of the projectile")]
	[SerializeField]
	float speed = 15f;

	[Tooltip("The weapon spread. In degrees. In eiher direction from straight")]
	[SerializeField]
	float spread = 3f;
	
	[Tooltip("Distance the projectile can fly")]
	[SerializeField]
	float range = 10f;

	[Tooltip("Delay between successive shots")]
	[SerializeField]
	float fireRate = 0.5f;

	[Tooltip("The minimum amount of damage the weapon can inflict")]
	[SerializeField]
	int damageMin = 1;

	[Tooltip("The maximum amount of damage the weapon can inflict")]
	[SerializeField]
	int damageMax = 1;

	public string Name { get { return weaponName; } }
	public GameObject ProjectilePrefab { get { return projectilePrefab; } }
	public float Speed { get { return speed; } }
	public float Spread { get { return spread; } }
	public float Range { get { return range; } }
	public float FireRate { get { return fireRate; } }
	public int DamageMin { get { return damageMin; } }
	public int DamageMax { get { return damageMax; } }
	public int Damage { get { return UnityEngine.Random.Range(damageMin, damageMax); } }


	public void OnAfterDeserialize() { }

	public void OnBeforeSerialize() { }
}