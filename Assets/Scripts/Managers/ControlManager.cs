using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

public class ControlManager : MonoBehaviour
{
	public static ControlManager Instance { get; private set; }
	public static PlayerControls Controls { get; private set; }

	//public static bool clickToShootEnabled { get; private set; } = false;

	//[SerializeField] private InputSystemUIInputModule inputSystemUIInputModule;

	private void Awake()
	{
		if (Instance != null && Instance != this) { Destroy(gameObject); return; }
		Instance = this;

		InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsManually;

		//if (Webgl) WebGLInput.captureAllKeyboardInput = true;

		Controls = new();
		Controls.Enable();

		InputSystem.Update(); // clear inputs

		// remove touch to shoot if not on mobile
		if (!Application.isMobilePlatform) Controls.game.shoot.ChangeBinding(1).Erase();
		//print(Controls.game.shoot.bindings);
	}

	// cursed (fixed update doesn't run when timescale is 0, so I need to manually do it.
	private void Update()
	{
		if (Time.timeScale == 0) InputSystem.Update();
	}

	private void FixedUpdate()
	{
		if (Time.timeScale != 0) InputSystem.Update();
	}

	/*
	public static void SetUpdateMode(InputSettings.UpdateMode mode)
	{
		InputSystem.settings.updateMode = mode;
	}
	*/

	// handle alternate shooting
	public static bool WasShootPressedThisFrame()
	{
		return Controls.game.shoot.WasPressedThisFrame() || (SaveManager.save.clickToShootEnabled && Controls.game.shootAlt.WasPressedThisFrame());
	}

	/*
	private void SetClickToShootEnabled(bool enabled)
	{
		clickToShootEnabled = enabled;
	}
	*/

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
