using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Player {
	public class HealthSystem {
		public event EventHandler OnHealthChanged;
		public event EventHandler OnHealthMaxChanged;
		public event EventHandler OnDamaged;
		public event EventHandler OnHealed;
		public event EventHandler OnDead;

		private int healthMax_;
		private int health_;

		public HealthSystem(int healthMax_) {
			this.healthMax_ = healthMax_;
			health_ = healthMax_;
		}

		public int GetHealth() {
			return health_;
		}

		public int GetHealthMax() {
			return healthMax_;
		}

		public float GetHealthNormalized() {
			return (float)health_ / healthMax_;
		}

		public void Damage(int amount) {
			health_ -= amount;
			if(health_ < 0) {
				health_ = 0;
			}
			OnHealthChanged?.Invoke(this, EventArgs.Empty);
			OnDamaged?.Invoke(this, EventArgs.Empty);

			if(health_ <= 0) {
				Die();
			}
		}

		public void Die() {
			OnDead?.Invoke(this, EventArgs.Empty);
		}

		public bool IsDead() {
			return health_ <= 0;
		}

		public void Heal(int amount) {
			health_ += amount;
			if(health_ > healthMax_) {
				health_ = healthMax_;
			}
			OnHealthChanged?.Invoke(this, EventArgs.Empty);
			OnHealed?.Invoke(this, EventArgs.Empty);
		}

		public void HealComplete() {
			health_ = healthMax_;
			OnHealthChanged?.Invoke(this, EventArgs.Empty);
			OnHealed?.Invoke(this, EventArgs.Empty);
		}

		public void SetHealthMax(int healthMax_, bool fullHealth) {
			this.healthMax_ = healthMax_;
			if(fullHealth) {
				health_ = healthMax_;
			}
			OnHealthMaxChanged?.Invoke(this, EventArgs.Empty);
			OnHealthChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
