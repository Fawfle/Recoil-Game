using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class FloatingText : MonoBehaviour
{
	private TextMeshPro textMeshPro;

	public float floatDistance = 0.5f;
	public float duration = 1f;

	private float t = 0f;

	private Vector2 startPosition;
	private Vector2 targetPosition;

	private void Awake()
	{
		textMeshPro = GetComponent<TextMeshPro>();

		startPosition = transform.position;
		targetPosition = startPosition + Vector2.up * floatDistance;
	}

	private void Update()
	{
		t += Time.deltaTime / duration;

		transform.position = Vector2.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0, 1, t));

		textMeshPro.alpha = Mathf.SmoothStep(1, 0, t);

		if (t > 1f) Destroy(gameObject);
	}
}
