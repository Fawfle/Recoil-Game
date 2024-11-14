using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    private Material mat;
    public Vector2 dampen = new Vector2(40, 10);

    //[SerializeField] private Transform cameraTransform;
    private GameObject player;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;
        transform.position = new Vector3(transform.position.x, GameHandler.Instance.maxPlayerHeight, transform.position.z);
        Vector2 offset = new Vector2(player.transform.position.x / dampen.x, (transform.position.y / dampen.y) % 10);
        mat.SetTextureOffset("_MainTex", offset);
    }
}
