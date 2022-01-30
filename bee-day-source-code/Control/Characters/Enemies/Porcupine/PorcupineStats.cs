using UnityEngine;

[CreateAssetMenu(fileName = "new PorcupineStats", menuName = "EnemyStats/PorcupineStats")]
public class PorcupineStats : EnemyStats
{
	[Header("Animation Speeds")]
	[Space]
	public float attackAnimSpeed = 1;
	public float deathAnimSpeed = 1;
	public float hitAnimSpeed = 1;
	public float readyAnimSpeed = 1;
	public float walkAnimSpeed = 1;

	[Header("Shooting")]
	[Space]
	public GameObject projectilePrefab;
	public float shootRange = 6;
	public float shotCooldown;
	public int projectilePoolSize = 20;
}
