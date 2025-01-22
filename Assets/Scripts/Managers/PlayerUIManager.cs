using Powerups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
	public static PlayerUIManager Instance { get; private set; }

	[SerializeField] private Transform activePowerupUIContainer;
	[SerializeField] private ActivePowerupUI activePowerupUIPrefab;
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
			bulletImages[i].color = (i < GameHandler.Instance.player.ammo) ? Color.yellow : new Color(0.1f, 0.1f, 0.1f);
		}
	}

	public ActivePowerupUI CreateActivePowerupUI(Powerup p)
	{
		if (!p.timed) return null;
		ActivePowerupUI pUI = Instantiate(activePowerupUIPrefab, activePowerupUIContainer);

		pUI.durationSeconds = p.durationSeconds;
		pUI.icon.sprite = p.sprite;

		return pUI;
	}
}
