using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupUI : MonoBehaviour
{
	private RectTransform rectTransform;
	[SerializeField] private ProgressType progressType = ProgressType.Size;
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

		if (progressType == ProgressType.Size) progressImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (1 - timer / durationSeconds) * rectTransform.sizeDelta.y);
		else if (progressType == ProgressType.Radial) progressImage.fillAmount = 1f - timer/durationSeconds;
	}

	private enum ProgressType
	{
		Size,
		Radial
	}
}
