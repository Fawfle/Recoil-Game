using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivePowerupUI : MonoBehaviour
{
	private RectTransform rectTransform;
	[SerializeField] private Image progressImage;
	public Image icon;

	public float durationSeconds = 1f;
	public float timer = 0f;
	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	private void Update()
	{
		timer += Time.deltaTime;

		if (timer >= durationSeconds)
		{
			Destroy(gameObject);
			return;
		}

		progressImage.rectTransform.sizeDelta = new Vector2(progressImage.rectTransform.sizeDelta.x, (1 - timer / durationSeconds) * rectTransform.sizeDelta.y);
	}
}
