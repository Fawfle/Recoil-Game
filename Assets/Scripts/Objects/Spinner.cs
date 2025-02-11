using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public bool spinning = true;
    [Tooltip("Rotations per second")]
    public float spinSpeed = 1f;

    public float spinOffsetDegrees = 0f;

	private void Awake()
	{
        transform.Rotate(0, 0, spinOffsetDegrees);
	}

	void Update()
    {
        if (!spinning) return;

        float rotateAmount = spinSpeed * 360f * Time.deltaTime;
        transform.Rotate(0, 0, rotateAmount);
    }
}