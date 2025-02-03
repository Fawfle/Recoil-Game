using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;
using System.Runtime;
using System.Runtime.CompilerServices;

public class CameraManager : MonoBehaviour
{

    [Header("Endless/Levels")]
    [SerializeField] private Transform target;
    [SerializeField] public float lookAhead = 2f;

    [Header("Levels")]
    [Tooltip("Whether or not to pan on intro")] // TODO: make this save on reset
    [SerializeField] private bool introPanEnabled = false;
    [Tooltip("If not null, will treat tranform's position as start position")]
    [SerializeField] private Transform panPositionTransform = null;
    [Tooltip("Position to start camera (for showing goal), it will lerp to target position")]
    [SerializeField] private Vector2 panPosition = Vector2.zero;
    [Tooltip("Speed to lerp to lookahead during Levels")]
    [SerializeField] private float lookAheadSpeed = 1f;

    private Vector2 lookAheadOffset = Vector2.zero;

    private float introPanLingerDurationSeconds = 0.5f;
    private float introPanDurationSeconds = 2f;

    private bool isPanning = false;

    private void Start()
    {
		if (!CanIntroPan())
        {
            if (GameHandler.Instance.IsGameMode(GameMode.Level) && target != null) UpdateCameraPosition(target.position);
            return;
        }

        if (panPositionTransform != null) panPosition = panPositionTransform.position;

        panPosition = ClampInLevelBounds(panPosition);
		UpdateCameraPosition(panPosition);

        StartCoroutine(CameraPanRoutine());
	}

    private IEnumerator CameraPanRoutine()
    {
        isPanning = true;

        yield return new WaitForSeconds(introPanLingerDurationSeconds);

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime / introPanDurationSeconds;

			UpdateLookAheadOffset();

			Vector3 lerpTarget = ClampInLevelBounds(target.position + (Vector3)lookAheadOffset);

            Vector2 lerpedPosition = Vector2.Lerp(panPosition, lerpTarget, t);

			UpdateCameraPosition(ClampInLevelBounds(lerpedPosition));
            yield return null;
        }

        UpdateCameraPosition(ClampInLevelBounds(target.position + (Vector3)lookAheadOffset));

        isPanning = false;
    }

	void Update()
    {
        if (target == null) return;

        if (GameHandler.Instance.IsGameMode(GameMode.Endless))
        {
            Vector3 newPosition = transform.position;
			newPosition.y = Mathf.Max(target.position.y + lookAhead, transform.position.y);

            UpdateCameraPosition(newPosition);
        }
        else if (GameHandler.Instance.IsGameMode(GameMode.Level))
        {
            if (isPanning) return;

            UpdateLookAheadOffset();

			Vector3 newPosition = target.position + (Vector3)lookAheadOffset;

            UpdateCameraPosition(ClampInLevelBounds(newPosition));
        }
    }

    private void UpdateLookAheadOffset()
    {
		Vector2 lookAheadTarget = (Camera.main.ScreenToViewportPoint(Input.mousePosition) - Vector3.one * 0.5f) * lookAhead;

		lookAheadOffset = Vector2.Lerp(lookAheadOffset, lookAheadTarget, lookAheadSpeed * Time.deltaTime);
	}

    // a method that doens't modify z component
    private void UpdateCameraPosition(Vector3 p)
    {
        transform.position = new Vector3(p.x, p.y, transform.position.z);
    }

    private Vector2 ClampInLevelBounds(Vector2 p)
    {
        return LevelManager.Instance.ClampInBounds(p, 2 * Camera.main.orthographicSize * Vector2.one);
	}

	private float Smoothing(float t)
    {
        return Mathf.Sign(t) * Mathf.Pow((Mathf.Abs(t) - 1), 3) + 1;
    }

    private bool CanIntroPan()
    {
        return GameHandler.Instance.IsGameMode(GameMode.Level) && introPanEnabled && !TransitionManager.sceneReloaded;

	}
}
