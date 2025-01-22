using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Powerups
{
	// should've used scriptable objects, but whatever

    public abstract class Powerup
    {
		public static bool DEBUG = false;

		public string name;

		public float durationSeconds; // set by manager

        public bool timed = true;

		public float timer = 0f;

		public Sprite sprite; // set by manager
		public Vector2 spriteoffset = Vector2.zero;
		public float spriteScale = 1f;

		private ActivePowerupUI activeUI = null;

		private bool over = false;

        // when powerup is applied
        public void Start()
		{
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
			if (activeUI != null) activeUI.timer = 0f;
		}

		public void AddUI(ActivePowerupUI g)
		{
			if (!timed) return;

			activeUI = g;
		}
    }

	public class RecoilPowerup : Powerup
	{
		private float previousRecoil;

		public RecoilPowerup()
		{
			timed = true;
		}

		public override void OnStart()
		{
			previousRecoil = GameHandler.Instance.player.recoilForce;
			GameHandler.Instance.player.recoilForce = previousRecoil * 2;
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
}