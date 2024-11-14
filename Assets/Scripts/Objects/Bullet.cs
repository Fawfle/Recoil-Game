using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
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
				// TODO: add some effect when destroying
				Destroy(gameObject);
			}
		}
	}
}
