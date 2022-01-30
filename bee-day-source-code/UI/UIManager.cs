using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
	#region Field Declarations
	[SerializeField] public Player player;

    [SerializeField] private GameObject winScreen;
	[SerializeField] private GameObject loseScreen;
	[SerializeField] private GameObject currentLevelScreen;
	[SerializeField] private TextMeshProUGUI currentLevelText;

	[SerializeField] private GameObject tutorialScreen0;
	[SerializeField] private GameObject tutorialScreen1;
	[SerializeField] private GameObject tutorialScreen2;
	[SerializeField] private GameObject tutorialScreen3;

	[SerializeField] private ObjectiveDisplay objectiveDisplay;
	[SerializeField] private WeaponsDisplay weaponsDisplay;
	[SerializeField] private TimeDisplay timeDisplay;
	[SerializeField] private PlayerHealthDisplay playerHealthDisplay;
	[SerializeField] private GameObject pauseScreen;

	private int nextScreen;
	#endregion

	#region Start Up/Shutdown
	private void OnDisable()
	{
		player.newObjective -= UpdateObjective;
		player.objectiveComplete -= CompleteObjective;
		player.switchWeapon -= SwitchWeapon;
		player.health.changed -= UpdateHealthDisplay;
	}
	#endregion

	public void StartNewLevel()
	{
		player = GameObject.Find("Player").GetComponent<Player>();
		HideWinScreen();
		HideLoseScreen();
		objectiveDisplay.ResetObjectiveDisplay();
		weaponsDisplay.StartLevel();
		player.newObjective += UpdateObjective;
		player.objectiveComplete += CompleteObjective;
		player.switchWeapon += SwitchWeapon;
		player.health.changed += UpdateHealthDisplay;
		player.health.full += UpdateHealthDisplay;
	}

	#region Objective Display
	public void UpdateObjective(FlowerColor flowerColor)
	{
		objectiveDisplay.SetObjective(flowerColor);
	}
	public void CompleteObjective()
	{
		objectiveDisplay.CompleteObjective();
	}
	#endregion

	#region Weapons/Weapons Resource Display
	public void SwitchWeapon(int weaponNum)
	{
		weaponsDisplay.SwitchWeapon(weaponNum);
	}
    #endregion

    #region Time Display
	public void RunTime(string strTimeLeft)
	{
		timeDisplay.UpdateTimeText(strTimeLeft);
	}
	public void ChangeTimeColor(Color color)
	{
		timeDisplay.ChangeTimeColor(color);
	}
	#endregion

	#region Health Display
	private void UpdateHealthDisplay()
	{
		float playerCurrentHealth = player.health.GetCurrentHealthPercentage();
		playerHealthDisplay.UpdateHealthDisplay(playerCurrentHealth);
	}
    #endregion

    #region Game Flow Management Screens
	public void TogglePause()
	{
		if (!pauseScreen.activeInHierarchy)
		{
			pauseScreen.SetActive(true);
		}
		else
		{
			pauseScreen.SetActive(false);
		}
	}
    #endregion

    #region Win And Lose Screens
    public void DisplayWinScreen()
	{
		winScreen.SetActive(true);
	}
	public void HideWinScreen()
	{
		winScreen.SetActive(false);
	}
	public void DisplayLoseScreen()
	{
		loseScreen.SetActive(true);
	}
	public void HideLoseScreen()
	{
		loseScreen.SetActive(false);
	}
	public void ShowCurrentLevelScreen(int level)
	{
		if(level == 0)
		{
			currentLevelText.text = "Tutorial";
		}
		else
		{
			currentLevelText.text = "Level " + level;
		}
		currentLevelScreen.SetActive(true);
		StartCoroutine(ShowCurrentLevelRoutine(level));
	}
	public IEnumerator ShowCurrentLevelRoutine(int level)
	{
		yield return new WaitForSeconds(0.2f);
		currentLevelScreen.SetActive(false);
		yield return new WaitForSeconds(0.2f);
		currentLevelScreen.SetActive(true);
		yield return new WaitForSeconds(0.2f);
		currentLevelScreen.SetActive(false);
		yield return new WaitForSeconds(0.2f);
		currentLevelScreen.SetActive(true);
		yield return new WaitForSeconds(0.2f);
		currentLevelScreen.SetActive(false);
		yield return new WaitForSeconds(0.2f);
		currentLevelScreen.SetActive(true);
		yield return new WaitForSeconds(0.2f);
		currentLevelScreen.SetActive(false);
		if(level == 0)
		{
			Time.timeScale = 0;
			ShowTutorialScreen(nextScreen);
		}
	}
	public void ShowTutorialScreen(int screen)
	{
		tutorialScreen0.SetActive(true);
		nextScreen++;
	}
	public void SkipTutorial()
	{
		tutorialScreen0.SetActive(false);
		tutorialScreen1.SetActive(false);
		tutorialScreen2.SetActive(false);
		tutorialScreen3.SetActive(false);
		nextScreen = 0;
		Time.timeScale = 1;
	}
	public void NextTutorialScreen()
	{
		tutorialScreen0.SetActive(false);
		tutorialScreen1.SetActive(false);
		tutorialScreen2.SetActive(false);
		tutorialScreen3.SetActive(false);
		switch (nextScreen)
		{
			case 1:
				tutorialScreen1.SetActive(true);
				nextScreen++;
				break;
			case 2:
				tutorialScreen2.SetActive(true);
				nextScreen++;
				break;
			case 3:
				tutorialScreen3.SetActive(true);
				nextScreen++;
				break;
			case 4:
				nextScreen = 0;
				Time.timeScale = 1;
				break;
		}
	}
	#endregion
}
