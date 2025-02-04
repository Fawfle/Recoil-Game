using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Powerups
{
	// should've used scriptable objects, but whatever
	//[System.Serializable]
	public abstract class Powerup
	{
		public static bool DEBUG = false;

		public string name;

		public float durationSeconds; // set by manager

		public bool timed = true;

		public float timer = 0f;

		public Sprite sprite; // set by manager
		public Vector2 spriteOffset = Vector2.zero;
		public float spriteScale = 1f;
		public float spriteRotation = 0f;

		public bool uiEnabled = true;

		private List<PowerupUI> activeUI = new();

		private bool over = false;

		// when powerup is applied
		public void Start()
		{
			over = false;
			ResetTimer();
			OnStart();
		}
		public abstract void OnStart(); // hook
										// when powerup is active, returns false if exit
		public bool Update()
		{
			if (DEBUG) Debug.Log(name + " Powerup Update");
			if (ExitCondition()) { End(); return false; }

			if (timed)
			{
				timer += Time.deltaTime;
				if (timer >= durationSeconds)
				{
					End();
					return false;
				}
			}

			return true;
		}
		// when powerup ends
		public void End()
		{
			if (over) return;
			over = true;
			if (DEBUG) Debug.Log(name + " Powerup Over");
			OnEnd();
		}
		public virtual void OnEnd() { } // hook

		// optional end condition
		public virtual bool ExitCondition()
		{
			return false;
		}

		public void ResetTimer()
		{
			timer = 0f;

			foreach (PowerupUI ui in activeUI)
			{
				if (ui == null) continue;
				ui.progress = 0;
			}
		}

		public void AddUI(PowerupUI g)
		{
			if (!uiEnabled) return;

			activeUI.Add(g);
		}

		public List<PowerupUI> GetUI()
		{
			return activeUI;
		}
	}

	public class RecoilPowerup : Powerup
	{
		private float previousRecoil;

		public static readonly float RECOIL_MULTIPLIER = 1.5f;

		public RecoilPowerup()
		{
			timed = true;
		}

		public override void OnStart()
		{
			previousRecoil = GameHandler.Instance.player.recoilForce;
			GameHandler.Instance.player.recoilForce = previousRecoil * RECOIL_MULTIPLIER;
		}

		public override void OnEnd()
		{
			GameHandler.Instance.player.recoilForce = previousRecoil;
		}
	}

	public class InfiniteAmmoPowerup : Powerup
	{
		public InfiniteAmmoPowerup()
		{
			timed = true;
		}

		public override void OnStart()
		{
			GameHandler.Instance.player.ammoEnabled = false;
			GameHandler.Instance.player.RefillAmmo();
		}

		public override void OnEnd()
		{
			GameHandler.Instance.player.ammoEnabled = true;
		}
	}

	public class ShieldPowerup : Powerup
	{
		public ShieldPowerup()
		{
			timed = false;
			uiEnabled = false;
		}

		public override void OnStart()
		{
			if (GameHandler.Instance.player.health != 1) return;

			GameHandler.Instance.player.AddShield();
		}

		public override bool ExitCondition()
		{
			return true;
		}
	}

	public class ShotgunPowerup : Powerup
	{
		private float previousRecoil;

		private int maxShots = 3;
		private int shotsLeft = 3;

		public static readonly float RECOIL_MULTIPLIER = 2f;

		public ShotgunPowerup()
		{
			timed = false;
		}

		public override void OnStart()
		{
			previousRecoil = GameHandler.Instance.player.recoilForce;
			GameHandler.Instance.player.recoilForce = previousRecoil * RECOIL_MULTIPLIER;

			GameHandler.Instance.player.OnShoot += OnPlayerShoot;
		}

		public override void OnEnd()
		{
			GameHandler.Instance.player.recoilForce = previousRecoil;

			GameHandler.Instance.player.OnShoot -= OnPlayerShoot;

			foreach (PowerupUI p in GetUI())
			{
				GameObject.Destroy(p.gameObject);
			}
		}

		private void OnPlayerShoot()
		{
			shotsLeft--;

			foreach (PowerupUI p in GetUI())
			{
				p.progress = (maxShots - shotsLeft) / (float)maxShots;
			}
		}

		public override bool ExitCondition()
		{
			return shotsLeft <= 0;
		}
	}
}