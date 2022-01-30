using UnityEngine;

public class Slime : Enemy
{
	#region Field Declarations
	[Header("Slime Stats")]
	[Space]
	[SerializeField] private SlimeStats slimeStats;

	private bool isJumping;
	#endregion

	#region Startup/Shutdown
	protected override void OnEnable()
	{
		base.OnEnable();
		mover.land += OnLand;
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		mover.land -= OnLand;
	}
	#endregion

	#region Abstract Method Implementations
	protected override void MoveTowardPlayer()
	{
		Vector2 direction = (path.vectorPath[currentWaypoint] - transform.position).normalized;
		if (!isJumping)
		{
			isJumping = true;
			mover.SetDirectionalInput(direction);
			animator.SetTrigger("jump");
		}
	}
	protected override void OnPlayerContact(Collider2D other)
	{
		other.gameObject.GetComponent<Health>().TakeDamage(meleeDamage);
	}
	#endregion

	#region Initialization Methods
	protected override void InitializeAnimatorParams()
	{
		animator.SetFloat("deathAnimSpeed", slimeStats.deathAnimSpeed);
		animator.SetFloat("hitAnimSpeed", slimeStats.hitAnimSpeed);
		animator.SetFloat("readyAnimSpeed", slimeStats.readyAnimSpeed);
		animator.SetFloat("prepAnimSpeed", slimeStats.prepAnimSpeed);
		animator.SetFloat("airAnimSpeed", slimeStats.airAnimSpeed);
		animator.SetFloat("landAnimSpeed", slimeStats.landAnimSpeed);
	}
	protected override void InitializeMoverParams()
	{
		base.InitializeMoverParams();
		mover.useGravity = true;
		mover.maxJumpHeight = slimeStats.maxJumpHeight;
		mover.minJumpHeight = slimeStats.minJumpHeight;
		mover.timeToJumpApex = slimeStats.timeToJumpApex;
	}
	#endregion

	#region Behavior Methods
	private void OnLand()
	{
		animator.SetTrigger("land");
		mover.SetDirectionalInput(Vector2.zero);
		isJumping = false;
	}
	#endregion
}
