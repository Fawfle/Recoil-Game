using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndlessLevelMenuManager : MonoBehaviour
{
	[Header("Game Active")]
	public TextMeshProUGUI scoreHeightText;


	[Header("Game Over")]
	public GameObject gameOverMenu;
    public Button retryButton, menuButton;

	public TextMeshProUGUI scoreText, highScoreText;

    void Awake()
    {
        retryButton.onClick.AddListener(() => TransitionManager.Instance.ReloadScene());
		menuButton.onClick.AddListener(() => TransitionManager.Instance.LoadScene("MainMenu"));
	}

	private void Update()
	{
		if (GameHandler.Instance.IsState(GameState.OVER)) return;

		scoreHeightText.text = Mathf.Floor(GameHandler.Instance.maxPlayerHeight * 10f).ToString();
	}


	private void OnGameInit()
	{
		gameOverMenu.SetActive(false);
	}

	private void OnGameOver()
	{
		gameOverMenu.SetActive(true);
		gameOverMenu.GetComponent<Animator>().Play("Slide In");

		scoreHeightText.text = GameHandler.Instance.score.ToString();

		scoreText.text = GameHandler.Instance.score.ToString();
		highScoreText.text = SaveManager.save.highScore.ToString();
	}

	private void OnEnable()
	{
		GameHandler.Instance.OnGameInit += OnGameInit;
		GameHandler.Instance.OnGameOver += OnGameOver;
	}

	private void OnDisable()
	{
		GameHandler.Instance.OnGameInit -= OnGameInit;
		GameHandler.Instance.OnGameOver -= OnGameOver;
	}
}
