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

		ParticleManager.CreateParticleSystem("Explosion", transform.position, transform.parent, true);

		AudioManager.PlaySoundGroup("Explosion");

		if (!DOTween.IsTweening(Camera.main)) Camera.main.DOShakePosition(0.5f, 0.5f, 12, fadeOut: true);
		Destroy(gameObject);
	}
}
