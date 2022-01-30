using TMPro;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI timeRemainingText;

	public void UpdateTimeText(string timeRemaining)
	{
		timeRemainingText.text = timeRemaining;
	}
	public void ChangeTimeColor(Color color)
	{
		timeRemainingText.color = color;
	}
}
