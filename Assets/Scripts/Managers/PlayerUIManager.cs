using Powerups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
	public static PlayerUIManager Instance { get; private set; }

	[SerializeField] private Transform powerupUIMainContainer;

	[SerializeField] private PowerupUI powerupUIMainPrefab, powerupUITimerPrefab;

	[SerializeField] private Transform bulletUIContainer;
	[SerializeField] private Image bulletUIPrefab;

	private List<Image> bulletImages = new();

	private void Awake()
	{
		if (Instance != null && Instance != this) { Destroy(gameObject); return; }
		Instance = this;
	}

	
	private void Start()
	{
		for (int i = 0; i < GameHandler.Instance.player.maxAmmo; i++)
		{
			bulletImages.Add(Instantiate(bulletUIPrefab, bulletUIContainer));
		}
	}
	
	private void Update()
	{
		if (GameHandler.Instance.player == null) return;
		
		for (int i = 0; i < bulletImages.Count; i++)
		{
			bulletImages[i].color = ((bulletImages.Count - 1 - i) < GameHandler.Instance.player.ammo) ? Color.yellow : new Color(0.1f, 0.1f, 0.1f);
		}

		/*
		Vector3 followPosition = GameHandler.Instance.player.transform.position;
		followPosition.z = powerupUITimerContainer.transform.position.z;
		powerupUITimerContainer.transform.position = followPosition;
		*/
	}

	private void DestroyPowerupTimerUI()
	{
		foreach (Transform child in GameHandler.Instance.player.powerupTimerContainer.transform)
		{
			Destroy(child.gameObject);
		}
	}
	
	public void AddPowerupUI(Powerup p)
	{
		CreatePowerupUI(p, powerupUIMainPrefab, powerupUIMainContainer);
		CreatePowerupUI(p, powerupUITimerPrefab, GameHandler.Instance.player.powerupTimerContainer);
	}

	private PowerupUI CreatePowerupUI(Powerup p, PowerupUI prefab, Transform parent)
	{
		if (!p.uiEnabled) return null;
		PowerupUI pUI = Instantiate(prefab, parent);

		pUI.durationSeconds = p.durationSeconds;
		pUI.icon.sprite = p.sprite;
		pUI.timed = p.timed;

		p.AddUI(pUI);

		return pUI;
	}

	private void OnEnable()
	{
		GameHandler.Instance.OnGameEnd += DestroyPowerupTimerUI;
	}

	private void OnDisable()
	{
		GameHandler.Instance.OnGameEnd -= DestroyPowerupTimerUI;
	}
}
