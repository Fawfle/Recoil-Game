using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DamageObject : MonoBehaviour, IShootable, ICollidable
{
	private SpriteRenderer sr;

	public bool shootable = false;

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	private void DestroySelf()
	{
		ParticleSystem p = ParticleManager.CreateParticleSystem("Destroy", transform.position, transform.parent);
		// set sprite/texture for particles to match
		ParticleSystem.MainModule main = p.main;
		main.startColor = sr.color;

		ParticleSystem.ShapeModule shape = p.shape;
		shape.sprite = sr.sprite;
		shape.texture = sr.sprite.texture;

		if (!DOTween.IsTweening(Camera.main)) Camera.main.DOShakePosition(0.5f, 0.5f, 12, fadeOut: true);

		AudioManager.PlaySoundGroup("DestroyDamageObject");

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
