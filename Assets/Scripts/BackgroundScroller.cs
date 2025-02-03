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

    private static readonly float PlayerOffsetDampen = 10f;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            if (GameHandler.Instance == null) return;

            if (GameHandler.Instance.IsGameMode(GameMode.Endless)) 
            { 
                transform.position = new Vector3(transform.position.x, GameHandler.Instance.maxPlayerHeight, transform.position.z);
				offset = new Vector2((target.transform.position.x / dampen.x) % 1, (transform.position.y / dampen.y) % 1);
			}
			else if (GameHandler.Instance.IsGameMode(GameMode.Level))
            {
                Vector3 newPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
                
                newPosition = LevelManager.Instance.ClampInBounds(newPosition, 2 * Camera.main.orthographicSize * Vector2.one);

                transform.position = newPosition;

                Vector2 newOffset = target.transform.position;

                // minor offset from player position so the background doesn't just "stop" at edges
				newOffset += (Vector2)(GameHandler.Instance.player.transform.position - target.position) / PlayerOffsetDampen;

				offset = new Vector2((newOffset.x / dampen.x) % 1, (newOffset.y / dampen.y) % 1);
			}
        }

        mat.SetTextureOffset("_MainTex", offset);
    }
}
