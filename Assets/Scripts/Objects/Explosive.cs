using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Explosive : MonoBehaviour, IShootable
{
	public float range = 3;
	public float strength = 1;
	public ParticleSystem particles;

	public void OnShot()
	{
		if (GameHandler.Instance.player == null) return;

		float distance = Vector2.Distance(transform.position, GameHandler.Instance.player.transform.position);
		if (distance < range)
		{
			Vector2 direction = (GameHandler.Instance.player.transform.position - transform.position).normalized;
			GameHandler.Instance.player.GetComponent<Rigidbody2D>().velocity += (range - distance) * strength * direction;
		}

		ParticleSystem p = Instantiate(particles, transform.position, Quaternion.identity);
		p.transform.SetParent(transform.parent);
		Destroy(p.gameObject, p.main.startLifetime.constantMax);
		Camera.main.DOShakePosition(0.5f, 0.5f, 12, fadeOut: true);
		Destroy(gameObject);
	}
}
