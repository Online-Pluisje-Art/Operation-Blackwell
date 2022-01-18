using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OperationBlackwell.Core;
using OperationBlackwell.Player;

namespace OperationBlackwell.Boss {
	public class Boss : AIUnit {
		[SerializeField][Range(200, 1000)] private int maxHealth_ = 200;

		public event Action<int> BossStageTrigged;

		protected override void Awake() {
			base.Awake();
			healthSystem_.SetHealthMax(maxHealth_, true);
			healthBar_ = null;
		}

		protected override void HealthSystem_OnHealthChanged(object sender, EventArgs e) {
			HealthChanged?.Invoke(this, healthSystem_.GetHealth());
			if(!enabled_) {
				return;
			}
			List<BossStage> stages = BossController.instance.GetStages();
			stages = stages.FindAll(x => x.triggerHealth <= healthSystem_.GetHealth());
			float health = stages.Max(x => x.triggerHealth);
			BossStage stage = stages.Find(x => x.triggerHealth == health);
			BossController.instance.SpawnUnits(stage.ID);
		}
		
		/*	
		 *	This method calculates and returns the hit chance between two Vector3's 
		 *	It is possible to add another float variable WeaponModifierHitChance to this method, then adjust float hitChance accordingly.
		 */
		private float RangedHitChance(Vector3 player, Vector3 target) {
			Grid<Tilemap.Node> grid = GameController.instance.GetGrid();
			List<Vector3> points = Utils.CalculatePoints(player, target);

			// float hitChance = currentWeapon_.GetBaseHitchance();

			// if(currentWeapon_.GetRange() >= points.Count - 1) {
			// 	// Target is in range
			// 	foreach(Vector3 v in points) {
			// 		// Calculates hitchance
			// 		hitChance -= grid.GetGridObject(v).hitChanceModifier;
			// 	}
			// }
			// return hitChance;
			return 0;
		}

		public override void Damage(CoreUnit attacker, float damageAmount) {
			base.Damage(attacker, damageAmount);
			if(healthSystem_.IsDead()) {
				BossController.instance.BossDied();
			}
		}
	}
}
