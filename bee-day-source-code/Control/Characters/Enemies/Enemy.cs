using UnityEngine;
using Pathfinding;
using System.Collections;

public enum EnemyClass { BASIC, GOOD, BETTER, BEST, INSANE }
public abstract class Enemy : MonoBehaviour
{
    #region Field Declarations
    [Header("Identification")]
	[Space]
	[SerializeField] protected string EnemyType;
	[SerializeField] protected EnemyClass enemyClass;
	[SerializeField] protected string EnemyDescription;
	[SerializeField] protected int EnemyID;

	[Header("Stats")]
	[Space]
	[SerializeField] protected EnemyStats enemyStats;

	[Header("Colliders")]
	[Space]
	[SerializeField] protected BoxCollider2D mainCollider;
	[SerializeField] protected CircleCollider2D aggroCollider;
	[SerializeField] protected CircleCollider2D damageCollider;

	[Header("Enemy Components")]
	[Space]
	[SerializeField] protected Mover mover;

	[Header("Graphics")]
	[Space]
	[SerializeField] protected Animator animator;
	[SerializeField] protected SpriteRenderer spriteRenderer;

	[Header("Health")]
	[Space]
	[SerializeField] protected Health health;
	[SerializeField] public AudioSource damageAudio;
	[SerializeField] public AudioClip damageClip;

	[Header("Pathfinding")]
	[Space]
	[SerializeField] protected Seeker seeker;
	[SerializeField] protected float nextWayPointDistance = 1.5f;
	[SerializeField] protected float pathUpdateRate = 0.75f;

	[Header("Poison")]
	[Space]
	[SerializeField] protected GameObject poisonVFX;
	[SerializeField] protected Color poisonColor;

	protected float meleeDamage;
	protected float maxHealth;
	protected float meleeCooldown;
	protected float moveSpeed;

	protected bool isDead;
	protected bool isAggro;
	protected bool isPoisoned;
	protected Path path;
	protected Transform target;
	protected int currentWaypoint = 0;
	protected float currentMoveSpeed;
	protected Coroutine poisonRoutine;
	#endregion

	#region Startup/Shutdown
	protected virtual void OnEnable()
	{
		health.changed += Aggro;
		health.empty += Die;
	}
	protected virtual void Awake()
	{
		InitializeClassParams();
		InitializeAnimatorParams();
		InitializeHealthParams();
		InitializeMoverParams();
	}
	protected void Start()
	{
		InvokeRepeating("UpdatePath", 0f, pathUpdateRate);
	}
	protected virtual void OnDisable()
	{
		health.changed -= Aggro;
		health.empty -= Die;
	}
    #endregion

    #region Updates
    protected virtual void Update()
	{
		ChasePlayer();
		FlipSprite();
	}
	#endregion

	#region Abstract Methods
	protected abstract void OnPlayerContact(Collider2D other);
	protected abstract void MoveTowardPlayer();
	protected abstract void InitializeAnimatorParams();
	#endregion

