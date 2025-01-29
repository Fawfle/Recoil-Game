using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public BackgroundScroller background;

    [SerializeField] private float backgroundScrollSpeed = 0.05f;


    [SerializeField] private List<string> levelScenes = new();

    [SerializeField] private GameObject startMenu, levelMenu, optionsMenu, leaderboardMenu;

    [SerializeField] private Button levelSelectButton, endlessButton, optionsButton, leaderboardButton;
    [SerializeField] private List<Button> backButtons = new();

    [Header("Levels")]
    [SerializeField] private LevelSelectUI levelSelectUIPrefab;
	[SerializeField] private Transform levelSelectUIContainer;

    private static readonly int PAGE_LEVELS = 24;

	[Header("Options")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle clickToShootToggle;

	private void Awake()
	{
        levelSelectButton.onClick.AddListener(() => SetMenuActive(MenuType.Level));

        endlessButton.onClick.AddListener(() => TransitionManager.Instance.LoadScene("GameEndless"));

        optionsButton.onClick.AddListener(() => SetMenuActive(MenuType.Options));

        leaderboardButton.onClick.AddListener(() => SetMenuActive(MenuType.Leaderboard));

        volumeSlider.onValueChanged.AddListener(OnVolumeSliderUpdate);
        clickToShootToggle.onValueChanged.AddListener(OnClickToShootToggleUpdate);

        foreach (var b in backButtons)
        {
            b.onClick.AddListener(() => SetMenuActive(MenuType.Start));
        }
	}

	private void Start()
	{
        SetMenuActive(MenuType.Start);

        LoadLevelPage();

        // audio manager sets self volume
        volumeSlider.SetValueWithoutNotify(SaveManager.save.volume);
		clickToShootToggle.SetIsOnWithoutNotify(SaveManager.save.clickToShootEnabled);
	}

	#region Levels

    private void LoadLevelPage()
    {
        ClearLevelPage();

        for (int i = 0; i < PAGE_LEVELS; i++)
        {
            int levelIndex = i;
            if (levelIndex > LevelsManager.levels.Length) break;

            var ui = Instantiate(levelSelectUIPrefab, levelSelectUIContainer);

            ui.UpdateLevel(levelIndex);
        }
    }

    private void ClearLevelPage()
    {
        foreach (Transform child in levelSelectUIContainer)
        {
            Destroy(child.gameObject);
        }
    }

	#endregion

	#region Options

	private void OnVolumeSliderUpdate(float value)
    {
        SaveManager.WriteVolumeToSave(value);

        AudioManager.SetAudioMixerVolume(value);
    }

    private void OnClickToShootToggleUpdate(bool isOn)
    {
        SaveManager.WriteClickToShootToSave(isOn);
    }

    #endregion

    void Update()
    {
        background.offset += backgroundScrollSpeed * Time.deltaTime * Vector2.one;
    }

    private void SetMenuActive(MenuType menu)
    {
        if (TransitionManager.transitioning) return;

        startMenu.SetActive(menu == MenuType.Start);
        levelMenu.SetActive(menu == MenuType.Level);
        optionsMenu.SetActive(menu == MenuType.Options);
        leaderboardMenu.SetActive(menu == MenuType.Leaderboard);

        if (menu == MenuType.Leaderboard)
        {
            LeaderboardManager.Instance.SetLeaderboardMenu(LeaderboardManager.Menu.Top);
        }
    }

    enum MenuType
    {
        Start,
        Level,
        Options,
        Leaderboard
    }
}
