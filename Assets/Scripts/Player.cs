using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Powerups;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	private CircleCollider2D coll;
	public Rigidbody2D rb { get; private set; }

	[Header("Components")]
	[SerializeField] private Transform gunAnchor;
	[SerializeField] private Transform gun;
	[SerializeField] private Bullet bulletPrefab;
	[SerializeField] private Transform bulletContainer;

	[SerializeField] private SpriteRenderer shieldSprite;
	[SerializeField] public Transform powerupTimerContainer;

	[SerializeField] private SkinManager skinManager;
	[SerializeField] private SpriteRenderer bodySr;
	[SerializeField] private SpriteRenderer outlineSr;
	[SerializeField] private SpriteRenderer gunSr;
	[SerializeField] private Image crownImage;

	[Header("Paramaters")]
	[SerializeField] public int health = 1;
	[SerializeField] public float recoilForce = 5;
	[Tooltip("Coefficient of velocity to conserve when shooting")]
	[SerializeField] private float velocityConservationCoefficient = 0.25f;

	private static readonly float ENDLESS_KILL_DISTANCE = 6f; // distance to kill player below top line (0.5f below viewport of 5.5f)
	private static readonly float LEVEL_KILL_DISTANCE_SIZE = 1f; // distance to kill player outside of level bounds (0.5f outside bounds)

	private static readonly float LEVEL_WON_LERP_SPEED = 1f;

	private bool invincible = false;
	private float hitInvincibleDuration = 0f;

	[Header("Bullets")]
	[SerializeField] public bool ammoEnabled = false;
	[SerializeField] public int maxAmmo = 6;
	[HideInInspector] public int ammo;
	[SerializeField] private float bulletSpeed = 5f;
	[SerializeField] private float bulletLifetime = 20f;

	[SerializeField] private bool firstShotSuper = true;

	private List<Powerup> powerups = new();

	//[SerializeField] private Transform bulletUIContainer;
	//[SerializeField] private SpriteRenderer bulletUIPrefab;
	//private List<SpriteRenderer> bulletUIList = new();

	private static readonly float SHIELD_SAVE_VELOCITY = 7f;
	private static readonly float SHOTGUN_BULLET_ANGLE_OFFSET = 25f;

	private static readonly bool ENDLESS_BULLET_WRAP = true;

	public event Action OnShoot;

	private void Awake()
	{
		coll = GetComponent<CircleCollider2D>();
		rb = GetComponent<Rigidbody2D>();

		ammo = maxAmmo;
	}

	private void Start()
	{
		LoadSkin(skinManager.GetSkin(SaveManager.save.playerSkin));
		LoadPlayerColor();
		if (SaveManager.save.leaderboardCrownEnabled && SaveManager.IsOnLeaderboardPodium())
		{
			crownImage.gameObject.SetActive(true);
			crownImage.color = SaveManager.save.currentLeaderboardRank == 0 ? LeaderboardManager.FIRST_COLOR : SaveManager.save.currentLeaderboardRank == 1 ? LeaderboardManager.SECOND_COLOR : LeaderboardManager.THIRD_COLOR;
			crownImage.CrossFadeAlpha(0.8f, 0f, true);
		}
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
	private void OnGameEnd()
	{
		rb.freezeRotation = false;
		coll.enabled = false;
	}
	

	private void Update()
	{
		if (TimeManager.Instance.paused || GameHandler.Instance.IsEndState()) return;
		//UpdateBulletUI();

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


		Vector3 target = Camera.main.ScreenToWorldPoint(ControlManager.Controls.game.mouse.ReadValue<Vector2>());
		Vector2 relative = target - transform.position;
		float angle = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;
		gunAnchor.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

		if (GameHandler.Instance.IsGameMode(GameMode.Endless))
		{
			float wrapBounds = EndlessLevelManager.LEVEL_BOUNDS + coll.radius;
			if (transform.position.x < -wrapBounds) transform.position = new Vector3(wrapBounds - 0.01f, transform.position.y, transform.position.z);
			else if (transform.position.x > wrapBounds) transform.position = new Vector3(-wrapBounds + 0.01f, transform.position.y, transform.position.z);
		}
	}

	private void FixedUpdate()
	{
		if (TimeManager.Instance.paused) return;

		if (GameHandler.Instance.IsGameMode(GameMode.Endless)) EndlessFixedUpdate();
		else if (GameHandler.Instance.IsGameMode(GameMode.Level)) LevelFixedUpdate();

		if (!GameHandler.Instance.IsState(GameState.Play)) return;

		if (ControlManager.WasShootPressedThisFrame() && (!ammoEnabled || ammo > 0))
		{
			ShootBullet();
		}

		if (ControlManager.Controls.game.restart.WasPressedThisFrame())
		{
			ControlManager.Controls.game.restart.Reset();
			Die();
		}
	}

	private void EndlessFixedUpdate()
	{
		// damp player velocity on death, cosmetic
		if (GameHandler.Instance.IsState(GameState.Over))
		{
			if (transform.position.y < Camera.main.transform.position.y - (ENDLESS_KILL_DISTANCE * 2f))
			{
				rb.velocity *= 0.99f;
				rb.gravityScale = 0f;
			}
			return;
		}

		if (transform.position.y < Camera.main.transform.position.y - ENDLESS_KILL_DISTANCE)
		{
			// save if shielded
			if (health > 1)
			{
				TakeDamage();
				rb.velocity = new Vector2(rb.velocity.x, SHIELD_SAVE_VELOCITY);
			}
			else
			{
				Die();
				return;
			}
		}
	}

	private void LevelFixedUpdate()
	{
		if (GameHandler.Instance.IsState(GameState.Over))
		{
			if (!LevelManager.Instance.IsInBounds(transform.position, LEVEL_KILL_DISTANCE_SIZE * Vector2.one)) 
			{
				rb.gravityScale = 0f;

				rb.velocity *= 0.99f;
			}
			return;
		} 
		else if (GameHandler.Instance.IsState(GameState.LevelComplete))
		{
			rb.gravityScale = 0f;

			rb.velocity *= 0.98f;
			rb.velocity = Vector3.Lerp(rb.velocity, (LevelManager.Instance.goal.transform.position - transform.position) * 10, LEVEL_WON_LERP_SPEED * Time.deltaTime);
			return;
		}

		if (!LevelManager.Instance.IsInBounds(transform.position, LEVEL_KILL_DISTANCE_SIZE * Vector2.one))
		{
			// shield bounce
			if (health > 1f)
			{
				TakeDamage();
				if (transform.position.x + LEVEL_KILL_DISTANCE_SIZE / 2f < LevelManager.Instance.bounds.xMin) rb.velocity = new Vector2(SHIELD_SAVE_VELOCITY, rb.velocity.y);
				if (transform.position.x - LEVEL_KILL_DISTANCE_SIZE / 2f > LevelManager.Instance.bounds.xMax) rb.velocity = new Vector2(-SHIELD_SAVE_VELOCITY, rb.velocity.y);
				if (transform.position.y + LEVEL_KILL_DISTANCE_SIZE / 2f < LevelManager.Instance.bounds.yMin) rb.velocity = new Vector2(rb.velocity.x, SHIELD_SAVE_VELOCITY);
				if (transform.position.y - LEVEL_KILL_DISTANCE_SIZE / 2f > LevelManager.Instance.bounds.yMax) rb.velocity = new Vector2(rb.velocity.x, -SHIELD_SAVE_VELOCITY);
				return;
			}

			Die();
			return;
		}
	}

	public void AddAmmo(int count)
	{
		// unused, caused audio bug and not sure its a big deal (overpolish)
		//AudioManager.PlaySoundGroup("GetAmmo", 0.2f);

		ammo = Mathf.Min((ammo + count), maxAmmo);
	}

	public void RefillAmmo()
	{
		AudioManager.PlaySoundGroup("RefillAmmo");

		ammo = maxAmmo;
	}

	void ShootBullet()
	{
		//print(ControlManager.Controls.game.mouse.ReadValue<Vector2>());
		//print(Input.mousePosition);

		Vector2 direction = (((Vector2)transform.position) - (Vector2)Camera.main.ScreenToWorldPoint(ControlManager.Controls.game.mouse.ReadValue<Vector2>())).normalized;
		rb.velocity = direction * recoilForce + rb.velocity * velocityConservationCoefficient;

		// first shot is "super"
		if (firstShotSuper)
		{
			rb.velocity *= 2;
			firstShotSuper = false;
		}

		if (ammoEnabled) ammo--;

		AudioManager.PlaySoundGroup("Shoot");

		SaveManager.IncrementShotsFired();

		CreateBullet(gunAnchor.rotation);

		if (HasPowerup(typeof(ShotgunPowerup)))
		{
			CreateBullet(gunAnchor.rotation * Quaternion.Euler(0, 0, SHOTGUN_BULLET_ANGLE_OFFSET));
			CreateBullet(gunAnchor.rotation * Quaternion.Euler(0, 0, -SHOTGUN_BULLET_ANGLE_OFFSET));
		}

		rb.angularVelocity = rb.angularVelocity * 0.25f + -Mathf.Sign(direction.x) * recoilForce * 25f;

		OnShoot?.Invoke();
	}

	void CreateBullet(Quaternion rotation)
	{
		Bullet bullet = Instantiate(bulletPrefab, gun.transform.position, rotation);
		bullet.transform.SetParent(bulletContainer);
		bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * bulletSpeed;
		Destroy(bullet, bulletLifetime);

		if (GameHandler.Instance.IsGameMode(GameMode.Endless)) bullet.wrap = ENDLESS_BULLET_WRAP;
	}

	public void TakeDamage()
	{
		if (invincible) return;

		health--;

		if (shieldSprite.gameObject.activeSelf)
		{
			shieldSprite.gameObject.SetActive(false);

			AudioManager.PlaySoundGroup("ShieldHit");
			ParticleManager.CreateParticleSystem("Shield", transform.position, transform.parent, true);
		}

		if (health <= 0)
		{
			Die();
			return;
		}

		if (hitInvincibleDuration > 0f) StartCoroutine(HitInvincibility());
	}

	public void Die()
	{
		if (!GameHandler.Instance.IsState(GameState.Play)) return;

		SaveManager.IncrementDeaths();

		GameHandler.Instance.SetState(GameState.Over);

		if (!DOTween.IsTweening(Camera.main)) DOTween.Complete(Camera.main);
		Camera.main.DOShakePosition(1f, 1f, 20, fadeOut: true);

		//DimSprites(GetComponents<SpriteRenderer>());
		DimSprites(GetComponentsInChildren<SpriteRenderer>());

		rb.AddTorque(25f * -Mathf.Sign(rb.velocity.x) * rb.velocity.magnitude);
		//rb.AddTorque(25f * Vector2.Dot(rb.velocity, Vector2.left));

		ParticleManager.CreateParticleSystem("PlayerDeathBurst", transform.position, transform, true);
		ParticleManager.CreateParticleSystem("Flames", transform.position, transform, false);

		AudioManager.PlaySoundGroup("Die", 0.3f);

		//gameObject.SetActive(false);
		//Destroy(gameObject);
	}

	private void DimSprites(SpriteRenderer[] srs)
	{
		foreach (SpriteRenderer sr in srs)
		{
			sr.color = UnityEngine.Color.Lerp(sr.color, new UnityEngine.Color(0, 0, 0), 0.3f);
		}
	}

	private void LoadSkin(PlayerSkin skin)
	{
		bodySr.sprite = skin.bodySprite;
		bodySr.transform.localScale = skin.bodyScale * Vector3.one;

		outlineSr.sprite = skin.bodySprite;
		outlineSr.transform.localScale = skin.outlineScale * Vector3.one;

		gunSr.sprite = skin.gunSprite;
		gunSr.transform.localScale = skin.gunScale * Vector3.one;
		gunSr.size = skin.gunSize;

		foreach (Transform child in outlineSr.transform)
		{
			Destroy(child.gameObject);
		}

		// create new sprites under outline with offsets
		if (skin.outlineOffset != 0f)
		{
			Vector2[] offsets = { new(skin.outlineOffset, 0), new(0, skin.outlineOffset), new(skin.outlineOffset, skin.outlineOffset), new(-skin.outlineOffset, skin.outlineOffset) };

			for (int i = 0; i < offsets.Length; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					GameObject g = new GameObject("Outline " + (i * 2 + j));
					var sr = g.AddComponent<SpriteRenderer>();

					g.transform.SetParent(outlineSr.transform);
					g.transform.localScale = Vector3.one;

					g.transform.localPosition = offsets[i] * (j == 0 ? 1 : -1);

					sr.color = outlineSr.color;
					sr.sprite = outlineSr.sprite;
				}
			}
		}
	}

	private void LoadPlayerColor()
	{
		gunSr.color = SaveManager.save.playerColor;
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

		if (powerup.uiEnabled)
		{
			PlayerUIManager.Instance.AddPowerupUI(powerup);
		}
	}

	public void RemovePowerup(Powerup powerup)
	{
		powerup.End();
		powerups.Remove(powerup);
	}

	public bool HasPowerup(Type powerupType)
	{
		foreach (Powerup p in powerups)
		{
			if (p.GetType() == powerupType) return true;
		}

		return false;
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

		else if (collision.CompareTag("Goal"))
		{
			GameHandler.Instance.SetState(GameState.LevelComplete);
			rb.angularDrag = 1f;
		}
	}

	/*
    private void UpdateBulletUI()
    {
        for (int i = 0; i < bulletUIList.Count; i++)
		{
			bulletUIList[i].color = (i < ammo) ? Color.yellow : new Color(0.1f, 0.1f, 0.1f, bulletUIList[i].color.a);
		}
    }
    */

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
		GameHandler.Instance.OnGameEnd += OnGameEnd;
	}

	private void OnDisable()
	{
		GameHandler.Instance.OnGameInit -= OnGameInit;
		GameHandler.Instance.OnGamePlay -= OnGamePlay;
		GameHandler.Instance.OnGameEnd -= OnGameEnd;
	}
}
