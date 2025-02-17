using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class Wall : MonoBehaviour
{
	public SpriteRenderer sr;
	public BoxCollider2D coll;
	
	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
		coll = GetComponent<BoxCollider2D>();

		UpdateCollider();
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		sr = GetComponent<SpriteRenderer>();
		coll = GetComponent<BoxCollider2D>();

		UpdateCollider();
	}
#endif

	public void UpdateCollider()
	{
		coll.size = sr.size - 2 * coll.edgeRadius * Vector2.one;
	}
}
