using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DamageObject : MonoBehaviour, IShootable, ICollidable
{
	private SpriteRenderer sr;

	public bool destructable = false;

	public bool refreshAmmo = true;

	public bool replenishAmmo => refreshAmmo;

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	private void DestroySelf()
	{
		ParticleSystem p = ParticleManager.CreateParticleSystem("Destroy", transform.position, transform.parent, true);
		// set sprite/texture for particles to match
		ParticleSystem.MainModule main = p.main;
		main.startColor = sr.color;

		ParticleSystem.ShapeModule shape = p.shape;
		shape.sprite = sr.sprite;
		shape.texture = sr.sprite.texture; // will sample texture to decide what colors to set particles to (on top of startColor)
		shape.scale *= sr.size; // account for scaling

		if (!DOTween.IsTweening(Camera.main)) Camera.main.DOShakePosition(0.5f, 0.5f, 12, fadeOut: true);

		AudioManager.PlaySoundGroup("DestroyDamageObject");

        Destroy(gameObject);
	}

	public void OnCollide(Player player)
	{
		player.TakeDamage();
		if (destructable) DestroySelf();
	}

    public void OnShot()
	{
		if (!destructable) return;
		DestroySelf();
	}
}
