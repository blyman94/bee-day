using UnityEngine;
using System.Collections;

public class Worm : Enemy
{
    #region Constants
    // Note: Attack Range is hardcoded to align with sprite size;
    public const float meleeRange = 5.0f;
	public const float specialRange = 3.0f;
	// Note: Underground time is fixed so the player can anitcipate attacks.
	public const float undergroundTime = 3.0f;
	#endregion

	#region Field Declarations
	[Header("Worm Stats")]
	[Space]
	[SerializeField] private WormStats wormStats;

	[Header("Components")]
	[Space]
	[SerializeField] private Shooter shooter;
	[SerializeField] private WormAnimatorHelper animatorHelper;

	[Header("Shooting")]
	[Space]
	[SerializeField] private Transform shotOrigin;
	[SerializeField] private Weapon weapon;

	private float shotRotation;
	private bool canAttack = true;
	private Vector3 startPos;
	private bool playerIsLow;
	#endregion

	#region Startup/Shut down
	protected override void OnEnable()
	{
		base.OnEnable();
		animatorHelper.shootFrame += OnShootFrame;
		animatorHelper.attackHit += OnAttackHit;
		animatorHelper.goUnderground += Underground;
	}
	protected override void Awake()
	{
		base.Awake();
		startPos = transform.position;
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		animatorHelper.shootFrame -= OnShootFrame;
		animatorHelper.attackHit -= OnAttackHit;
		animatorHelper.goUnderground -= Underground;
	}
	#endregion

	#region Updates
	protected override void Update()
	{
		base.Update();
		AimAtTarget();
		UpdateAnimatorParams();
		if(target != null)
		{
			playerIsLow = target.position.y - transform.position.y <= 0;
		}
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

	#region Pathfinding Methods
	
	
	protected override void FlipSprite()
	{
		float shotXPos = Mathf.Abs(shotOrigin.localPosition.x);
		if (target != null && !isDead)
		{
			if (target.position.x < transform.position.x)
			{
				spriteRenderer.flipX = true;
				shotOrigin.localPosition = new Vector3(-shotXPos, shotOrigin.localPosition.y, 0);
			}
			else
			{
				spriteRenderer.flipX = false;
				shotOrigin.localPosition = new Vector3(shotXPos, shotOrigin.localPosition.y, 0);
			}
		}
	}
	#endregion

	#region Combat Methods
	private void DecideWhichAttack()
	{
		float distanceToPlayer = Mathf.Abs(Vector2.Distance(transform.position, target.transform.position));
		if (distanceToPlayer > wormStats.shootRange)
		{
			return;
		}

		if (distanceToPlayer <= wormStats.shootRange && distanceToPlayer > meleeRange)
		{
			StopAndShoot();
			return;
		}

		bool isTooHigh = target.position.y - transform.position.y > 3;

		if (playerIsLow)
		{
			StopAndSpecialAttack();
			return;
		}

		if (distanceToPlayer <= meleeRange && !isTooHigh)
		{
			StopAndAttack();
		}
		else
		{
			StopAndShoot();
			return;
		}
	}

	#region Shoot Methods
	private void AimAtTarget()
	{
		if (target != null)
		{
			Vector3 shootDir = target.position - shotOrigin.position;
			float rotation = (Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg) - 90.0f;
			weapon.shotRotation = rotation;
		}
	}
	private void StopAndShoot()
	{
		if (weapon.canShoot)
		{
			animator.SetTrigger("shoot");
			mover.SetDirectionalInput(Vector2.zero);
			shooter.shotRotation = shotRotation;
		}
	}
	private void OnShootFrame()
	{
		weapon.FireProjectile();
		animator.ResetTrigger("shoot");
	}
	#endregion

	#region Melee Methods
	private void StopAndAttack()
	{
		if (canAttack)
		{
			canAttack = false;
			mover.SetDirectionalInput(Vector2.zero);
			animator.SetTrigger("attack");
		}
	}
	private void OnAttackHit()
	{
		Vector2 playerDirection = (target.position - transform.position).normalized;
		RaycastHit2D hit = Physics2D.Raycast(transform.position, playerDirection, meleeRange, wormStats.canAttackLayer);
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

	#region Special Attack Methods
	private void StopAndSpecialAttack()
	{
		if (canAttack)
		{
			canAttack = false;
			mover.SetDirectionalInput(Vector2.zero);
			animator.SetTrigger("dig");
		}
	}
	private void Underground()
	{
		StartCoroutine(UndergroundRoutine());
	}
	private IEnumerator UndergroundRoutine()
	{
		yield return new WaitForSeconds(undergroundTime);
		if (playerIsLow)
		{
			transform.position = new Vector2(target.position.x, transform.position.y);
		}
		else
		{
			transform.position = startPos;
		}
		animator.SetTrigger("specialStrike");
	}
	#endregion

	#endregion

	#region Initialization Methods
	protected override void InitializeAnimatorParams()
	{
		animator.SetFloat("attackAnimSpeed", wormStats.attackAnimSpeed);
		animator.SetFloat("dieAnimSpeed", wormStats.dieAnimSpeed);
		animator.SetFloat("hitAnimSpeed", wormStats.hitAnimSpeed);
		animator.SetFloat("readyAnimSpeed", wormStats.readyAnimSpeed);
		animator.SetFloat("moveAnimSpeed", wormStats.moveAnimSpeed);
		animator.SetFloat("upAnimSpeed", wormStats.upAnimSpeed);
		animator.SetFloat("downAnimSpeed", wormStats.downAnimSpeed);
		animator.SetFloat("shootAnimSpeed", wormStats.shootAnimSpeed);
	}
	protected override void InitializeMoverParams()
	{
		base.InitializeMoverParams();
		mover.useGravity = true;
	}
	#endregion

	#region Abstract Method Implementations
	protected override void OnPlayerContact(Collider2D other)
	{
		// The worm can only damage the player when shooting or attacking.
		return;
	}
	protected override void MoveTowardPlayer()
	{
		Vector2 direction = (path.vectorPath[currentWaypoint] - transform.position).normalized;

		if (Vector2.Distance(transform.position, target.position) >= nextWayPointDistance)
		{
			mover.SetDirectionalInput(direction);
		}

		DecideWhichAttack();
	}
	#endregion
}
