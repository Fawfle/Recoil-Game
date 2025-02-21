using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

// manager for individual levels
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

	public Rect bounds = new Rect(-11/2f, -11/2f, 11, 11f);

	public LevelGoal goal;

	public TextMeshProUGUI practiceUIText;

	[HideInInspector] public static bool isPracticeMode = false;
	[HideInInspector] public static Vector2 practiceSpawnPosition;
	public Transform practiceSpawnMarker;

	private void Awake()
	{
		if (Instance != null && Instance != this) { Destroy(gameObject); return; }
		Instance = this;
	}

	private void OnEnable()
	{
		if (!TransitionManager.sceneReloaded)
		{
			DisablePracticeMode();
		}

		if (isPracticeMode) UpdatePracticeUI();
	}

	private void Update()
	{
		if (TransitionManager.transitioning) return;

		if (!GameHandler.Instance.IsEndState() && ControlManager.Controls.game.start_practice.WasPressedThisFrame())
		{
			practiceSpawnPosition = GameHandler.Instance.player.transform.position;

			if (!isPracticeMode) EnablePracticeMode();
			UpdatePracticeUI();
		}
		else if (isPracticeMode && ControlManager.Controls.game.end_practice.WasPressedThisFrame())
		{
			DisablePracticeMode();
			TransitionManager.Instance.ReloadScene();
		}
	}

	public void EnablePracticeMode()
	{
		isPracticeMode = true;
	}

	public void UpdatePracticeUI()
	{
		practiceUIText.gameObject.SetActive(true);
		practiceSpawnMarker.transform.position = practiceSpawnPosition;
		practiceSpawnMarker.gameObject.SetActive(true);
	}

	public void DisablePracticeMode()
	{
		isPracticeMode = false;
	}

	public bool IsInBounds(Vector3 vec, Vector2 size = default(Vector2))
	{
		if (vec.x + size.x / 2f < bounds.xMin) return false;
		if (vec.x - size.x /2f > bounds.xMax) return false;
		if (vec.y + size.y / 2f  < bounds.yMin) return false;
		if (vec.y - size.y /2f > bounds.yMax) return false;

		return true;
	}

	public Vector3 ClampInBounds(Vector3 vec, Vector2 size = default(Vector2))
	{
		vec.x = Mathf.Clamp(vec.x, bounds.xMin + size.x/2f, bounds.xMax - size.x/2f);
		vec.y = Mathf.Clamp(vec.y, bounds.yMin + size.y/2f, bounds.yMax - size.y/2f);

		return vec;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		// draw level bounds
		Gizmos.color = Color.green;

		Vector3[] points = new Vector3[4]
		{
			new Vector2(bounds.xMin, bounds.yMin),
			new Vector2(bounds.xMin, bounds.yMax),
			new Vector2(bounds.xMax, bounds.yMax),
			new Vector2(bounds.xMax, bounds.yMin)
		};

		Gizmos.DrawLineStrip(points, true);
	}
#endif
}
