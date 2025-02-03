using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoal : MonoBehaviour, ICollidable
{
	private SpriteRenderer sr;

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	public void OnCollide(Player player)
	{
		ParticleSystem p = ParticleManager.CreateParticleSystem("Goal", transform.position, transform.parent, true);
		// set sprite/texture for particles to match
		ParticleSystem.MainModule main = p.main;
		main.startColor = sr.color;

		ParticleSystem.ShapeModule shape = p.shape;
		shape.sprite = sr.sprite;
		shape.texture = sr.sprite.texture; // will sample texture to decide what colors to set particles to (on top of startColor)

		ParticleSystemRenderer rend = p.GetComponent<ParticleSystemRenderer>();
		rend.material.SetTexture("_MainTex", sr.sprite.texture);

		AudioManager.PlaySound("Goal");
	}
}
