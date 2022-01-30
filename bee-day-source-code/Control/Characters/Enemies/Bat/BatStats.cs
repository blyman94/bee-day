using UnityEngine;

[CreateAssetMenu(fileName = "new BatStats", menuName = "EnemyStats/BatStats")]
public class BatStats : EnemyStats
{
	[Header("Animation Speeds")]
	[Space]
	public float attackAnimSpeed = 1;
	public float deathAnimSpeed = 1;
	public float hitAnimSpeed = 1;
	public float moveAnimSpeed = 1;
	public float readyAnimSpeed = 1;
	
}
