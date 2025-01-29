using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelectUI : MonoBehaviour
{
	[SerializeField] private Color completedColor = Color.green;
	private Color uncompletedColor = Color.white;

	[SerializeField] private TextMeshProUGUI numberText;

	private int levelNumber;
	private string levelKey;

	private bool completed = false;

	private void Awake()
	{
		uncompletedColor = numberText.color;
	}

	public void UpdateLevel(int num, string key)
	{
		levelNumber = num;
		levelKey = key;
		//completed = SaveManager.HasCompletedLevel(key);

		numberText.color = completed ? completedColor : uncompletedColor;

		numberText.text = (levelNumber + 1).ToString();
	}

	public void UpdateLevel(int num)
	{
		UpdateLevel(num, LevelsManager.levels[num]);
	}
}