using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;

[RequireComponent(typeof(Animator))]
public class TransitionManager : MonoBehaviour
{
	private Animator anim;

	public Action TransitionStart;
	public Action TransitionEnd;

	public static TransitionManager Instance { get; private set; }

	[HideInInspector] public static bool transitioning = false;

	public static bool DEBUG = true;

	private void Awake()
	{
		if (Instance != null && Instance != this) { Destroy(this); return; }
		else Instance = this;

		anim = GetComponent<Animator>();
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
		anim.Play("TransitionIn");
		
		if (DEBUG) print("Loading Scene: " + name);

		transitioning = true;
		yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length * anim.GetCurrentAnimatorStateInfo(0).speed);

		SceneManager.LoadScene(name);
	}

	public void SceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (!transitioning) return;
		print(transitioning);
		StartCoroutine(SceneLoadedRoutine(scene));
	}

	public IEnumerator SceneLoadedRoutine(Scene scene)
	{
		if (!transitioning) yield break;

		anim.Play("TransitionOut");
		transitioning = true;

		if (DEBUG) print("Loaded Scene: " + scene.name);

		yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length * anim.GetCurrentAnimatorStateInfo(0).speed);

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
