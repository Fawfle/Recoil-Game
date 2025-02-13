using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class TransitionManager : MonoBehaviour
{
	private Animator anim;

	public Action TransitionStart;
	public Action TransitionEnd;

	public static TransitionManager Instance { get; private set; }

	[HideInInspector] public static bool transitioning = false;

	public static bool DEBUG = false;


	public static bool sceneReloaded { get; private set; } = false;
	public static string lastSceneLoaded { get; private set; } = null;

	private void Awake()
	{
		if (Instance != null && Instance != this) { Destroy(this); return; }
		else Instance = this;

		anim = GetComponent<Animator>();
		// scaled/unscaled used for pausing game issues
		//anim.updateMode = AnimatorUpdateMode.UnscaledTime;
	}

	public void LoadScene(String scene)
	{
		if (transitioning) return;
		StartCoroutine(LoadSceneRoutine(scene));
	}

	public void ReloadScene()
	{
		LoadScene(SceneManager.GetActiveScene().name);
	}

	public IEnumerator LoadSceneRoutine(string name)
	{
		if (transitioning) yield break;

		TransitionStart?.Invoke();

		if (Time.timeScale == 0) anim.updateMode = AnimatorUpdateMode.UnscaledTime;
		anim.Play("TransitionIn");
		
		if (DEBUG) print("Loading Scene: " + name);

		transitioning = true;
		yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length * anim.GetCurrentAnimatorStateInfo(0).speed);

		if (Time.timeScale == 0) anim.updateMode = AnimatorUpdateMode.Normal;

		DOTween.Clear();
		SceneManager.LoadScene(name);
	}

	public void SceneLoaded(Scene scene, LoadSceneMode mode)
	{
		sceneReloaded = lastSceneLoaded == scene.name;
		lastSceneLoaded = scene.name;

		if (!transitioning) return;
		Time.timeScale = 1.0f;

		StartCoroutine(SceneLoadedRoutine(scene));
	}

	public IEnumerator SceneLoadedRoutine(Scene scene)
	{
		if (!transitioning) yield break;

		anim.Play("TransitionOut");
		transitioning = true;

		if (DEBUG) print("Loaded Scene: " + scene.name);

		yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length * anim.GetCurrentAnimatorStateInfo(0).speed * 0.3f);

		TransitionEnd?.Invoke();
		transitioning = false;
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += SceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= SceneLoaded;
	}
}
