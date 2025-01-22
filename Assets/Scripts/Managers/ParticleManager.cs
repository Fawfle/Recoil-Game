using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
	public static ParticleManager Instance;

	[SerializeField] private List<ParticleData> particleSystemList = new();
	public Dictionary<string, ParticleData> particleSystems = new();

	private void Awake()
	{
		if (Instance != null && Instance != this) { Destroy(gameObject); return; }
		Instance = this;

		particleSystems = new();
		
		foreach (ParticleData p in particleSystemList) {
			particleSystems.Add(p.key, p);
		}
	}

	public static ParticleSystem DestroyAfterDuration(ParticleSystem p)
	{
		Destroy(p, p.main.duration);

		return p;
	}

	public static ParticleSystem CreateParticleSystem(string key)
	{
		ParticleSystem p = Instantiate(Instance.particleSystems[key].particleSystem);

		ParticleSystem.MainModule main = p.main;
		main.loop = Instance.particleSystems[key].loop;

		return p;
	}

	public static ParticleSystem CreateParticleSystem(string key, Vector2 position)
	{
		ParticleSystem p = CreateParticleSystem(key);

		p.transform.SetPositionAndRotation(position, Quaternion.identity);

		return p;
	}

	public static ParticleSystem CreateParticleSystem(string key, Vector2 position, Transform parent)
	{
		ParticleSystem p = CreateParticleSystem(key, position);

		p.transform.SetParent(parent);

		return p;
	}

	[System.Serializable]
	public class ParticleData
	{
		public string key;
		public ParticleSystem particleSystem;
		public bool loop = false;
	}
}