using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MoveBetweenWaypoints : MonoBehaviour
{
	[Tooltip("Relative offset from starting position")]
    public List<Vector2> waypoints;
	public float speed = 2f;
	private int currentWaypointIndex = 0;
	private int nextWaypointIndex = 1;

	[HideInInspector] public float t = 0;
	//[HideInInspector] public Vector2 startPosition;

	private void Update()
	{
		if (transform.childCount == 0) return;

		foreach (Transform child in transform)
		{

			t += speed * Time.deltaTime;
			child.localPosition = Vector2.Lerp(waypoints[currentWaypointIndex], waypoints[nextWaypointIndex], Mathf.SmoothStep(0, 1, t));

			if (t >= 1f)
			{
				GoToNextWaypoint();
			}
		}
	}

	void GoToNextWaypoint()
	{
		currentWaypointIndex = nextWaypointIndex;
		nextWaypointIndex = (nextWaypointIndex + 1) % waypoints.Count;

		t = 0;
	}
	
	/*

	Vector2 GetWaypointPosition(int index)
	{
		return startPosition + waypoints[index];
	}

	public void UpdateStartPosition()
	{
		startPosition = transform.position;
	}
	*/
}
