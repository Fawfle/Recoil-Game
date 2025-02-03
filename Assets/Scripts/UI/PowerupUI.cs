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

	[HideInInspector] public bool timed = true;

	public float durationSeconds = 1f;
	public float progress = 0f;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	private void Update()
	{
		if (timed)
		{
			progress += Time.deltaTime / durationSeconds;
		}

		if (progress >= 1f)
		{
			Destroy(gameObject);
			return;
		}

		if (progressType == ProgressType.Size) progressImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (1 - progress) * rectTransform.sizeDelta.y);
		else if (progressType == ProgressType.Radial) progressImage.fillAmount = 1f - progress;
	}

	private enum ProgressType
	{
		Size,
		Radial
	}
}
