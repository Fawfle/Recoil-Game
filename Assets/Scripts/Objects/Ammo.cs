using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour, ICollidable
{
	public void OnCollide(Player player)
	{
		player.AddAmmo(1);
		Destroy();
	}

	private void Destroy()
	{
		Destroy(gameObject);
	}
}
