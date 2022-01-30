using UnityEngine;


public class EnemyStats : ScriptableObject
{
	[Header("Collision Detection")]
	[Space]
	public LayerMask groundLayer;

	[Header("Combat")]
	[Space]
	public LayerMask canAttackLayer;
	public float maxHealth;
	public float meleeDamage = 10.0f;
	public float meleeCooldown = 2.0f;

	[Header("Movement")]
	[Space]
	public float moveSpeed = 1;
	public float xAccelGround = 0.2f;
	public float accelAir = 0.1f;
	public float xAccelAir = 0.1f;
}
