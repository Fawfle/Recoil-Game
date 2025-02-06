using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using PathCreation;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Header("Endless/Levels")]
    [SerializeField] private Transform target;
    [SerializeField] public float lookAhead = 2f;

    [Header("Levels")]
    [Tooltip("Whether or not to pan on intro")]
    [SerializeField] private bool introPanEnabled = false;
    public PathCreator panPath;
    [Tooltip("If not null, will treat tranform's position as start position")]
    [SerializeField] private Transform panPositionStartTarget = null;
    [Tooltip("Speed to lerp to lookahead during Levels")]
    [SerializeField] private float lookAheadSpeed = 1f;

    private Vector2 lookAheadOffset = Vector2.zero;

    private float introPanLingerDurationSeconds = 0.5f;
    private float introPanSpeed = 7.5f;

    public bool isPanning { get; private set; } = false;

	private void Awake()
	{
		if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
	}

	private void Start()
    {
		if (!CanIntroPan())
        {
            if (GameHandler.Instance.IsGameMode(GameMode.Level) && target != null) UpdateCameraPosition(target.position);
            return;
        }

        if (panPath == null) return;

        // loop through anchor points (every 3) and clamp them in level bounds
		for (int i = 0; i < panPath.bezierPath.NumAnchorPoints; i++)
		{
            int index = i * 3;
			panPath.bezierPath.SetPoint(index, ClampInLevelBounds(panPath.bezierPath.GetPoint(index)));
		}

        panPath.bezierPath.AutoSetAllControlPoints();

		UpdateCameraPosition(ClampInLevelBounds(panPath.path.GetPointAtDistance(0, EndOfPathInstruction.Stop)));

        StartCoroutine(CameraPanRoutine());
	}

    private IEnumerator CameraPanRoutine()
    {
        isPanning = true;

        yield return new WaitForSeconds(introPanLingerDurationSeconds);

        float distanceTraveled = 0f;

        while (panPath.path.length - distanceTraveled > 0.05f)
        {
            distanceTraveled += introPanSpeed * Time.deltaTime;
			Vector2 nextPosition = panPath.path.GetPointAtDistance(distanceTraveled);

			UpdateLookAheadOffset();
			nextPosition += lookAheadOffset;

			UpdateCameraPosition(ClampInLevelBounds(nextPosition));
            yield return null;
        }

		print(panPath.path.GetPointAtDistance(100f, EndOfPathInstruction.Stop));


		//UpdateCameraPosition(ClampInLevelBounds(panPath.path.GetPointAtTime(1f, EndOfPathInstruction.Stop) + (Vector3)lookAheadOffset));

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

			Vector2 newPosition = ClampInLevelBounds(target.position) + lookAheadOffset;

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

    private bool CanIntroPan()
    {
        return GameHandler.Instance.IsGameMode(GameMode.Level) && introPanEnabled && !TransitionManager.sceneReloaded && SaveManager.save.levelIntroPanEnabled;

	}

#if UNITY_EDITOR
    // update start and end for cam path
    private void OnDrawGizmos()
	{
        if (Application.isPlaying || !introPanEnabled) return;

        if (panPath == null) return;

		if (panPositionStartTarget != null) panPath.bezierPath.SetPoint(0, panPositionStartTarget.position);

        if (target != null) panPath.bezierPath.SetPoint(panPath.bezierPath.NumPoints - 1, target.position);

        if (panPositionStartTarget != null || target != null) panPath.bezierPath.AutoSetAllControlPoints();
	}
#endif
}
