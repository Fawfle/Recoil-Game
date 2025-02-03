using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoRefill : MonoBehaviour, ICollidable
{
	public void OnCollide(Player player)
	{
		player.RefillAmmo();

		ParticleManager.CreateParticleSystem("Ammo", transform.position, transform.parent, true);

		Destroy(gameObject);
	}
}
