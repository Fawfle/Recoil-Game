using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParentGraphic : MonoBehaviour
{
    public bool matchAlpha = true;

    private Graphic graphic;

    public List<Graphic> childGraphics = new();

	private void Awake()
	{
		graphic = GetComponent<Graphic>();
	}

	private void Update()
	{
		if (graphic != null)
		{
			foreach (Graphic g in childGraphics)
			{
				if (matchAlpha)
				{
					g.CrossFadeAlpha(graphic.canvasRenderer.GetAlpha(), 0f, true);
				}
			}
		}
	}
}
