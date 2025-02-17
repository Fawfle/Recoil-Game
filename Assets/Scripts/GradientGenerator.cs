
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(GradientGenerator))]
public class GradientGeneratorInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		GradientGenerator g = (GradientGenerator)target;

		if (GUILayout.Button("Generate Gradient"))
		{
			g.GenerateGradient();
		}
	}
}

#endif

public class GradientGenerator : MonoBehaviour
{
	//private Image image;

	[SerializeField] private Texture2D texture;

	[SerializeField] private bool useAlpha = false;

	[Range(0f, 1f)]
	public float saturation = 0.67f;

	private void Awake()
	{
		GenerateGradient();
	}

	public void GenerateGradient()
	{
		//image = GetComponent<Image>();

		//texture = (Texture2D)image.mainTexture;

		//print(texture);

		//print(texture.width);

		for (int x = 0; x < texture.width; x++)
		{
			Color sampleColor = Color.HSVToRGB((float)x / (float)texture.width, saturation, 1f);

			for (int y = 0; y < texture.height; y++)
			{
				if (useAlpha) sampleColor.a = texture.GetPixel(x, y).a;
				texture.SetPixel(x, y, sampleColor, 0);
			}
		}

		texture.Apply();

		//image.sprite = Sprite.Create(texture);
	}
}