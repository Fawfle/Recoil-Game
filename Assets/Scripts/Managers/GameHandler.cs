using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance { get; private set; }

	public Player player;
	public BackgroundScroller background;

    public GameState state;

    public Action OnGameInit, OnGamePlay, OnGameOver;

	public int score { get; private set; } = 0;

	private void Awake()
	{
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
	}

	private void Start()
	{
		SetState(GameState.INIT);
	}

	public bool IsState(GameState s)
	{
		return state == s;
	}

	public void SetState(GameState s)
	{
        state = s;

		if (s == GameState.INIT) OnGameInit?.Invoke();
		else if (s == GameState.PLAY) OnGamePlay?.Invoke();
		else if (s == GameState.OVER)
		{
			score = (int)Mathf.Floor(maxPlayerHeight * 10f);

			if (score > SaveManager.save.highScore)
			{
				SaveManager.WriteHighScoreToSave(score);
			}

			OnGameOver?.Invoke();
		}
	}

	private void Update()
	{
		//if (state == GameState.INIT) InitUpdate();
		//if (state == GameState.PLAY) PlayUpdate();
		//if (state == GameState.OVER) OverUpdate();
	}

	private void FixedUpdate()
	{
		if (IsState(GameState.INIT)) InitFixedUpdate();
	}

	private void InitFixedUpdate()
	{
		if (ControlManager.Controls.game.shoot.WasPressedThisFrame())
		{
			SetState(GameState.PLAY);
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
    INIT, // reset/prep
    PLAY, // playing
    OVER // game oever
}

// loads corresponding game elements/scene
public enum GameMode
{
	Endless,
	Level
}