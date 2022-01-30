using UnityEngine;
using UnityEngine.UI;

public class WeaponsDisplay : MonoBehaviour
{
	[SerializeField] private Image weapon0Background;
	[SerializeField] private Image weapon1Background;
	[SerializeField] private Image weapon2Background;
	[SerializeField] private GameObject weapon0Label;
	[SerializeField] private GameObject weapon1Label;
	[SerializeField] private GameObject weapon2Label;

	private int currentWeapon;
	public void StartLevel()
	{
		ResetWeaponsDisplay();
		weapon0Background.color = Color.yellow;
		weapon0Label.SetActive(true);
		currentWeapon = 0;
	}

	private void ResetWeaponsDisplay()
	{
		weapon0Background.color = Color.white;
		weapon1Background.color = Color.white;
		weapon2Background.color = Color.white;
	}

	public void SwitchWeapon(int weaponNum)
	{
		
		switch (weaponNum)
		{
			case 0:
				if(currentWeapon != 0)
				{
					ResetWeaponsDisplay();
					weapon0Background.color = Color.yellow;
					currentWeapon = 0;
				}
				break;
			case 1:
				if (currentWeapon != 1)
				{
					ResetWeaponsDisplay();
					weapon1Background.color = Color.yellow;
					currentWeapon = 1;
				}
				break;
			case 2:
				if (currentWeapon != 2)
				{
					ResetWeaponsDisplay();
					weapon2Background.color = Color.yellow;
					currentWeapon = 2;
				}
				break;
		}
	}
}
