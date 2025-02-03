using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public bool paused = false;

    [SerializeField] private GameObject pausedMenu;
    [SerializeField] private Button resumeButton;
	[SerializeField] private Button menuButton;

	private void Awake()
	{
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

	private void Start()
	{
        resumeButton.onClick.AddListener(() => Pause(false));
        menuButton.onClick.AddListener(() => TransitionManager.Instance.LoadScene("MainMenu"));
	}

	void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && (!GameHandler.Instance.IsEndState() || paused) && !TransitionManager.transitioning)
		{
            Pause(!paused);
		}
    }

    void Pause(bool input)
	{
        paused = input;
        Time.timeScale = paused ? 0 : 1;

        pausedMenu.SetActive(paused);
	}
}
