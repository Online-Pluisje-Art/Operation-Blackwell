using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Player {
	public class Weapon : MonoBehaviour {
		[SerializeField] protected float damage_;
		[SerializeField] protected float range_;
		[SerializeField] protected int costs_;
		[SerializeField] protected float baseHitchance_;

		public virtual float GetDamage() {
			return damage_;
		}

		public virtual float GetRange() {
			return range_;
		}
		
		public virtual int GetActionPointsCost() {
			return costs_;
		}

		public virtual float GetBaseHitchance() {
			return baseHitchance_;
		}
	}
}
