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

    [Header("Paramaters")]
    [SerializeField] private int health = 1;
    [SerializeField] private float recoilForce = 5;
    [Tooltip("Coefficient of velocity to conserve when shooting")]
    [SerializeField] private float velocityConservationCoefficient = 0.25f;

    private static readonly float KILL_DISTANCE = 6f;

    private bool invincible = false;
    [SerializeField] private float invincibleDuration = 0.5f;

    [Header("Bullets")]
    [SerializeField] private int ammo = 5;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float bulletLifetime = 2f;

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
        if (TimeManager.Instance.paused) return;

        if (transform.position.y < Camera.main.transform.position.y - KILL_DISTANCE) { Die(); return; }

        foreach (Powerup p in powerups)
		{
            p.Update();
		}

        Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 relative = target - transform.position;
        float angle = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;
        gunAnchor.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (transform.position.x < -5.5f) transform.position = new Vector3(5.4f, transform.position.y, transform.position.z);
        else if (transform.position.x > 5.5f) transform.position = new Vector3(-5.4f, transform.position.y, transform.position.z);
    }

	private void FixedUpdate()
	{
        if (TimeManager.Instance.paused || GameHandler.Instance.state != GameState.PLAY) return;

        if (ControlManager.Controls.game.shoot.WasPressedThisFrame())
		{
            ShootBullet();
		}
	}

    public void AddAmmo(int count)
	{
        ammo += count;
	}

    void ShootBullet()
	{
        //print("shoot");
        Vector2 direction = (((Vector2)transform.position) - (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition)).normalized;
        rb.velocity = direction * recoilForce + rb.velocity * velocityConservationCoefficient;

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

        health --;

        if (health <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(Invincibility());
	}

    public void Die()
	{
        GameHandler.Instance.OnGameOver?.Invoke();
        Camera.main.DOShakePosition(1f, 1f, 20, fadeOut:true);
        Destroy(gameObject);
	}

    IEnumerator Invincibility()
	{
        invincible = true;
        yield return new WaitForSeconds(invincibleDuration);
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
