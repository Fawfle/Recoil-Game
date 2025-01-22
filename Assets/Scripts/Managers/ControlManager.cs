using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

public class ControlManager : MonoBehaviour
{
	public static ControlManager Instance { get; private set; }
	public static PlayerControls Controls { get; private set; }

	//[SerializeField] private InputSystemUIInputModule inputSystemUIInputModule;

	private void Awake()
	{
		if (Instance != null && Instance != this) { Destroy(gameObject); return; }
		Instance = this;

		InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;

		Controls = new();
		Controls.Enable();
	}

	private void OnDestroy()
	{
		Controls.Disable();
	}

	private void OnTransitionStart()
	{
		//controls.asset.Disable();
		//controls.Disable();

		//inputSystemUIInputModule.enabled = false;
	}

	private void OnTransitionEnd()
	{
		//inputSystemUIInputModule.enabled = true;
		//controls.universal.Enable();
	}

	private void OnEnable()
	{
		//TransitionManager.Instance.TransitionStart += OnTransitionStart;
		//TransitionManager.Instance.TransitionEnd += OnTransitionEnd;
	}

	private void OnDisable()
	{
		//TransitionManager.Instance.TransitionStart -= OnTransitionStart;
		//TransitionManager.Instance.TransitionEnd -= OnTransitionEnd;
	}
}
