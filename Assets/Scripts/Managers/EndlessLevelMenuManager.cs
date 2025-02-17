using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class EndlessLevelMenuManager : MonoBehaviour
{
	[Header("Game Start")]
	public TextMeshProUGUI introText;

	[Header("Game Active")]
	public TextMeshProUGUI scoreHeightText;


	[Header("Game Over")]
	public GameObject gameOverMenu;
	public GameObject leaderboardMenu;
    public Button retryButton, menuButton, leaderboardButton, leaderboardBackButton;

	public TextMeshProUGUI scoreText, highScoreText;

	private Menu menu;

    void Awake()
    {
        retryButton.onClick.AddListener(() => TransitionManager.Instance.ReloadScene());
		menuButton.onClick.AddListener(() => TransitionManager.Instance.LoadScene("MainMenu"));

		leaderboardButton.onClick.AddListener(() => SetMenu(Menu.Leaderboard));
		leaderboardBackButton.onClick.AddListener(() => SetMenu(Menu.GameOver));
	}

	private void Update()
	{
		// r to reset
		if (GameHandler.Instance.IsState(GameState.Over) && ControlManager.Controls.game.restart.WasPressedThisFrame()) TransitionManager.Instance.ReloadScene();

		if (GameHandler.Instance.IsEndState()) return;

		scoreHeightText.text = Mathf.Floor(GameHandler.Instance.maxPlayerHeight * 10f).ToString();
	}

	private void SetMenu(Menu m)
	{
		menu = m;

		gameOverMenu.SetActive(menu == Menu.GameOver);
		leaderboardMenu.SetActive(menu == Menu.Leaderboard);

		if (menu == Menu.Leaderboard)
		{
			LeaderboardManager.Instance.SetLeaderboardMenu(LeaderboardManager.Menu.Local);
		}
	}

	private void OnGameInit()
	{
		SetMenu(Menu.None);

		introText.gameObject.SetActive(true);
	}

	private void OnGamePlay()
	{
		introText.gameObject.SetActive(false);
	}

	private void OnGameOver()
	{
		SetMenu(Menu.GameOver);
		gameOverMenu.GetComponent<Animator>().Play("Slide In");

		scoreHeightText.text = GameHandler.Instance.displayScore.ToString();
		scoreHeightText.CrossFadeAlpha(0f, 2f, false);

		scoreText.text = GameHandler.Instance.displayScore.ToString();

		highScoreText.text = Math.Floor(SaveManager.save.highScore).ToString();
	}

	private void OnEnable()
	{
		GameHandler.Instance.OnGameInit += OnGameInit;
		GameHandler.Instance.OnGamePlay += OnGamePlay;
		GameHandler.Instance.OnGameOver += OnGameOver;
	}

	private void OnDisable()
	{
		GameHandler.Instance.OnGameInit -= OnGameInit;
		GameHandler.Instance.OnGamePlay -= OnGamePlay;
		GameHandler.Instance.OnGameOver -= OnGameOver;
	}

	enum Menu
	{
		None,
		GameOver,
		Leaderboard
	}
}
