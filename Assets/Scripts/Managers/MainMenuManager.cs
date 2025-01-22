using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public BackgroundScroller background;

    [SerializeField] private float backgroundScrollSpeed = 0.05f;


    [SerializeField] private List<string> levelScenes = new();

    [SerializeField] private Button levelSelectButton, endlessButton, optionsButton;

	private void Awake()
	{
        endlessButton.onClick.AddListener(() => TransitionManager.Instance.LoadScene("EndlessGame"));
	}

	void Update()
    {
        background.offset += backgroundScrollSpeed * Time.deltaTime * Vector2.one;
    }
}
