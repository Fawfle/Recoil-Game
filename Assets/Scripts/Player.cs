using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Powerups;

public class Player : MonoBehaviour
{
    private CircleCollider2D coll;
    private Rigidbody2D rb;

    [Header("Components")]
    [SerializeField] private Transform gunAnchor;
    [SerializeField] private Transform gun;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletContainer;

    [SerializeField] private SpriteRenderer shieldSprite;

    [SerializeField] private bool screenWrap = false;

    [Header("Paramaters")]
    [SerializeField] public int health = 1;
    [SerializeField] public float recoilForce = 5;
    [Tooltip("Coefficient of velocity to conserve when shooting")]
    [SerializeField] private float velocityConservationCoefficient = 0.25f;


    private static readonly float KILL_DISTANCE = 6f;

    private bool invincible = false;
    [SerializeField] private float hitInvincibleDuration = 0.5f;

    [Header("Bullets")]
    [SerializeField] public bool ammoEnabled = false;
    [SerializeField] public int maxAmmo = 6;
    [SerializeField] public int ammo = 6;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float bulletLifetime = 2f;

    private bool firstShotSuper = true;

    private List<Powerup> powerups = new();
    
    private void Awake()
    {
        coll = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // called on init
	private void OnGameInit()
	{
        rb.simulated = false;
	}

    // called on start game
    private void OnGamePlay()
	{
        rb.simulated = true;
	}

    private void OnGameOver()
	{

	}

	private void Update()
    {
        if (TimeManager.Instance.paused || GameHandler.Instance.IsState(GameState.OVER)) return;

        for (int i = 0; i < powerups.Count; i++)
		{
            Powerup p = powerups[i];
            bool powerupOver = !p.Update();
            if (powerupOver)
            {
                RemovePowerup(p);
                i--;
            }
		}
        
        Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 relative = target - transform.position;
        float angle = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;
        gunAnchor.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (screenWrap)
        {
            float wrapBounds = 5.5f + coll.radius;
            if (transform.position.x < -wrapBounds) transform.position = new Vector3(wrapBounds - 0.01f, transform.position.y, transform.position.z);
            else if (transform.position.x > wrapBounds) transform.position = new Vector3(-wrapBounds + 0.01f, transform.position.y, transform.position.z);
        }
    }

	private void FixedUpdate()
	{
        if (TimeManager.Instance.paused) return;
        if (!GameHandler.Instance.IsState(GameState.PLAY))
        {
            if (transform.position.y < Camera.main.transform.position.y - (KILL_DISTANCE * 2f))
            {
                rb.velocity *= 0.99f;
                rb.gravityScale = 0f;
            }
            return;
        }

		if (transform.position.y < Camera.main.transform.position.y - KILL_DISTANCE) { Die(); return; }

		if (ControlManager.Controls.game.shoot.WasPressedThisFrame() && (!ammoEnabled || ammo > 0))
		{
            ShootBullet();
		}
	}

    public void AddAmmo(int count)
	{
        ammo = Mathf.Min((ammo + count), maxAmmo);
	}

    public void RefillAmmo()
    {
        ammo = maxAmmo;
    }

    void ShootBullet()
	{
        //print("shoot");
        Vector2 direction = (((Vector2)transform.position) - (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition)).normalized;
        rb.velocity = direction * recoilForce + rb.velocity * velocityConservationCoefficient;

        // first shot is "super"
        if (firstShotSuper)
        {
            rb.velocity *= 2;
            firstShotSuper = false;
        }

        if (ammoEnabled) ammo--;

        CreateBullet();
    }

    void CreateBullet()
	{
        GameObject bullet = Instantiate(bulletPrefab, gun.transform.position, gunAnchor.localRotation);
        bullet.transform.SetParent(bulletContainer);
        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * bulletSpeed;
        Destroy(bullet, bulletLifetime);
    }

    public void TakeDamage()
	{
        if (invincible) return;

        health--;

        if (shieldSprite.gameObject.activeSelf) shieldSprite.gameObject.SetActive(false);

		if (health <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(HitInvincibility());
	}

    public void Die()
	{
        if (!GameHandler.Instance.IsState(GameState.PLAY)) return;

        GameHandler.Instance.SetState(GameState.OVER);

        if (!DOTween.IsTweening(Camera.main)) DOTween.Complete(Camera.main);
        Camera.main.DOShakePosition(1f, 1f, 20, fadeOut:true);

        DimSprites(GetComponents<SpriteRenderer>());
        DimSprites(GetComponentsInChildren<SpriteRenderer>());

        rb.freezeRotation = false;
        coll.enabled = false;

		rb.AddTorque(25f * -Mathf.Sign(rb.velocity.x) * rb.velocity.magnitude);
		//rb.AddTorque(25f * Vector2.Dot(rb.velocity, Vector2.left));

        ParticleManager.DestroyAfterDuration(ParticleManager.CreateParticleSystem("PlayerDeathBurst", transform.position, transform));
        ParticleManager.CreateParticleSystem("Flames", transform.position, transform);
		//gameObject.SetActive(false);
		//Destroy(gameObject);
	}

    private void DimSprites(SpriteRenderer[] srs)
    {
		foreach (SpriteRenderer sr in srs)
		{
			sr.color = Color.Lerp(sr.color, new Color(0, 0, 0), 0.3f);
		}
	}

    public void AddPowerup(Powerup powerup)
    {
        // Handle duplicates
        foreach (Powerup p in powerups)
        {
            if (p.name == powerup.name)
            {
                p.ResetTimer();
                return;
            }
        }

        powerup.Start();
        powerups.Add(powerup);

        if (powerup.timed)
        {
            powerup.AddUI(PlayerUIManager.Instance.CreateActivePowerupUI(powerup));
        }
    }

    public void RemovePowerup(Powerup powerup)
    {
        powerup.End();
        powerups.Remove(powerup);
    }

    public void AddShield()
    {
        if (health <= 0) return;

        health++;

        shieldSprite.gameObject.SetActive(true);
    }

    IEnumerator HitInvincibility()
	{
        if (invincible) yield break;

        invincible = true;
        yield return new WaitForSeconds(hitInvincibleDuration);
        invincible = false;
	}

	public void OnCollision(GameObject collision)
    {
        MonoBehaviour[] scripts = collision.GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            if (script is ICollidable)
            {
                (script as ICollidable).OnCollide(this);
            }
        }

        // kill tag for instakills like in levels
        if (collision.CompareTag("Kill")) Die();
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
        OnCollision(collision.gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
        OnCollision(collision.gameObject);
    }

	private void OnEnable()
	{
        GameHandler.Instance.OnGameInit += OnGameInit;
        GameHandler.Instance.OnGamePlay += OnGamePlay;
        GameHandler.Instance.OnGameOver += OnGameOver;
	}

	private void OnDisable()
	{
        GameHandler.Instance.OnGameInit -= OnGameInit;
        GameHandler.Instance.OnGamePlay -= OnGamePlay;
        GameHandler.Instance.OnGameOver -= OnGameOver;
    }
}
