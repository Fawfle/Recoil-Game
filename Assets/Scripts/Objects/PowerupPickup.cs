using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Powerups;
using TMPro;

[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D))]
public class PowerupPickup : MonoBehaviour, ICollidable
{
	[SerializeField] private ParticleSystem particles;

	private SpriteRenderer sr;
	[SerializeField] private SpriteRenderer powerupSr;

	private Powerup powerup;

	[SerializeField] private TextMeshPro floatingTextPrefab;

	//[SerializeField] private bool randomPowerup = true;
	//[Tooltip("if no random powerup, use this one instead")]
	[SerializeField] private PowerupManager.PowerupType powerupType;

	public void Start()
	{
		if (powerupType == PowerupManager.PowerupType.Random) powerup = GetRandomPowerup();
		else powerup = PowerupManager.Instance.GetPowerupOfType(powerupType);

		sr = GetComponent<SpriteRenderer>();
		powerupSr.sprite = powerup.sprite;
		powerupSr.transform.localScale = Vector3.one * powerup.spriteScale;
		powerupSr.transform.localPosition = powerup.spriteOffset;
		powerupSr.transform.rotation = Quaternion.Euler(0, 0, powerup.spriteRotation);
	}

	public void OnCollide(Player player)
	{
		player.AddPowerup(powerup);

		ParticleSystem p = (ParticleManager.CreateParticleSystem("Powerup", transform.position, transform.parent, true));

		ParticleSystem.MainModule main = p.main;
		main.startColor = sr.color;

		var text = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
		text.text = powerup.name;

		AudioManager.PlaySoundGroup("PickupPowerup", 0.3f);

		Destroy(gameObject);
	}

	private Powerup GetRandomPowerup()
	{
		Powerup p = PowerupManager.Instance.GetRandomPowerup();

		// don't give player shield if they already have one
		if (p is ShieldPowerup && GameHandler.Instance.player.health > 1f)
		{
			return GetRandomPowerup();
		}

		return p;
	}
}
