using System;
using System.IO;
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

	public static void WriteHighScoreToSave(double score)
	{
		save.highScore = score;

		WriteToSave(save);
	}

	public static void WriteVolumeToSave(float value)
	{
		save.volume = value;

		WriteToSave(save);
	}

	public static void WriteClickToShootToSave(bool isOn)
	{
		save.clickToShootEnabled = isOn;

		WriteToSave(save);
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