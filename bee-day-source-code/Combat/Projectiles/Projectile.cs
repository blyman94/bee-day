using UnityEngine;

public class Projectile : MonoBehaviour
{
	[HideInInspector] public Transform parentTransform;
	[HideInInspector] public string hitTag;
	public float projectileShotForce = 20;
	[SerializeField] protected float projectileDamage = 10.0f;

	private void Awake()
	{
		parentTransform = transform.parent;
	}

	protected virtual void ResetProjectile()
	{
		transform.parent = parentTransform;
		gameObject.SetActive(false);
	}

	protected virtual void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag(hitTag))
		{
			Health targetHealth = other.gameObject.GetComponent<Health>();
			if(targetHealth != null)
			{
				targetHealth.TakeDamage(projectileDamage);
			}
			ResetProjectile();
		}
		else if (other.gameObject.CompareTag("Obstacle") || other.gameObject.CompareTag("Dead") ||
			other.gameObject.CompareTag("Enemy"))
		{
			ResetProjectile();
		}
	}
	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.transform.CompareTag("Destroyer"))
		{
			ResetProjectile();
		}
	}
}
