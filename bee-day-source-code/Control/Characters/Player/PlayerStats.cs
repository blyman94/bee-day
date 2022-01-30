using UnityEngine;

[CreateAssetMenu(fileName = "new PlayerStats", menuName = "PlayerStats")]
public class PlayerStats : ScriptableObject
{
	[Header("Animation Speeds")]
	[Space]
	public float attackAnimSpeed = 1;
	public float deathAnimSpeed = 1;
	public float hitAnimSpeed = 1;
	public float moveAnimSpeed = 1;
	public float readyAnimSpeed = 1;

	[Header("Collision Detection")]
	[Space]
	public LayerMask groundLayer;

	[Header("Combat")]
	[Space]
	public LayerMask canAttackLayer;
	public float attackRange;
	public float maxHealth;
	public float posionDuration;
	public float meleeCooldown;

	[Header("Movement")]
	[Space]
	public float moveSpeed = 1.0f;
	public float accelAir = 0.1f;
	
}
