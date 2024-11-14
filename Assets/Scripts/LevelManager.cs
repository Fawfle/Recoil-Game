using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    private GameObject player;

    /*
    [Header("Explosives")]
    [SerializeField] private GameObject explosive;
    [SerializeField] private Transform explosiveContainer;
    */
    public List<LevelItemData> levelItems = new();
    /*
    private float explosiveDestroyDistance = 5;
    private float minimumExplosiveCount = 10;

    private float explosiveSpawnHeight = 0;
    [SerializeField] private float spawnDistanceMin = 10f;
    [SerializeField] private float spawnDistanceMax = 10f;
    */

    [SerializeField] private float explosiveStrength = 1f;

    public static readonly float LEVEL_BOUNDS = 4.5f;


    void Start()
    {
        player = GameObject.FindWithTag("Player");

        // spawn initial levelitems
        foreach (LevelItemData item in levelItems)
        {
            for (int i = 0; i < item.maxCount; i++) item.AddNew();
        }
    }

    void Update()
    {
        if (player == null) return;
        foreach (LevelItemData item in levelItems)
        {
            foreach (Transform child in item.container)
			{
                if (child.position.y < player.transform.position.y - LevelItemData.DESTROY_DISTANCE) Destroy(child.gameObject);
			}

            while (item.count < item.maxCount) item.AddNew();
        }
    }

	#region Configure LevelItems

    public void ConfigureExplosive(GameObject g)
	{
        Explosive e = g.GetComponent<Explosive>();
        e.strength = explosiveStrength;
	}

    public void ConfigureSpike(GameObject g)
	{
        DamageObject o = g.GetComponent<DamageObject>();
	}

	#endregion
}


[System.Serializable]
public class LevelItemData
{
    public string name;

    public GameObject obj;
    public Transform container;

    public float spawnDistanceMin = 8f;
    public float spawnDistanceMax = 10f;

    [HideInInspector] public float spawnHeight = 0;

    public static readonly float DESTROY_DISTANCE = 5f;
    public float maxCount = 8f;
    public int count { get { return container.childCount; } }

    public UnityEvent<GameObject> configure;

    public void AddNew()
    {
        spawnHeight += Random.Range(spawnDistanceMin, spawnDistanceMax);

        GameObject g = GameObject.Instantiate(obj, new Vector3(Random.Range(-LevelManager.LEVEL_BOUNDS, LevelManager.LEVEL_BOUNDS), spawnHeight, 0), Quaternion.identity);
        g.transform.SetParent(container);

        configure?.Invoke(g);
    }
}