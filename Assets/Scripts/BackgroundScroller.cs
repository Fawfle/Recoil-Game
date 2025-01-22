using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    private Material mat;
    public Vector2 dampen = new Vector2(40, 10);

    public Transform target = null;
    public Vector2 offset = Vector2.zero;

    //[SerializeField] private Transform cameraTransform;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null) {
			if (target.CompareTag("Player")) transform.position = new Vector3(transform.position.x, GameHandler.Instance.maxPlayerHeight, transform.position.z);
			offset = new Vector2((target.transform.position.x / dampen.x) % 1, (transform.position.y / dampen.y) % 1);
		}
        
        mat.SetTextureOffset("_MainTex", offset);
    }
}
