using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraManager : MonoBehaviour
{
    public GameObject player;

    [SerializeField] public float lookAhead = 2f;

    void Update()
    {
        if (player == null) return;
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(player.transform.position.y + lookAhead, transform.position.y, Mathf.Infinity), transform.position.z);
    }
}
