using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeRotation : MonoBehaviour
{
	private void Update()
	{
		transform.rotation = Quaternion.identity;
	}

	private void FixedUpdate()
	{
		transform.rotation = Quaternion.identity;
	}
}
