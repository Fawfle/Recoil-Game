using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public static class SaveManager
{
	private static readonly string SAVE_FILE = "Save.txt";

	public static SaveData save = InitializeSave();

	private static SaveData InitializeSave()
	{
		Debug.Log($"Initialize Save (Path:{Path.Combine(Application.persistentDataPath, SAVE_FILE)})");

		save = LoadSave();

		if (save == null)
		{
			save = new();
			WriteToSave(save);
		}

		return save;
	}

	public static bool HasCompletedLevel(string key)
	{
		foreach (var level in save.completedLevels)
		{
			if (level.levelKey == key) return true;
		}

		return false;
	}

	public static void UpdateLevelCompleted(SaveData.LevelCompleteData data)
	{
		if (!HasCompletedLevel(data.levelKey))
		{
			save.completedLevels.Add(data);

			save.completedLevels.Sort((a, b) => a.levelIndex - b.levelIndex);
		} else
		{
			foreach (var level in save.completedLevels)
			{
				if (level.levelKey == data.levelKey)
				{
					// update stats
					break;
				}
			}
		}

		WriteSaveToSave();
	}

	public static void AddDeltaTime()
	{
		save.playTime += Time.deltaTime;
	}

	public static void IncrementShotsFired()
	{
		save.shotsFired++;
	}

	public static void IncrementDeaths()
	{
		save.deaths++;

		//WriteToSave(save); for "performance"
	}

	public static void IncrementEndlessRuns()
	{
		save.endlessRuns++;

		//WriteToSave(save); for "performance"
	}

	public static void SetSaveHighScore(double score, bool write = true)
	{
		save.highScore = score;

		if (write) WriteSaveToSave();
	}

	public static void SetSaveVolume(float value, bool write = true)
	{
		save.volume = value;

		if (write) WriteSaveToSave();
	}

	public static void SetSaveClickToShoot(bool isOn, bool write = true)
	{
		save.clickToShootEnabled = isOn;

		if (write) WriteSaveToSave();
	}

	public static bool WriteSaveToSave()
	{
		return WriteToSave(save);
	}

	public static bool WriteToSave(SaveData data)
	{
		var fullPath = Path.Combine(Application.persistentDataPath, SAVE_FILE);

		try
		{
			File.WriteAllText(fullPath, ToJson(data));
			Debug.Log("Saved data: " + ToJson(data));
			Debug.Log("Stored data: " + ToJson(LoadSave()));
			return true;
		}
		catch (Exception e)
		{
			Debug.LogError($"Failed to write to {fullPath} with exception {e}");
			return false;
		}
	}

	public static SaveData LoadSave()
	{
		var fullPath = Path.Combine(Application.persistentDataPath, SAVE_FILE);

		try
		{
			string contents = File.ReadAllText(fullPath);
			return LoadFromJson(contents);
		}
		catch (Exception e)
		{
			Debug.LogError($"Failed to read from {fullPath} with exception {e}");
			return null;
		}
	}

	public static SaveData LoadFromJson(string json)
	{
		return JsonUtility.FromJson<SaveData>(json);
	}

	public static string ToJson(SaveData data)
	{
		return JsonUtility.ToJson(data);
	}
}