using System.Collections;
using UnityEngine;

public class BeeBombProjectile : Projectile
{
	[SerializeField] private CircleCollider2D explosionRadius;
	[SerializeField] private BoxCollider2D initalCollider;
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private GameObject explosionVFX;
	[SerializeField] private float radiusGrowthPerTick = 0.5f;
	[SerializeField] private float tickRate = 0.25f;
	[SerializeField] private float maxRadiusSize = 2.0f;
	[SerializeField] private float explosionShakeIntensity = 2;
	[SerializeField] private float explosionShakeTime = 0.25f;
	[SerializeField] private AudioSource projectileAudio;
	[SerializeField] private AudioClip explosionClip;
	private bool isExploded;

	public delegate void ShakeCamera(float explosionShakeIntensity, float explosionShakeTime);
	public static ShakeCamera shakeCamera;

	private void OnEnable()
	{
		isExploded = false;
		spriteRenderer.enabled = true;
		explosionRadius.radius = 0.01f;
	}

	protected override void OnCollisionEnter2D(Collision2D other)
	{
		if (!isExploded && !other.gameObject.CompareTag("Player"))
		{
			Explode();
			shakeCamera?.Invoke(explosionShakeIntensity,explosionShakeTime);
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (isExploded && (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy")))
		{
			Health targetHealth = other.gameObject.GetComponent<Health>();
			if (targetHealth != null)
			{
				targetHealth.TakeDamage(projectileDamage);
			}
		}
	}

	protected override void ResetProjectile()
	{
		base.ResetProjectile();
		explosionVFX.SetActive(false);
	}

	private void Explode()
	{
		isExploded = true;
		projectileAudio.PlayOneShot(explosionClip);
		spriteRenderer.enabled = false;
		explosionVFX.SetActive(true);
		StartCoroutine(ExplodeRoutine());
	}

	private IEnumerator ExplodeRoutine()
	{
		while(explosionRadius.radius < maxRadiusSize)
		{
			explosionRadius.radius += radiusGrowthPerTick;
			yield return new WaitForSeconds(tickRate);
		}
		gameObject.GetComponent<SpriteRenderer>().enabled = false;
		yield return new WaitForSeconds(2.0f);
		ResetProjectile();
	}
}
