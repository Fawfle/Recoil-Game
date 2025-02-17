using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
	public double highScore = 0;

	public int deaths = 0;
	public int shotsFired = 0;

	public double playTimeSeconds = 0f;

	public int endlessRuns = 0;

	public int highestLeaderboardRank = -1;
	public int currentLeaderboardRank = -1;

	// settings
	public float volume = 1f;
	public bool clickToShootEnabled = false;
	public bool levelIntroPanEnabled = true;

	public Color playerColor = new Color(0.3254902f, 0.6156863f, 1f); // initial player color
	public SkinKey playerSkin = SkinKey.Default;

	public bool leaderboardCrownEnabled = true;

	public List<LevelCompleteData> completedLevels = new();

	//currently preemptive, can be used if I want to add time and stuff
	[System.Serializable]
	public class LevelCompleteData
	{
		public string levelKey;
		public int levelIndex;

		public LevelCompleteData(string levelKey, int levelIndex)
		{
			this.levelKey = levelKey;
			this.levelIndex = levelIndex;
		}
	}
}