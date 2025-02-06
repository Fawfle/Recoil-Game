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
    [SerializeField] private Button pageForwardButton, pageBackwardButton;
    private int currentLevelPageIndex = 0;

    private static readonly int PAGE_LEVEL_SIZE = 24;

	[Header("Options")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle clickToShootToggle, levelIntroPanToggle;

	private void Awake()
	{
        levelSelectButton.onClick.AddListener(() => SetMenuActive(MenuType.Level));

        endlessButton.onClick.AddListener(() => TransitionManager.Instance.LoadScene("GameEndless"));

        optionsButton.onClick.AddListener(() => SetMenuActive(MenuType.Options));

        leaderboardButton.onClick.AddListener(() => SetMenuActive(MenuType.Leaderboard));

        // levels
        pageForwardButton.onClick.AddListener(() => LoadLevelPage(currentLevelPageIndex + 1));
        pageBackwardButton.onClick.AddListener(() => LoadLevelPage(currentLevelPageIndex - 1));

        // options
        volumeSlider.onValueChanged.AddListener(OnVolumeSliderUpdate);
        clickToShootToggle.onValueChanged.AddListener(OnClickToShootToggleUpdate);
        levelIntroPanToggle.onValueChanged.AddListener(OnLevelIntroPanToggleUpdate);

        foreach (var b in backButtons)
        {
            b.onClick.AddListener(() => SetMenuActive(MenuType.Start));
        }
	}

	private void Start()
	{
        SetMenuActive(MenuType.Start);

        LoadLevelPage(0);

        // audio manager sets self volume
        volumeSlider.SetValueWithoutNotify(SaveManager.save.volume);
		clickToShootToggle.SetIsOnWithoutNotify(SaveManager.save.clickToShootEnabled);
        levelIntroPanToggle.SetIsOnWithoutNotify(SaveManager.save.levelIntroPanEnabled);
	}

	#region Levels

    private void LoadLevelPage(int pageIndex)
    {
        ClearLevelPage();

        int startIndex = pageIndex * PAGE_LEVEL_SIZE;
        currentLevelPageIndex = pageIndex;

        for (int levelIndex = startIndex; levelIndex < startIndex + PAGE_LEVEL_SIZE; levelIndex++)
        {
            if (levelIndex >= LevelsManager.Levels.Count) break;

            var ui = Instantiate(levelSelectUIPrefab, levelSelectUIContainer);

            ui.UpdateLevel(levelIndex);
        }

        // update page buttons
        pageBackwardButton.interactable = pageIndex != 0;
        pageForwardButton.interactable = LevelsManager.Levels.Count >= (pageIndex + 1) * PAGE_LEVEL_SIZE;
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
        SaveManager.SetSaveVolume(value);

        AudioManager.SetAudioMixerVolume(value);
    }

    private void OnClickToShootToggleUpdate(bool isOn)
    {
        SaveManager.SetSaveClickToShoot(isOn);
    }

    private void OnLevelIntroPanToggleUpdate(bool isOn)
    {
        SaveManager.SetSaveLevelIntroPan(isOn);
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
