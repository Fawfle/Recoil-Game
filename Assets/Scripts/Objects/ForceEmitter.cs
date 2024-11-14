using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ForceEmitter : MonoBehaviour
{

    public float range = 3f;
	public float force = 3f;

	public void Update()
	{
		if (GameHandler.Instance.player == null) return;

		float distance = Vector2.Distance(transform.position, GameHandler.Instance.player.transform.position);
		if (distance < range)
		{
			Vector2 direction = (GameHandler.Instance.player.transform.position - transform.position).normalized;
			GameHandler.Instance.player.GetComponent<Rigidbody2D>().velocity += (range - distance) * force * Time.deltaTime * direction;
		}
	}
}
