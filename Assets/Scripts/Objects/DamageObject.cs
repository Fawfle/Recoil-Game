using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DamageObject : MonoBehaviour, IShootable, ICollidable
{
	private SpriteRenderer sr;

    public ParticleSystem particles;
	public bool shootable = false;

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	private void DestroySelf()
	{
		ParticleSystem p = Instantiate(particles, transform.position, Quaternion.identity);
		// set sprite/texture for particles to match
		ParticleSystem.MainModule main = p.main;
		main.startColor = sr.color;

		ParticleSystem.ShapeModule shape = p.shape;
		shape.sprite = sr.sprite;
		shape.texture = sr.sprite.texture;

		p.transform.SetParent(transform.parent);
        //Destroy(p.gameObject, p.main.startLifetime.constantMax);

		Camera.main.DOShakePosition(0.5f, 0.5f, 12, fadeOut: true);
        Destroy(gameObject);
	}

	public void OnCollide(Player player)
	{
		player.TakeDamage();
		if (player != null || shootable) DestroySelf();
	}

    public void OnShot()
	{
		if (!shootable) return;
		DestroySelf();
	}
}
