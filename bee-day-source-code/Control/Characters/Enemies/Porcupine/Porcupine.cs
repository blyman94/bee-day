using UnityEngine;
using System.Collections;

public class Porcupine : Enemy
{
	#region Constants
	// Note: Attack Range is hardcoded to align with sprite size;
	public const float attackRange = 2.0f;
	#endregion

	#region Field Declarations
	[Header("Porcupine Stats")]
	[Space]
	[SerializeField] private PorcupineStats porcupineStats;

	[Header("Porcupine Components")]
	[Space]
	[SerializeField] private MultiShooter multiShooter;
	[SerializeField] private PorcupineAnimatorHelper animatorHelper;

	[Header("Shooting")]
	[Space]
	[SerializeField] private Transform[] shotOrigins;
	[SerializeField] private Transform projectileParent;

	private bool canAttack = true;
	#endregion

	#region Startup/Shutdown
	protected override void Awake()
	{
		base.Awake();
		InitializeMultiShooterParams();
	}
	protected override void OnEnable()
	{
		base.OnEnable();
		animatorHelper.attackHit += OnAttackHit;
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		animatorHelper.attackHit -= OnAttackHit;
	}
	#endregion

	#region Updates
	protected override void Update()
	{
		base.Update();
		UpdateAnimatorParams();
	}
    #endregion

    #region Initialization Methods
    protected override void InitializeAnimatorParams()
	{
		animator.SetFloat("attackAnimSpeed", porcupineStats.attackAnimSpeed);
		animator.SetFloat("deathAnimSpeed", porcupineStats.deathAnimSpeed);
		animator.SetFloat("hitAnimSpeed", porcupineStats.hitAnimSpeed);
		animator.SetFloat("readyAnimSpeed", porcupineStats.readyAnimSpeed);
		animator.SetFloat("walkAnimSpeed", porcupineStats.walkAnimSpeed);
	}
	protected override void InitializeMoverParams()
	{
		base.InitializeMoverParams();
		mover.useGravity = true;
	}
	private void InitializeMultiShooterParams()
	{
		multiShooter.canHitTag = "Player";
		multiShooter.projectilePrefab = porcupineStats.projectilePrefab;
		multiShooter.projectilePoolSize = porcupineStats.projectilePoolSize;
		multiShooter.projectileParent = projectileParent;
		multiShooter.shotOrigins = shotOrigins;
		multiShooter.shotCooldown = porcupineStats.shotCooldown;
	}
    #endregion

    #region Animator Controller Methods
    private void UpdateAnimatorParams()
	{
		if (isAggro && !isDead)
		{
			animator.SetFloat("horizontalInput", mover.ControllerInput.x);
		}
	}
    #endregion

	#region Combat Methods
	private void StopAndShoot()
	{
		if (multiShooter.canShoot)
		{
			animator.SetTrigger("shoot");
			mover.SetDirectionalInput(Vector2.zero);
			multiShooter.FireProjectiles();
		}
	}
	private void StopAndAttack()
	{
		if (canAttack)
		{
			mover.SetDirectionalInput(Vector2.zero);
			animator.SetTrigger("attack");
			canAttack = false;
		}
	}
	private void OnAttackHit()
	{
		Vector2 playerDirection = (target.position - transform.position).normalized;
		RaycastHit2D hit = Physics2D.Raycast(transform.position, playerDirection, attackRange,porcupineStats.canAttackLayer);
		if (hit)
		{
			if (hit.transform.CompareTag("Player"))
			{
				hit.transform.GetComponent<Health>().TakeDamage(meleeDamage);
			}
		}
		StartCoroutine(AttackCooldownRoutine());
	}
	private IEnumerator AttackCooldownRoutine()
	{
		yield return new WaitForSeconds(meleeCooldown);
		canAttack = true;
	}
	#endregion

	#region Abstract Method Implementations
	protected override void MoveTowardPlayer()
	{
		Vector2 direction = (path.vectorPath[currentWaypoint] - transform.position).normalized;

		if (Vector2.Distance(transform.position, target.position) >= nextWayPointDistance)
		{
			mover.SetDirectionalInput(direction);
		}

		float distanceToPlayer = Vector2.Distance(transform.position, target.transform.position);
		float distanceToPlayerY = Mathf.Abs(transform.position.y - target.position.y);

		if (Mathf.Abs(distanceToPlayer) < porcupineStats.shootRange)
		{
			if (distanceToPlayerY < attackRange)
			{
				StopAndAttack();
			}
			else
			{
				StopAndShoot();
			}
		}
	}
	protected override void OnPlayerContact(Collider2D other)
	{
		// The porcupine can only damage the player when shooting or attacking.
		return;
	}
	#endregion
}
