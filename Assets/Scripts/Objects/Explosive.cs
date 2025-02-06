using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Explosive : MonoBehaviour, IShootable
{
	public float range = 5;
	public float strength = 1;

	public bool replenishAmmo => true;

	// to stop getting shot multiple times in the same frame
	bool shot = false;

	private SpriteRenderer sr;

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	public void OnShot()
	{
		if (GameHandler.Instance.player == null || shot) return;
		shot = true;

		float distance = Vector2.Distance(transform.position, GameHandler.Instance.player.transform.position);
		if (distance < range)
		{
			Vector2 direction = ((Vector2)GameHandler.Instance.player.transform.position - (Vector2)transform.position).normalized;

			GameHandler.Instance.player.rb.velocity += (range - distance) * strength * direction;
		}

		var p = ParticleManager.CreateParticleSystem("Explosion", transform.position, transform.parent, true);

		// for explosive walls
		if (sr != null)
		{
			ParticleSystem.ShapeModule shape = p.shape;
			shape.sprite = sr.sprite;
			shape.texture = sr.sprite.texture; // will sample texture to decide what colors to set particles to (on top of startColor)
			shape.scale *= sr.size; // account for scaling
		}

		AudioManager.PlaySoundGroup("Explosion");

		if (!DOTween.IsTweening(Camera.main)) Camera.main.DOShakePosition(0.5f, 0.5f, 12, fadeOut: true);
		Destroy(gameObject);
	}
}
