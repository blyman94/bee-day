using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	#region Properties
	public static T Instance { get; private set; }
	public static bool IsInitialized => Instance != null;
	#endregion

	#region Start Up
	protected virtual void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("[Singleton] Trying to instantiate a second" +
				"instance of a singleton class.");
		}
		else
		{
			Instance = (T)this;
		}

		DontDestroyOnLoad(this);
	}
	#endregion

	#region Behaviour Methods
	protected void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}
	#endregion
}
