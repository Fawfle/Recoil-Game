using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Powerups
{
    public abstract class Powerup
    {
        public float durationSeconds;
        public float timer;

        // when powerup is applied
        public virtual void Start()
		{
            timer = durationSeconds;
		}
        // when powerup is active
        public virtual void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0) End();
        }
        // when powerup ends
        public abstract void End();
    }

    public class RecoilPowerup: Powerup
	{
        public override void Start()
		{
            durationSeconds = 2f;
        }

		public override void End()
		{
			throw new System.NotImplementedException();
		}
	}
}