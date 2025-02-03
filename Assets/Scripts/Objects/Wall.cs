using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class Wall : MonoBehaviour
{
	private void Awake()
	{
		UpdateCollider();
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		UpdateCollider();
	}
#endif

	private void UpdateCollider()
	{
		var sr = GetComponent<SpriteRenderer>();
		var coll = GetComponent<BoxCollider2D>();

		coll.size = sr.size - 2 * coll.edgeRadius * Vector2.one;
	}
}
