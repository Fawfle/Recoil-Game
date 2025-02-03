using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenuManager : MonoBehaviour
{
	public GameObject gameOverMenu, levelCompleteMenu;

	public Button nextLevelButton, resetButton;
		
	public List<Button> mainMenuButtons;

	private Menu menu = Menu.None;

	public TextMeshProUGUI levelText;
	private static readonly float TEXT_LINGER_TIME_SECONDS = 2f;
	private static readonly float TEXT_FADE_TIME = 1f;

	private void Start()
	{
		resetButton.onClick.AddListener(() => { TransitionManager.Instance.ReloadScene(); });
		
		foreach (var b in mainMenuButtons)
		{
			b.onClick.AddListener(() => { TransitionManager.Instance.LoadScene("MainMenu"); });
		}

		string nextLevel = LevelsManager.GetNextLevel();
		if (nextLevel != null) nextLevelButton.onClick.AddListener(() => { TransitionManager.Instance.LoadScene(nextLevel); });
		else nextLevelButton.interactable = false;

		SetMenuActive(Menu.None);

		StartCoroutine(IntroRoutine());
	}

	// show some intro text and stuff
	private IEnumerator IntroRoutine()
	{
		levelText.enabled = true;

		var c = levelText.color;
		c.a = 1f;
		levelText.color = c;

		yield return new WaitForSeconds(TEXT_LINGER_TIME_SECONDS);

		levelText.CrossFadeAlpha(0f, TEXT_FADE_TIME, false);

		yield return new WaitForSeconds(TEXT_FADE_TIME);

		levelText.enabled = false;
	}

	private void SetMenuActive(Menu m)
	{
		menu = m;

		gameOverMenu.SetActive(menu == Menu.GameOver);
		levelCompleteMenu.SetActive(menu == Menu.Complete);
	}

	private void OnGameOver()
	{
		SetMenuActive(Menu.GameOver);
		gameOverMenu.GetComponent<Animator>().Play("Slide In");
	}

	private void OnLevelComplete()
	{
		SetMenuActive(Menu.Complete);
		levelCompleteMenu.GetComponent<Animator>().Play("Slide In");
	}

	enum Menu
	{
		None,
		GameOver,
		Complete
	}

	private void OnEnable()
	{
		//GameHandler.Instance.OnGameInit += OnGameInit;
		//GameHandler.Instance.OnGamePlay += OnGamePlay;
		GameHandler.Instance.OnGameOver += OnGameOver;
		GameHandler.Instance.onLevelComplete += OnLevelComplete;
	}

	private void OnDisable()
	{
		//GameHandler.Instance.OnGameInit -= OnGameInit;
		//GameHandler.Instance.OnGamePlay -= OnGamePlay;
		GameHandler.Instance.OnGameEnd -= OnGameOver;
		GameHandler.Instance.onLevelComplete -= OnLevelComplete;
	}
}
