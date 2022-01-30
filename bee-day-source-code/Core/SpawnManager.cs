using UnityEngine;
using System.Collections.Generic;
using System;

public class SpawnManager : Singleton<SpawnManager>
{
	public const int enemyLimit = 26;

	[Header("Objectives Spawning")]
	[SerializeField] private List<Transform> flowerSpawnPoints;
	[SerializeField] private List<GameObject> flowerPrefabs;

	[Header("Enemy Spawning")]
	[Space]
	[SerializeField] private List<Transform> batSpawnPoints;
	[SerializeField] private List<Transform> enemySpawnPoints;

	[Header("Enemie Lists")]
	[Space]
	[SerializeField] private List<GameObject> batEnemies;
	[SerializeField] private List<GameObject> tierOneEnemies;
	[SerializeField] private List<GameObject> tierTwoEnemies;
	[SerializeField] private List<GameObject> tierThreeEnemies;
	[SerializeField] private List<GameObject> tierFourEnemies;
	[SerializeField] private List<GameObject> tierFiveEnemies;
	[SerializeField] private List<GameObject> tierSixEnemies;
	[SerializeField] private List<GameObject> tierSevenEnemies;

	private List<Transform> currentObjectiveSpawnPointList;
	private List<GameObject> currentFlowerPrefabs;

	private List<Transform> currentBatSpawnPointList;
	private List<Transform> currentEnemySpawnPointList;

	private readonly GameObject[] instantiatedFlowers = new GameObject[6];
	private List<GameObject> instantiatedEnemies = new List<GameObject>();
	private List<GameObject> currentEnemyList = new List<GameObject>();
	private List<GameObject> currentBatList = new List<GameObject>();

	public void StartLevel(int level)
	{
		SpawnFlowers();
		SpawnEnemies(level);
	}

	public void SpawnEnemies(int level)
	{
		if(instantiatedEnemies.Count > 0)
		{
			ClearEnemies();
			instantiatedEnemies.Clear();
		}
		int numEnemies = 0;
		if(level == 0)
		{
			// No enemies spawn in the tutorial
			return;
		}

		int enemyCap = Convert.ToInt32(level * 1.5f);

		UpdateCurrentEnemyList(level);
		currentBatSpawnPointList = new List<Transform>(batSpawnPoints);
		currentEnemySpawnPointList = new List<Transform>(enemySpawnPoints);

		if(enemyCap > enemyLimit)
		{
			enemyCap = enemyLimit;
		}

		while (numEnemies <= enemyCap)
		{
			numEnemies++;
			float spawnType = UnityEngine.Random.Range(0.0f, 1.0f);
			if(spawnType <= 0.25f)
			{
				SpawnBat();
			}
			else
			{
				SpawnEnemy();
			}
		}
	}
	public void SpawnBat()
	{
		if(currentBatSpawnPointList.Count > 0)
		{
			int spawnPointIndex = UnityEngine.Random.Range(0, currentBatSpawnPointList.Count);
			int batTypeIndex = UnityEngine.Random.Range(0, currentBatList.Count);
			instantiatedEnemies.Add(Instantiate(currentBatList[batTypeIndex],currentBatSpawnPointList[spawnPointIndex]));
			currentBatSpawnPointList.RemoveAt(spawnPointIndex);
		}
	}
	public void SpawnEnemy()
	{
		if (currentEnemySpawnPointList.Count > 0)
		{
			int spawnPointIndex = UnityEngine.Random.Range(0, currentEnemySpawnPointList.Count);
			int enemyTypeIndex = UnityEngine.Random.Range(0, currentEnemyList.Count);
			instantiatedEnemies.Add(Instantiate(currentEnemyList[enemyTypeIndex], currentEnemySpawnPointList[spawnPointIndex]));
			currentEnemySpawnPointList.RemoveAt(spawnPointIndex);
		}
	}

	public void ClearEnemies()
	{
		foreach (GameObject enemy in instantiatedEnemies)
		{
			Destroy(enemy);
		}
	}

	public void SpawnFlowers()
	{
		ClearFlowers();
		currentObjectiveSpawnPointList = new List<Transform>(flowerSpawnPoints);
		currentFlowerPrefabs = flowerPrefabs;

		for(int i = 0; i < currentFlowerPrefabs.Count; i++)
		{
			int spawnPointIndex = UnityEngine.Random.Range(0, currentObjectiveSpawnPointList.Count);
			instantiatedFlowers[i] = Instantiate(flowerPrefabs[i], currentObjectiveSpawnPointList[spawnPointIndex]);
			currentObjectiveSpawnPointList.RemoveAt(spawnPointIndex);
		}
	}
	public void ClearFlowers()
	{
		foreach(GameObject flower in instantiatedFlowers)
		{
			Destroy(flower);
		}
	}
	private void UpdateCurrentEnemyList(int level)
	{
		if (level == 1)
		{
			currentEnemyList.AddRange(tierOneEnemies);
			currentBatList.Add(batEnemies[0]);
		}
		else if (level == 3)
		{
			currentEnemyList.AddRange(tierTwoEnemies);
			currentBatList.Add(batEnemies[1]);
		}
		else if (level == 5)
		{
			currentEnemyList.AddRange(tierThreeEnemies);
			currentBatList.Add(batEnemies[2]);
		}
		else if (level == 7)
		{
			currentEnemyList.AddRange(tierFourEnemies);
			currentBatList.Add(batEnemies[3]);
		}
		else if (level == 10)
		{
			currentEnemyList.AddRange(tierFiveEnemies);
		}
		else if (level == 13)
		{
			currentEnemyList.AddRange(tierSixEnemies);
		}
		else if (level == 17)
		{
			currentEnemyList.AddRange(tierSevenEnemies);
			currentBatList.Add(batEnemies[4]);
		}
	}
}
