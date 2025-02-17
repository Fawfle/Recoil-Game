using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public BackgroundScroller background;

    [SerializeField] private float backgroundScrollSpeed = 0.05f;


    [SerializeField] private List<string> levelScenes = new();

    [SerializeField] private GameObject startMenu, levelMenu, optionsMenu, leaderboardMenu, customizationMenu, statsMenu, creditsMenu;

    [SerializeField] private Button levelSelectButton, endlessButton, optionsButton, leaderboardButton, customizationButton, statsButton, creditsButton;
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

    [Header("Customization")]
    [SerializeField] private SkinManager skinManager;
    [SerializeField] private Slider playerColorSlider;
    [SerializeField] private Button equipSkinButton, skinForwardButton, skinBackwardButton;
    [SerializeField] private TextMeshProUGUI skinDescriptionText, equipButtonText;

    [SerializeField] private SpriteRenderer playerBody, playerOutline, playerGun;
    private Color playerBodyColor, playerOutlineColor;

    private int currentSkinIndex = 0; // current skin page, corresponds to skinmanger indexes
    private PlayerSkin currentSkin;
    private bool isSkinUnlocked = true;

    private static readonly float PLAYER_COLOR_SATURATION = 0.67f;
    private static readonly float PLAYER_COLOR_VALUE = 1f;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI statsText;

	private void Awake()
	{
        levelSelectButton.onClick.AddListener(() => SetMenuActive(MenuType.Level));

        endlessButton.onClick.AddListener(() => TransitionManager.Instance.LoadScene("GameEndless"));

        optionsButton.onClick.AddListener(() => SetMenuActive(MenuType.Options));

        leaderboardButton.onClick.AddListener(() => SetMenuActive(MenuType.Leaderboard));

        customizationButton.onClick.AddListener(() => SetMenuActive(MenuType.Customization));

        statsButton.onClick.AddListener(() => SetMenuActive(MenuType.Stats));

        creditsButton.onClick.AddListener(() => SetMenuActive(MenuType.Credits));

        // levels
        pageForwardButton.onClick.AddListener(() => LoadLevelPage(currentLevelPageIndex + 1));
        pageBackwardButton.onClick.AddListener(() => LoadLevelPage(currentLevelPageIndex - 1));

        // options
        volumeSlider.onValueChanged.AddListener(OnVolumeSliderUpdate);
        clickToShootToggle.onValueChanged.AddListener(OnClickToShootToggleUpdate);
        levelIntroPanToggle.onValueChanged.AddListener(OnLevelIntroPanToggleUpdate);

        // customization
        playerColorSlider.onValueChanged.AddListener(OnPlayerColorSliderUpdate);
        skinForwardButton.onClick.AddListener(() => LoadSkinPage(currentSkinIndex >= skinManager.skins.Count - 1 ? 0 : currentSkinIndex + 1));
        skinBackwardButton.onClick.AddListener(() => LoadSkinPage(currentSkinIndex == 0 ? skinManager.skins.Count - 1 : currentSkinIndex - 1));
        equipSkinButton.onClick.AddListener(() => EquipCurrentSkin());
        

        foreach (var b in backButtons)
        {
            b.onClick.AddListener(() => SetMenuActive(MenuType.Start));
        }
	}

	private void Start()
	{
        SetMenuActive(MenuType.Start);

        LoadLevelPage(0);

        PopulateStats();

        // audio manager sets self volume
        volumeSlider.SetValueWithoutNotify(SaveManager.save.volume);
		clickToShootToggle.SetIsOnWithoutNotify(SaveManager.save.clickToShootEnabled);
        levelIntroPanToggle.SetIsOnWithoutNotify(SaveManager.save.levelIntroPanEnabled);

        // set slider to hue value of player
        Color.RGBToHSV(SaveManager.save.playerColor, out var playerHue, out var _, out var _);

		playerColorSlider.SetValueWithoutNotify(playerHue);

		playerBodyColor = playerBody.color;
		playerOutlineColor = playerOutline.color;

		LoadSkinPage(skinManager.GetSkinIndex(SaveManager.save.playerSkin));

		UpdatePlayerColor();
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
        pageBackwardButton.transform.parent.gameObject.SetActive(pageIndex != 0);
        pageForwardButton.transform.parent.gameObject.SetActive(LevelsManager.Levels.Count - 1 >= (pageIndex + 1) * PAGE_LEVEL_SIZE);
    }

    private void ClearLevelPage()
    {
        foreach (Transform child in levelSelectUIContainer)
        {
            Destroy(child.gameObject);
        }
    }

	#endregion

	#region Customization

    private void OnPlayerColorSliderUpdate(float value)
    {
        Color newColor = Color.HSVToRGB(value, PLAYER_COLOR_SATURATION, PLAYER_COLOR_VALUE);

        SaveManager.SetPlayerColor(newColor);

        UpdatePlayerColor();
	}

    private void UpdatePlayerColor()
    {
        playerColorSlider.targetGraphic.color = SaveManager.save.playerColor;

        customizationButton.targetGraphic.color = SaveManager.save.playerColor;

        UpdatePlayerSkinColor();
	}

    public void LoadSkinPage(int index)
    {
        currentSkinIndex = index;

        currentSkin = skinManager.skins[currentSkinIndex];
		isSkinUnlocked = SkinManager.HasSkinUnlocked(currentSkin.skinKey);

        skinDescriptionText.text = currentSkin.description;

        UpdatePlayerSkin();

        UpdateSkinPage();
    }

    private void UpdateSkinPage()
    {
        bool isEquipped = currentSkin.skinKey == SaveManager.save.playerSkin;
        equipSkinButton.interactable = !(isEquipped || !isSkinUnlocked);
        equipButtonText.text = isEquipped ? "Equipped" : "Equip";
    }

    private void EquipCurrentSkin()
    {
        if (!isSkinUnlocked) return;

        SaveManager.SetPlayerSkin(currentSkin.skinKey);

        UpdateSkinPage();
    }

    public void UpdatePlayerSkin()
    {
        playerBody.sprite = currentSkin.bodySprite;
        playerBody.transform.localScale = currentSkin.bodyScale * Vector3.one;

        playerOutline.sprite = currentSkin.bodySprite;
		playerOutline.transform.localScale = currentSkin.outlineScale * Vector3.one;

		playerGun.sprite = currentSkin.gunSprite;
		playerGun.transform.localScale = currentSkin.gunScale * Vector3.one;
        playerGun.size = currentSkin.gunSize;

        // dim locked skins
        playerBody.color = Color.Lerp(playerBodyColor, isSkinUnlocked ? playerBodyColor : Color.black, 0.3f);
        playerOutline.color = Color.Lerp(playerOutlineColor, isSkinUnlocked ? playerOutlineColor : Color.black, 0.3f);

        UpdatePlayerSkinColor();
	}

    public void UpdatePlayerSkinColor()
    {
		playerGun.color = Color.Lerp(SaveManager.save.playerColor, isSkinUnlocked ? SaveManager.save.playerColor : Color.black, 0.3f);
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

	#region Stats

    private void PopulateStats()
    {
        statsText.text = statsText.text.Replace("-score-", Mathf.Round((float)SaveManager.save.highScore).ToString());
        statsText.text = statsText.text.Replace("-deaths-", SaveManager.save.deaths.ToString());
        statsText.text = statsText.text.Replace("-playtime-", Mathf.Round((float)SaveManager.save.playTimeSeconds / 60f).ToString());
        statsText.text = statsText.text.Replace("-shots-", SaveManager.save.shotsFired.ToString());
        statsText.text = statsText.text.Replace("-runs-", SaveManager.save.endlessRuns.ToString());
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

        customizationMenu.SetActive(menu == MenuType.Customization);
        statsMenu.SetActive(menu == MenuType.Stats);
        creditsMenu.SetActive(menu == MenuType.Credits);

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
        Leaderboard,
        Customization,
        Stats,
        Credits
    }
}
