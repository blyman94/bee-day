using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthDisplay : MonoBehaviour
{
	[SerializeField] private Image currentHealthImage;
	private float currentHealthImageXStart;

	private void Start()
	{
		currentHealthImageXStart = currentHealthImage.rectTransform.localScale.x;
		currentHealthImage.color = Color.cyan;
	}

	public void UpdateHealthDisplay(float currentHealthPercentage)
	{
		Vector3 currentScale = currentHealthImage.rectTransform.localScale;
		currentScale.x = currentHealthPercentage * currentHealthImageXStart;
		currentHealthImage.rectTransform.localScale = currentScale;
		if (currentHealthPercentage <= 0.2f)
		{
			currentHealthImage.color = Color.magenta;
		}
		else if (currentHealthPercentage <= 0.5f)
		{
			currentHealthImage.color = Color.yellow;
		}
		else
		{
			currentHealthImage.color = Color.cyan;
		}
	}
}
