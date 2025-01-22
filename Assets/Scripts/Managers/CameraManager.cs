using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraManager : MonoBehaviour
{
    [SerializeField] public float lookAhead = 2f;

    void Update()
    {
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(GameHandler.Instance.player.transform.position.y + lookAhead, transform.position.y, Mathf.Infinity), transform.position.z);
    }
}
