using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Explosive : MonoBehaviour, IShootable
{
	public float range = 3;
	public float strength = 1;

	public void OnShot()
	{
		if (GameHandler.Instance.player == null) return;

		float distance = Vector2.Distance(transform.position, GameHandler.Instance.player.transform.position);
		if (distance < range)
		{
			Vector2 direction = (GameHandler.Instance.player.transform.position - transform.position).normalized;
			GameHandler.Instance.player.GetComponent<Rigidbody2D>().velocity += (range - distance) * strength * direction;
		}

		ParticleManager.DestroyAfterDuration(ParticleManager.CreateParticleSystem("Explosion", transform.position, transform.parent));

		if (!DOTween.IsTweening(Camera.main)) Camera.main.DOShakePosition(0.5f, 0.5f, 12, fadeOut: true);
		Destroy(gameObject);
	}
}
