using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsManager : MonoBehaviour
{
	public static LevelsManager Instance { get; private set; }

	// level keys are used so levels can be modified and updated (trailing letter indicates version) by scene name and for saving/updating to new level versions.

	[SerializeField] private List<string> levelKeys;

	public static string[] levels;

	// not to be confused with levelmanager
	private void Awake()
	{
		if (Instance != null && Instance != this) { Destroy(gameObject); return; }
		else Instance = this;

		DontDestroyOnLoad(gameObject);

		levels = levelKeys.ToArray();
	}
}
