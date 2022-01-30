using UnityEngine;

[CreateAssetMenu(fileName = "new SlimeStats", menuName = "EnemyStats/SlimeStats")]
public class SlimeStats : EnemyStats
{
	[Header("Animation Speeds")]
	[Space]
	public float deathAnimSpeed = 1;
	public float hitAnimSpeed = 1;
	public float readyAnimSpeed = 1;
	public float prepAnimSpeed = 1;
	public float airAnimSpeed = 1;
	public float landAnimSpeed = 1;

	[Header("Jumping")]
	public float maxJumpHeight = 4.0f;
	public float minJumpHeight = 1.0f;
	public float timeToJumpApex = 0.4f;
}
