using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// manager for storing and accessing different levels
public class LevelsManager : MonoBehaviour
{
	public static LevelsManager Instance { get; private set; }

	// level keys are used so levels can be modified and updated (trailing letter indicates version) by scene name and for saving/updating to new level versions.

	[SerializeField] private List<string> levels;

	// not to be confused with levelmanager
	private void Awake()
	{
		if (Instance != null && Instance != this) { Destroy(gameObject); return; }
		else Instance = this;

		DontDestroyOnLoad(gameObject);
	}

	public static List<string> Levels { get { return Instance.levels; } }

	public static string GetNextLevel()
	{
		int currentLevelIndex = GetCurrentLevelIndex();

		if (currentLevelIndex == Levels.Count - 1 || currentLevelIndex == -1) return null;
		
		return Levels[currentLevelIndex + 1];
	}

	public static string GetCurrentLevelKey()
	{
		string testKey = SceneManager.GetActiveScene().name;

		if (Levels.Contains(testKey)) return testKey;

		return null;
	}

	public static int GetCurrentLevelIndex()
	{
		return Levels.IndexOf(SceneManager.GetActiveScene().name);
	}
}
