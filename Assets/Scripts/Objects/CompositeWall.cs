using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeWall : MonoBehaviour
{
	private void Awake()
	{
		foreach (Transform child in transform)
		{
			child.TryGetComponent<Collider2D>(out var coll);

			if (coll != null)
			{
				coll.usedByComposite = true;
			}

			child.TryGetComponent<DamageObject>(out var damageObject);

			if (damageObject != null)
			{
				damageObject.enabled = false;
			}
		}
	}
}
