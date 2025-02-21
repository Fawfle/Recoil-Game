using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance { get; private set; }

	public Player player;
	public BackgroundScroller background;

    [SerializeField] private GameState state;

    public event Action OnGameInit, OnGamePlay, OnGameEnd, OnGameOver, OnLevelComplete;

	[SerializeField] private GameMode mode;

	// probably shouldn't need to handle states and scores (should've put them in endlessLevelManager), but bad code is whatever

	public float score { get; private set; } = 0;

	public int displayScore { get { return Mathf.FloorToInt(score); } }

	private void Awake()
	{
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
	}

	private void Start()
	{
		SetState(GameState.Init);
	}

	public bool IsState(GameState s)
	{
		return state == s;
	}

	public bool IsEndState()
	{
		return state == GameState.Over || state == GameState.LevelComplete;
	}

	public bool IsGameMode(GameMode m)
	{
		return mode == m;
	}

	public void SetState(GameState s)
	{
        state = s;

		if (s == GameState.Init)
		{
			OnGameInit?.Invoke();
			if (mode == GameMode.Level && LevelManager.isPracticeMode) player.transform.position = LevelManager.practiceSpawnPosition;
		}

		else if (s == GameState.Play) OnGamePlay?.Invoke();
		else if (s == GameState.Over)
		{
			if (mode == GameMode.Endless)
			{
				score = maxPlayerHeight * 10f;

				if (score > SaveManager.save.highScore)
				{
					SaveManager.SetSaveHighScore(score, false);
				}

				SaveManager.WriteSaveToSave();

				_ = LeaderboardManager.Instance.AddScore(SaveManager.save.highScore);
			}

			OnGameEnd?.Invoke();
			OnGameOver?.Invoke();
		}
		else if (s == GameState.LevelComplete)
		{
			OnGameEnd?.Invoke();
			OnLevelComplete?.Invoke();

			// if level is in practice mode, don't save
			if (!LevelManager.isPracticeMode)
			{
				SaveData.LevelCompleteData completeData = new(LevelsManager.GetCurrentLevelKey(), LevelsManager.GetCurrentLevelIndex());
				SaveManager.UpdateLevelCompleted(completeData);
			}
		}
	}

	private void Update()
	{
		SaveManager.AddDeltaTime();
		//if (state == GameState.INIT) InitUpdate();
		//if (state == GameState.PLAY) PlayUpdate();
		//if (state == GameState.OVER) OverUpdate();
	}

	private void FixedUpdate()
	{
		if (IsState(GameState.Init)) InitFixedUpdate();
	}

	private void InitFixedUpdate()
	{
		if (ControlManager.WasShootPressedThisFrame() && !CameraManager.Instance.isPanning)
		{
			SetState(GameState.Play);
		}
	}

	private float m_maxPlayerHeight = 0;
	public float maxPlayerHeight
	{
		get {
			if (player != null/*&& IsState(GameState.PLAY)*/) m_maxPlayerHeight = Mathf.Max(player.transform.position.y, m_maxPlayerHeight);
			return m_maxPlayerHeight;
		}
	}
}

public enum GameState
{
    Init, // reset/prep
    Play, // playing
    Over, // game over
	LevelComplete // only for levels
}

// loads corresponding game elements/scene
public enum GameMode
{
	Endless,
	Level
}