	#region Initialization Methods
	protected virtual void InitializeClassParams()
	{
		switch (enemyClass)
		{
			case EnemyClass.BASIC:
				meleeDamage = enemyStats.meleeDamage;
				meleeCooldown = enemyStats.meleeCooldown;
				maxHealth = enemyStats.maxHealth;
				moveSpeed = enemyStats.moveSpeed;
				break;
			case EnemyClass.GOOD:
				meleeDamage = enemyStats.meleeDamage * 1.1f;
				meleeCooldown = enemyStats.meleeCooldown * 0.9f;
				maxHealth = enemyStats.maxHealth * 1.1f;
				moveSpeed = enemyStats.moveSpeed * 1.1f;
				break;
			case EnemyClass.BETTER:
				meleeDamage = enemyStats.meleeDamage * 1.3f;
				meleeCooldown = enemyStats.meleeCooldown * 0.7f;
				maxHealth = enemyStats.maxHealth * 1.3f;
				moveSpeed = enemyStats.moveSpeed * 1.3f;
				break;
			case EnemyClass.BEST:
				meleeDamage = enemyStats.meleeDamage * 1.5f;
				meleeCooldown = enemyStats.meleeCooldown * 0.5f;
				maxHealth = enemyStats.maxHealth * 1.5f;
				moveSpeed = enemyStats.moveSpeed * 1.5f;
				break;
			case EnemyClass.INSANE:
				meleeDamage = enemyStats.meleeDamage * 2.0f;
				meleeCooldown = enemyStats.meleeCooldown * 0.4f;
				maxHealth = enemyStats.maxHealth * 2.0f;
				moveSpeed = enemyStats.moveSpeed * 2.0f;
				break;
			default:
				Debug.LogError("Enemy.cs :: Unrecognized enemy class passed to InitializeClassParams() function.");
				break;
		}
	}
	protected virtual void InitializeHealthParams()
	{
		health.spriteRenderer = spriteRenderer;
		health.maxHealth = maxHealth;
		health.damageAudio = damageAudio;
		health.damageClip = damageClip;
	}
	protected virtual void InitializeMoverParams()
	{
		mover.bc2d = mainCollider;
		mover.groundLayer = enemyStats.groundLayer;
		mover.xAccelGround = enemyStats.xAccelGround;
		mover.moveSpeed = moveSpeed;
		mover.xAccelAir = enemyStats.xAccelAir;
	}
	#endregion

	#region Pathfinding Methods
	protected virtual void ChasePlayer()
	{
		if (isAggro && !isDead)
		{
			if (path == null)
			{
				return;
			}
			if (currentWaypoint >= path.vectorPath.Count)
			{
				return;
			}

			MoveTowardPlayer();

			float distance = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
			if (distance < nextWayPointDistance)
			{
				currentWaypoint++;
			}
		}
		else
		{
			mover.SetDirectionalInput(Vector2.zero);
		}
	}
	protected void OnPathComplete(Path p)
	{
		if (!p.error)
		{
			path = p;
			currentWaypoint = 0;
		}
	}
	protected void UpdatePath()
	{
		if (!isAggro || isDead)
		{
			return;
		}

		if (seeker.IsDone())
		{
			seeker.StartPath(transform.position, target.position, OnPathComplete);
		}
	}
	#endregion

	#region Behaviour Methods
	public void AddPoison(float duration)
	{
		if (!isPoisoned)
		{
			isPoisoned = true;
			health.isPoisoned = true;
			currentMoveSpeed = mover.moveSpeed;
			mover.moveSpeed *= 0.5f;
			poisonVFX.SetActive(true);
			spriteRenderer.color = poisonColor;
			poisonRoutine = StartCoroutine(PoisonRoutine(duration));
		}
		else
		{
			StopCoroutine(poisonRoutine);
			poisonRoutine = StartCoroutine(PoisonRoutine(duration));
		}
	}
	public void RemovePoison()
	{
		mover.moveSpeed = currentMoveSpeed;
		poisonVFX.SetActive(false);
		spriteRenderer.color = Color.white;
		isPoisoned = false;
		health.isPoisoned = false;
	}
	public IEnumerator PoisonRoutine(float duration)
	{
		yield return new WaitForSeconds(duration);
		RemovePoison();
	}
	protected virtual void Aggro()
	{
		if (target == null)
		{
			target = GameObject.FindGameObjectWithTag("Player").transform;
		}
		isAggro = true;
		aggroCollider.enabled = false;
	}
	protected virtual void Die()
	{
		mover.useGravity = true;
		animator.SetTrigger("die");
		gameObject.tag = "Dead";
		isDead = true;
	}
	protected virtual void FlipSprite()
	{
		if (target != null && !isDead)
		{
			if (target.position.x < transform.position.x)
			{
				spriteRenderer.flipX = true;
			}
			else
			{
				spriteRenderer.flipX = false;
			}
		}
	}
	protected virtual void OnTriggerEnter2D(Collider2D other)
	{
		if (other.transform.CompareTag("Player"))
		{
			if (!isAggro)
			{
				target = other.transform;
				Aggro();
			}
			else
			{
				if (!isDead)
				{
					OnPlayerContact(other);
				}
			}
		}
	}
    #endregion
}
