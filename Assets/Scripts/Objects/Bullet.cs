using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
	public int refillOnShot = 3;

	private Rigidbody2D rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		HandleCollision(collision.gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		HandleCollision(collision.gameObject);
	}

	private void HandleCollision(GameObject g)
	{
		MonoBehaviour[] scripts = g.GetComponents<MonoBehaviour>();

		foreach (MonoBehaviour script in scripts)
		{
			if (script is IShootable)
			{
				(script as IShootable).OnShot();
				if (GameHandler.Instance.player.ammoEnabled) GameHandler.Instance.player.AddAmmo(refillOnShot);

				ParticleSystem p = ParticleManager.DestroyAfterDuration(ParticleManager.CreateParticleSystem("Bullet", transform.position, transform.parent));

				var velModule = p.velocityOverLifetime;

				velModule.xMultiplier = rb.velocity.x;
				velModule.yMultiplier = rb.velocity.y;	

				Destroy(gameObject);
			}
		}
	}
}
