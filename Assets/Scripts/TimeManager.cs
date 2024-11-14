using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public bool paused = false;

	private void Awake()
	{
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

	void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
		{
            Pause(!paused);
		}
    }

    void Pause(bool input)
	{
        paused = input;
        Time.timeScale = paused ? 0 : 1;
	}
}
