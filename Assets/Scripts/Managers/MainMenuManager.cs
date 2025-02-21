using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    [SerializeField] private Toggle leaderboardCrownToggle;
	[SerializeField] private Graphic[] leaderboardCrownToggleGraphics;
	[SerializeField] private Graphic[] leaderboardCrownToggleCheckbox;

	[SerializeField] private SpriteRenderer playerBody, playerOutline, playerGun, playerCrown;
    private Color playerBodyColor, playerOutlineColor, skinDescriptionColor, equipButtonColor;

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
        leaderboardCrownToggle.onValueChanged.AddListener(OnLeaderboardCrownToggleUpdate);

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
        leaderboardCrownToggle.SetIsOnWithoutNotify(SaveManager.save.leaderboardCrownEnabled);

        // set slider to hue value of player
        Color.RGBToHSV(SaveManager.save.playerColor, out var playerHue, out var _, out var _);

		playerColorSlider.SetValueWithoutNotify(playerHue);

        // save colors so I can grey them out later
		playerBodyColor = playerBody.color;
		playerOutlineColor = playerOutline.color;
        skinDescriptionColor = skinDescriptionText.color;
        equipButtonColor = equipButtonText.color;

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

	private void OnLeaderboardCrownToggleUpdate(bool isOn)
	{
		SaveManager.SetLeaderboardCrownEnabled(isOn);

        UpdatePlayerCrown();
	}


	// configure customization menu by disabling podium crown toggle if the player isn't in the top 3
	private void OnCustomizationLoad()
	{
        //Graphic[] graphics = leaderboardCrownToggle.transform.parent.GetComponentsInChildren<Graphic>();
        //print(graphics.Length);

        bool isTop3 = SaveManager.IsOnLeaderboardPodium();
		leaderboardCrownToggle.enabled = isTop3;

		foreach (Graphic g in leaderboardCrownToggleGraphics)
		{
			g.CrossFadeAlpha(isTop3 ? 1f : 0.3f, 0f, true);
		}

        if (leaderboardCrownToggle.isOn)
        {
			foreach (Graphic g in leaderboardCrownToggleCheckbox)
			{
				g.CrossFadeAlpha(isTop3 ? 1f : 0.3f, 0f, true);
			}
		}

        UpdatePlayerCrown();
	}

    private void UpdatePlayerCrown()
    {
		if (SaveManager.IsOnLeaderboardPodium() && SaveManager.save.leaderboardCrownEnabled)
		{
			playerCrown.gameObject.SetActive(true);
			Color crownColor = SaveManager.save.currentLeaderboardRank == 0 ? LeaderboardManager.FIRST_COLOR : SaveManager.save.currentLeaderboardRank == 1 ? LeaderboardManager.SECOND_COLOR : LeaderboardManager.THIRD_COLOR;
            crownColor.a = 0.8f;
            playerCrown.color = crownColor;
		}
		else
		{
			playerCrown.gameObject.SetActive(false);
		}
	}

	private void UpdatePlayerColor()
    {
        playerColorSlider.targetGraphic.color = SaveManager.save.playerColor;

        customizationButton.targetGraphic.color = SaveManager.save.playerColor;

        UpdatePlayerPreviewColor();
	}

    public void LoadSkinPage(int index)
    {
        currentSkinIndex = index;

        currentSkin = skinManager.skins[currentSkinIndex];
		isSkinUnlocked = SkinManager.HasSkinUnlocked(currentSkin.skinKey);

        skinDescriptionText.text = currentSkin.description;
		skinDescriptionText.color = DimIfLocked(skinDescriptionColor);

		UpdatePlayerSkin();

        UpdateSkinPage();
    }

    private void UpdateSkinPage()
    {
        bool isEquipped = currentSkin.skinKey == SaveManager.save.playerSkin;
        equipSkinButton.interactable = !(isEquipped || !isSkinUnlocked);
        equipButtonText.text = isEquipped ? "Equipped" : isSkinUnlocked ? "Equip" : "Locked";

        equipButtonText.color = DimIfLocked(equipButtonColor, 1f);
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
        playerBody.color = DimIfLocked(playerBodyColor);
        playerOutline.color = DimIfLocked(playerOutlineColor);

		// more complex drawing for offseted outlines
		foreach (Transform child in playerOutline.transform)
		{
			Destroy(child.gameObject);
		}

        // create new sprites under outline with offsets
		if (currentSkin.outlineOffset != 0f)
		{
			Vector2[] offsets = { new(currentSkin.outlineOffset, 0), new(0, currentSkin.outlineOffset), new(currentSkin.outlineOffset, currentSkin.outlineOffset), new(-currentSkin.outlineOffset, currentSkin.outlineOffset) };

			for (int i = 0; i < offsets.Length; i++)
			{
                for (int j = 0; j < 2; j++)
                {
                    GameObject g = new GameObject("Outline " + i);
                    var sr = g.AddComponent<SpriteRenderer>();

                    g.transform.SetParent(playerOutline.transform);
                    g.transform.localScale = Vector3.one;

                    g.transform.localPosition = offsets[i] * (j == 0 ? 1 : -1);

                    sr.color = playerOutline.color;
                    sr.sprite = playerOutline.sprite;
                }
			}
		}

		UpdatePlayerPreviewColor();
	}

    public void UpdatePlayerPreviewColor()
    {
        playerGun.color = DimIfLocked(SaveManager.save.playerColor);
	}

    // returns dimmed color if skin isn't unlocked
    private Color DimIfLocked(Color c, float t = 0.3f)
    {
        if (isSkinUnlocked) return c;

        return Color.Lerp(c, Color.black, t);
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
		statsText.text = statsText.text.Replace("-rank-", SaveManager.save.currentLeaderboardRank != -1 ? (SaveManager.save.currentLeaderboardRank + 1).ToString() : "none");
		statsText.text = statsText.text.Replace("-highrank-", SaveManager.save.highestLeaderboardRank != -1 ? (SaveManager.save.highestLeaderboardRank + 1).ToString() : "none");
		statsText.text = statsText.text.Replace("-deaths-", SaveManager.save.deaths.ToString());
        statsText.text = statsText.text.Replace("-playtime-", Mathf.Round((float)SaveManager.save.playTimeSeconds / 60f).ToString());
        statsText.text = statsText.text.Replace("-shots-", SaveManager.save.shotsFired.ToString());
        statsText.text = statsText.text.Replace("-runs-", SaveManager.save.endlessRuns.ToString());

        int collectedSkins = 0;

        foreach (SkinKey key in Enum.GetValues(typeof(SkinKey)))
        {
            if (SkinManager.HasSkinUnlocked(key)) collectedSkins++;
        }

		statsText.text = statsText.text.Replace("-skins-", collectedSkins.ToString());
		statsText.text = statsText.text.Replace("-maxskins-", skinManager.skins.Count.ToString());
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
        else if (menu == MenuType.Customization) OnCustomizationLoad();
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
