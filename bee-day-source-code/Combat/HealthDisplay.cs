using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
	[SerializeField] private Health health;
	[SerializeField] private SpriteRenderer currentHealthSpriteRenderer;
	
	private float healthSpriteRenderXStart;

	#region Start Up/Shutdown
	private void OnEnable()
	{
		health.changed += UpdateHealthDisplay;	
	}
	private void Start()
	{
		healthSpriteRenderXStart = currentHealthSpriteRenderer.transform.localScale.x;
		currentHealthSpriteRenderer.color = Color.cyan;
	}
	private void OnDisable()
	{
		health.changed -= UpdateHealthDisplay;
	}
	#endregion

	private void UpdateHealthDisplay()
	{
		Vector3 currentHealthScale = currentHealthSpriteRenderer.transform.localScale;
		float currentHealthPercentage = health.GetCurrentHealthPercentage();
		currentHealthScale.x = currentHealthPercentage * healthSpriteRenderXStart;
		currentHealthSpriteRenderer.transform.localScale = currentHealthScale;
		if(currentHealthPercentage <= 0.2f)
		{
			currentHealthSpriteRenderer.color = Color.magenta;
		}
		else if(currentHealthPercentage <= 0.5f)
		{
			currentHealthSpriteRenderer.color = Color.yellow;
		}
		else
		{
			currentHealthSpriteRenderer.color = Color.cyan;
		}
	}
}
