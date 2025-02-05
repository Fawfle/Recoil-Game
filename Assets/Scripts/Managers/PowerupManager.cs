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

	private static readonly Dictionary<PowerupType, Powerup> POWERUP_TYPES = new() {
		{ PowerupType.Recoil, new RecoilPowerup() },
		{ PowerupType.Shield, new ShieldPowerup() },
		{ PowerupType.InfiniteAmmo, new InfiniteAmmoPowerup() },
		{ PowerupType.Shotgun, new ShotgunPowerup() },
	};

	private void Awake()
	{
		if (Instance != null && Instance != this) { Destroy(gameObject); return; }
		Instance = this;

		powerups = new();
		foreach (PowerupData pData in powerupData)
		{
			Powerup p = POWERUP_TYPES[pData.type].CloneViaFakeSerialization();

			p.name = pData.name;
			p.durationSeconds = pData.duration;
			p.sprite = pData.sprite;
			p.spriteOffset = pData.spriteOffset;
			p.spriteScale = pData.spriteScale;
			p.spriteRotation = pData.spriteRotation;

			powerups.Add(p);
		}
	}

	public Powerup GetRandomPowerup()
	{
		return powerups[Random.Range(0, powerups.Count)].CloneViaFakeSerialization<Powerup>();
	}

	public Powerup GetPowerupOfType(PowerupType type)
	{
		foreach (var p in powerups)
		{
			if (p.GetType() == POWERUP_TYPES[type].GetType()) return p.CloneViaSerialization<Powerup>();
		}

		return null;
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
		public float spriteRotation = 0f;
	}


	public enum PowerupType
	{
		Random, // for non random powerups
		Recoil,
		Shield,
		InfiniteAmmo,
		Shotgun
	}
}