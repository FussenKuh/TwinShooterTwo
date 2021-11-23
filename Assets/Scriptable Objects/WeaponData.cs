using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponData", menuName = "Weapon Data", order = 1)]
public class WeaponData : ScriptableObject, ISerializationCallbackReceiver
{
	public struct DamageType
    {
		public float damage;
		public bool critical;
    }

	public enum WeaponType { Enemy, Player}

	[Tooltip("The type of weapon")]
	[SerializeField]
	WeaponType weaponType = WeaponType.Enemy;

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

	[Tooltip("Do damage to anyone within this radius")]
	[SerializeField]
	float damageRadius = 0;

	[Tooltip("Amount  of energy one shot of this weapon takes")]
	[SerializeField]
	float energyUsedPerShot = 1;

	[SerializeField]
	[Range(0, 100)]
	float criticalChance = 5;
	[SerializeField]
	float criticalMultiplier = 0.2f;

	public WeaponType Type { get { return weaponType; } }
	public string Name { get { return weaponName; } }
	public GameObject ProjectilePrefab { get { return projectilePrefab; } }
	public float Speed { get { return speed; } }
	public float Spread { get { return spread; } }
	public float Range { get { return range; } }
	public float FireRate { get { return fireRate; } }
	public int DamageMin { get { return damageMin; } }
	public int DamageMax { get { return damageMax; } }
	public float DamageRadius { get { return damageRadius; } }
	public DamageType Damage 
	{ 
		get 
		{
			bool crit = FKS.Utils.UtilsClass.TestChance((int)CriticalChance);
			float damage = UnityEngine.Random.Range(damageMin, damageMax + 1);
			if (crit)
            {
				damage *= (1 + CriticalMultiplier);
            }
			return new DamageType() { damage = damage, critical = crit }; 
		} 
	}
	public float EnergyUsedPerShot { get { return energyUsedPerShot; } }
	public float CriticalChance { get { return criticalChance; } }
	public float CriticalMultiplier { get { return criticalMultiplier; } }


	public void OnAfterDeserialize() { }

	public void OnBeforeSerialize() { }
}