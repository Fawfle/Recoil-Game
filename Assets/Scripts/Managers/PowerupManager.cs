using Powerups;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
	public static PowerupManager Instance { get; private set; }

	// this genuinely sucks, but it works and I don't know how to do it better
	[SerializeField] private List<PowerupData> powerupData = new();
	public List<Powerup> powerups;

	private void Awake()
	{
		if (Instance != null && Instance != this) { Destroy(gameObject); return; }
		Instance = this;

		powerups = new();
		foreach (PowerupData pData in powerupData)
		{
			Powerup p = null;
			switch (pData.type)
			{
				case PowerupType.Recoil:
					p = new RecoilPowerup();
					break;
				case PowerupType.Shield:
					p = new ShieldPowerup();
					break;
				case PowerupType.InfiniteAmmo:
					p = new InfiniteAmmoPowerup();
					break;
			}
			p.name = pData.name;
			p.durationSeconds = pData.duration;
			p.sprite = pData.sprite;
			p.spriteoffset = pData.spriteOffset;
			p.spriteScale = pData.spriteScale;

			powerups.Add(p);
		}
	}

	public Powerup GetRandomPowerup()
	{
		return powerups[Random.Range(0, powerups.Count)];
	}

	[System.Serializable]
	private class PowerupData
	{
		public string name;
		public float duration;
		public PowerupType type;
		public Sprite sprite;
		public Vector2 spriteOffset = Vector2.zero;
		public float spriteScale = 1f;
	}


	enum PowerupType
	{
		Recoil,
		Shield,
		InfiniteAmmo
	}
}