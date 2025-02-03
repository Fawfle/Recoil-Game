using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
	private Button button;

	[SerializeField] private Color completedColor = Color.green;
	private Color uncompletedColor = Color.white;

	[SerializeField] private TextMeshProUGUI numberText;

	private int levelNumber;
	private string levelKey = null;

	private bool completed = false;

	public void UpdateLevel(int num, string key)
	{
		if (levelKey != null) return;

		uncompletedColor = numberText.color;
		button = GetComponent<Button>();

		levelNumber = num;
		levelKey = key;
		completed = SaveManager.HasCompletedLevel(key);

		numberText.color = completed ? completedColor : uncompletedColor;

		numberText.text = (levelNumber + 1).ToString();

		button.onClick.AddListener(() => TransitionManager.Instance.LoadScene(key));
	}

	public void UpdateLevel(int num)
	{
		UpdateLevel(num, LevelsManager.Levels[num]);
	}
}