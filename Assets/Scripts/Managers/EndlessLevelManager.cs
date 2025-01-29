using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessLevelManager : MonoBehaviour
{
    public static EndlessLevelManager Instance { get; private set; }

	/*
    [Header("Explosives")]
    [SerializeField] private GameObject explosive;
    [SerializeField] private Transform explosiveContainer;
    */

	public Transform itemContainer; // parent container

	public List<LevelObjectData> levelObjects = new();

    public List<LevelObjectModifierData> levelObjectModifiers = new();
	/*
    private float explosiveDestroyDistance = 5;
    private float minimumExplosiveCount = 10;

    private float explosiveSpawnHeight = 0;
    [SerializeField] private float spawnDistanceMin = 10f;
    [SerializeField] private float spawnDistanceMax = 10f;
    */

	[Header("Objects")]
    [SerializeField] private float explosiveStrength = 1f;
	[SerializeField] private float forceEmitterStrength = 3f;

	[Header("Modifiers")]
	[SerializeField] private float spinnerSpeed = 0.5f;
	[SerializeField] private float waypointSpeed = 0.5f;

	public static readonly float LEVEL_BOUNDS = 4.5f;

	private void Awake()
	{
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
	}


	void Start()
    {
        // spawn initial levelitems
        foreach (LevelObjectData item in levelObjects)
        {
            item.Init();

			SpawnItemsUntilFull(item);
		}
    }

	void Update()
	{
		// kinda cursed, but it means I don't have to continuously update a list of active objects etc. Instead, the container transform will hold whatever items are active. This means each object needs its own container.
		foreach (LevelObjectData item in levelObjects)
		{
			foreach (Transform child in item.container)
			{
				if (child.position.y < GameHandler.Instance.maxPlayerHeight - LevelObjectData.DESTROY_DISTANCE) Destroy(child.gameObject);
			}

			SpawnItemsUntilFull(item);
		}
	}

	public void SpawnItemsUntilFull(LevelObjectData item)
	{
		while (item.count < item.maxCount)
		{
			// Fix overlapping issue on init
			var g = item.AddNew();

			if (!item.modifiable) return;
			foreach (LevelObjectModifierData modifier in levelObjectModifiers)
			{
				modifier.Update();
				if (modifier.nextApplyCounter > 0) continue;
				modifier.Apply(g);
				break;
			}
		}
	}

    public void ConfigureLevelItem(GameObject g)
    {
        g.TryGetComponent<Explosive>(out Explosive e);
        if (e != null)
        {
            e.strength = explosiveStrength;
        }

        g.TryGetComponent(out ForceEmitter f);
        if (f != null)
        {
            f.force = -forceEmitterStrength;
        }
    }

	public void ConfigureLevelItemModifier(GameObject g)
	{
		g.TryGetComponent(out Spinner s);
		if (s != null)
		{
			s.spinSpeed = spinnerSpeed * Mathf.Sign(Random.value - 0.5f);
			s.transform.Rotate(0, 0, Random.Range(0, 360f));
		}
		g.TryGetComponent(out MoveBetweenWaypoints w);
		if (w != null)
		{
			w.t = Random.value;
			w.speed = waypointSpeed;
		}
	}

	[System.Serializable]
	public class LevelObjectData
	{
		public string name;

		public GameObject obj;

		[HideInInspector] public Transform container;

		public float spawnDistanceMin = 8f;
		public float spawnDistanceMax = 10f;

		[Tooltip("Height to begin spawning")]
		public float spawnHeight = 0;

		public static readonly float DESTROY_DISTANCE = 7f;
		public float maxCount = 8f;

		[Tooltip("Distance from other objects required to spawn")]
		public float minimumSpawnDistance = 0f;

		[Tooltip("Coefficient of height for decreasing spawn distance")]
		[Range(0f, 1f)]
		public float frequencyScalingCoefficient = 0;
		private static readonly float SCALING_COEFFICIENT = 0.0000005f; // scaling for height

		public bool modifiable = false;

		public int count { get { return container.childCount; } }

		//public UnityEvent<GameObject> configure;
		public void Init()
		{
			container = new GameObject(name + " Container").transform;
			container.SetParent(Instance.itemContainer);

			//while (count < maxCount) AddNew();
		}

		public GameObject AddNew()
		{
			if (container == null) { Init(); return null; }

			spawnHeight += Random.Range(spawnDistanceMin, spawnDistanceMax) * ((1 - frequencyScalingCoefficient) + frequencyScalingCoefficient * (1 / (1 + spawnHeight * SCALING_COEFFICIENT)));

			GameObject g = GameObject.Instantiate(obj);

			if (minimumSpawnDistance != 0)
			{
				for (int i = 0; i <= 99; i++)
				{
					if (i == 99) Debug.Log("Endless level manager item failed to get minimumSpawnDistance position");

					Vector3 testPosition = new Vector3(Random.Range(-EndlessLevelManager.LEVEL_BOUNDS, EndlessLevelManager.LEVEL_BOUNDS), spawnHeight, 0);

					Collider2D[] collisions = Physics2D.OverlapCircleAll(testPosition, minimumSpawnDistance);
					if (collisions.Length > 0) continue;

					g.transform.position = testPosition;
					break;
				}
			}
			else
			{
				g.transform.position = new Vector3(Random.Range(-EndlessLevelManager.LEVEL_BOUNDS, EndlessLevelManager.LEVEL_BOUNDS), spawnHeight, 0);
			}

			g.transform.SetParent(container);

			Instance.ConfigureLevelItem(g);

			return g;
		}
	}

	[System.Serializable]
	public class LevelObjectModifierData
	{
		public string name;

		public GameObject obj;

		public int nextApplyCounter = 20;

		[Tooltip("Apply to every nth object")]
		public int applyPeriodMin = 15;
		public int applyPeriodMax = 15;

		public Vector2 offset = Vector2.zero;

		private int appliedCount = 0;
		[Tooltip("Applied count to increase apply period. Scaled by (1+times_increased/10)")]
		public int appliedCountToIncrease = 50;
		private int timesIncreased = 0;
		private int maxIncrease = 5;

		public void Update()
		{
			nextApplyCounter--;
		}

		public GameObject Apply(GameObject applyObject)
		{
			GameObject g = GameObject.Instantiate(obj);	

			g.transform.SetParent(applyObject.transform.parent);
			g.transform.position = applyObject.transform.position;

			applyObject.transform.SetParent(g.transform);

			applyObject.transform.localPosition = offset;

			nextApplyCounter = Random.Range(applyPeriodMin, applyPeriodMax);

			Instance.ConfigureLevelItemModifier(g);

			appliedCount++;
			if (appliedCount >= appliedCountToIncrease * (1 + timesIncreased/10) && timesIncreased < maxIncrease)
			{
				appliedCount -= appliedCountToIncrease;
				timesIncreased++;

				applyPeriodMin--;
				applyPeriodMax--;
			}

			return g;
		}
	}
}
