using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour
{
    public float scalingMin = 1f;
	public float scalingMax = 1f;

	public float period = 2f;
    private float t = 0;

    void Update()
    {   
        t += Time.deltaTime / period * Mathf.PI * 2;

		float scalingDelta = scalingMax - scalingMin;

		transform.localScale = Vector3.one * (scalingMin + (scalingDelta / 2f) + Mathf.Sin(t) * (scalingDelta / 2f));
		//transform.localScale = Vector3.one * Mathf.SmoothStep(scaling.x, scaling.y, 0.5f + Mathf.Sin(t) * 0.5f);
	}
}
