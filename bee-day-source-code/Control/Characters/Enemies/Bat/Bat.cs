using UnityEngine;

public class Bat : Enemy
{
	#region Field Declarations

	[Header("Bat Stats")]
	[Space]
	[SerializeField] private BatStats batStats;

	#endregion

	#region Initialization Methods
	protected override void InitializeAnimatorParams()
	{
		animator.SetFloat("attackAnimSpeed", batStats.attackAnimSpeed);
		animator.SetFloat("deathAnimSpeed", batStats.deathAnimSpeed);
		animator.SetFloat("hitAnimSpeed", batStats.hitAnimSpeed);
		animator.SetFloat("moveAnimSpeed", batStats.moveAnimSpeed);
		animator.SetFloat("readyAnimSpeed", batStats.readyAnimSpeed);
	}
	protected override void InitializeMoverParams()
	{
		base.InitializeMoverParams();
		mover.useGravity = false;
	}
	#endregion

	#region Abstract Method Implementations
	protected override void OnPlayerContact(Collider2D other)
	{
		other.gameObject.GetComponent<Health>().TakeDamage(meleeDamage);
	}
	protected override void MoveTowardPlayer()
	{
		Vector2 direction = (path.vectorPath[currentWaypoint] - transform.position).normalized;

		if (Vector2.Distance(transform.position, target.position) >= nextWayPointDistance)
		{
			mover.SetDirectionalInput(direction);
		}
	}
	#endregion

	#region Behavior Methods
	protected override void Aggro()
	{
		base.Aggro();
		animator.SetTrigger("wake");
	}
	#endregion
}
