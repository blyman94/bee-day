using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    #region Field Declarations
    [SerializeField] private Player player;
	[SerializeField] private UIManager uIManager;
	[SerializeField] private SpawnManager spawnManager;

	private readonly int timeLimit = 2;
	public static bool isPaused;
	private int level = 0;
	TimeSpan timeLeft;
	#endregion

	#region Start Up/Shutdown
	private void OnEnable()
	{
		SceneManager.sceneLoaded += StartLevel;
	}

	private void StartLevel(Scene scene, LoadSceneMode mode)
	{
		uIManager.ShowCurrentLevelScreen(level);
		player = GameObject.Find("Player").GetComponent<Player>();
		player.win += WinGame;
		player.lose += LoseGame;
		uIManager.player = player;
		uIManager.StartNewLevel();
		timeLeft = TimeSpan.FromMinutes(timeLimit);
		spawnManager.StartLevel(level);
		InvokeRepeating("RunTime", 1.0f, 1.0f);
	}

	private void OnDisable()
	{
		player.win -= WinGame;
		player.lose -= LoseGame;
	}
	#endregion

	#region Updates
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (!isPaused)
			{
				PauseGame();
			}
			else
			{
				UnpauseGame();
			}
			
		}
	}
	#endregion

	#region Game Flow Management
	public void PauseGame()
	{
		uIManager.TogglePause();
		isPaused = true;
		Time.timeScale = 0;
		Cursor.visible = true;
	}
	public void UnpauseGame()
	{
		uIManager.TogglePause();
		isPaused = false;
		Time.timeScale = 1;
		Cursor.visible = false;
	}
	public void QuitGame()
	{
		Application.Quit();
	}
	public void RestartGame()
	{
		level = 1;
		SceneManager.LoadScene("Game");
	}
	#endregion

	#region Win and Lose
	private void RunTime()
	{
		timeLeft -= new TimeSpan(0, 0, 1);
		string strTimeLeft = timeLeft.ToString("m':'ss");
		uIManager.RunTime(strTimeLeft);

		if (timeLeft <= new TimeSpan(0, 0, 10))
		{
			uIManager.ChangeTimeColor(Color.magenta);
		}
		else if (timeLeft <= new TimeSpan(0, 0, 30))
		{
			uIManager.ChangeTimeColor(Color.yellow);
		}
		else if (timeLeft <= new TimeSpan(0, 1, 0))
		{
			uIManager.ChangeTimeColor(Color.cyan);
		}
		else
		{
			uIManager.ChangeTimeColor(Color.white);
		}

		if (timeLeft == new TimeSpan(0, 0, 0))
		{
			player.Die();
		}
	}
	public void WinGame()
	{
		CancelInvoke();
		HideCrosshair();
		uIManager.DisplayWinScreen();
		level ++;
	}
	public void NextLevel()
	{
		SceneManager.LoadScene("Game");
	}
	private void LoseGame()
	{
		CancelInvoke();
		HideCrosshair();
		uIManager.DisplayLoseScreen();
	}
	#endregion

    #region Misc.
    public void HideCrosshair()
	{
		Cursor.visible = true;
		player.crosshair.SetActive(false);
	}
    #endregion
}
