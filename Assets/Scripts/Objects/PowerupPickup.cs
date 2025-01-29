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

	public void Start()
	{
		powerup = PowerupManager.Instance.GetRandomPowerup();

		sr = GetComponent<SpriteRenderer>();
		powerupSr.sprite = powerup.sprite;
		powerupSr.transform.localScale = Vector3.one * powerup.spriteScale;
		powerupSr.transform.localPosition = powerup.spriteoffset;
	}

	public void OnCollide(Player player)
	{
		player.AddPowerup(powerup);

		ParticleSystem p = ParticleManager.DestroyAfterDuration(ParticleManager.CreateParticleSystem("Powerup", transform.position, transform.parent));

		ParticleSystem.MainModule main = p.main;
		main.startColor = sr.color;

		var text = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
		text.text = powerup.name;

		AudioManager.PlaySoundGroup("PickupPowerup", 0.3f);

		Destroy(gameObject);
	}
}
