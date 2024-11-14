using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
	[Header("Game Active")]
	public TextMeshProUGUI scoreHeightText;


	[Header("Game Over")]
	public GameObject gameOverMenu;
    public Button retry;

    void Awake()
    {
        retry.onClick.AddListener(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().name); });
    }

	private void Update()
	{
		scoreHeightText.text = Mathf.Round(GameHandler.Instance.maxPlayerHeight * 10f).ToString();
	}


	private void OnGameInit()
	{
		gameOverMenu.SetActive(false);
	}

	private void OnGameOver()
	{
		gameOverMenu.SetActive(true);
		gameOverMenu.GetComponent<Animator>().Play("Slide In");
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
