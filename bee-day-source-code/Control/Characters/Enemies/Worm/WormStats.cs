using UnityEngine;

[CreateAssetMenu(fileName = "new WormStats", menuName = "EnemyStats/WormStats")]
public class WormStats : EnemyStats
{
	[Header("Animation Speeds")]
	[Space]
	public float attackAnimSpeed = 1;
	public float dieAnimSpeed = 1;
	public float hitAnimSpeed = 1;
	public float readyAnimSpeed = 1;
	public float moveAnimSpeed = 1;
	public float upAnimSpeed = 1;
	public float downAnimSpeed = 1;
	public float shootAnimSpeed = 1;

	[Header("Shooting")]
	[Space]
	public float shootRange = 15;
}
