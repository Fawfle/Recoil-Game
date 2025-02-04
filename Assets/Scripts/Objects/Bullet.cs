using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
	public int refillOnShot = 3;

	private Rigidbody2D rb;

	[SerializeField] public bool wrap = false;

	public readonly float PADDING = 0.5f;

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

	private void FixedUpdate()
	{
		if (GameHandler.Instance.IsGameMode(GameMode.Endless)) EndlessFixedUpdate();
		else if (GameHandler.Instance.IsGameMode(GameMode.Level)) LevelFixedUpdate();
	}

	private void EndlessFixedUpdate()
	{
		float levelBounds = EndlessLevelManager.LEVEL_BOUNDS + PADDING;

		// destroy/wrap on horizontal bounds
		if (transform.position.x < -levelBounds)
		{
			if (wrap)
			{
				transform.position = new Vector3(levelBounds - 0.01f, transform.position.y, transform.position.z);
				wrap = false;
			}
			else Destroy(gameObject);
		}
		else if (transform.position.x > levelBounds)
		{
			if (wrap)
			{
				transform.position = new Vector3(-levelBounds + 0.01f, transform.position.y, transform.position.z);
				wrap = false;
			}
			else Destroy(gameObject);
		}

		// destroy on vertical bounds
		if (transform.position.y < -levelBounds - EndlessLevelManager.DESTROY_DISTANCE + Camera.main.transform.position.y || transform.position.y > levelBounds + 2f + Camera.main.transform.position.y) Destroy(gameObject);
	}

	private void LevelFixedUpdate()
	{
		//if ()
	}

	private void HandleCollision(GameObject g)
	{
		MonoBehaviour[] scripts = g.GetComponents<MonoBehaviour>();

		foreach (MonoBehaviour script in scripts)
		{
			if (script is IShootable)
			{
				IShootable shootable = (script as IShootable);
				shootable.OnShot();
				if (GameHandler.Instance.player.ammoEnabled && shootable.replenishAmmo) GameHandler.Instance.player.AddAmmo(refillOnShot);

				ParticleSystem p = ParticleManager.CreateParticleSystem("Bullet", transform.position, transform.parent, true);

				var velModule = p.velocityOverLifetime;

				velModule.xMultiplier = rb.velocity.x;
				velModule.yMultiplier = rb.velocity.y;	

				Destroy(gameObject);
			}
		}
	}
}
