using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
	public double highScore = 0;

	public float volume = 1f;

	public bool clickToShootEnabled = false;

	public List<LevelCompleteData> completedLevels = new();

	//currently preemptive, can be used if I want to add time and stuff
	[System.Serializable]
	public class LevelCompleteData
	{
		public string levelKey;
	}
}