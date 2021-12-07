using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationBlackwell.Core;

namespace OperationBlackwell.Player {
	[CreateAssetMenu(fileName = "New Weapon", menuName = "Player/Weapon")]
	public class Weapon : ScriptableObject {
		[SerializeField] protected List<float> damage_;
		[SerializeField] protected List<int> costs_;
		[SerializeField] protected float range_;
		[SerializeField] protected float baseHitchance_;
		[SerializeField] protected string name_;
		[SerializeField] protected Actions.AttackType type_;
		[SerializeField] protected AudioClip audioClip_;

		public virtual float GetDamage() {
			return Random.Range(damage_[0], damage_[1]);
		}

		public virtual float GetRange() {
			return range_;
		}
		
		public virtual int GetActionPointsCost() {
			return Random.Range(costs_[0], costs_[1]);
		}

		public virtual float GetBaseHitchance() {
			return baseHitchance_;
		}

		public virtual string GetName() {
			return name_;
		}

		public virtual Actions.AttackType GetAttackType() {
			return type_;
		}

		public virtual AudioClip GetAttackSound() {
			return audioClip_;
		}
	}
}
