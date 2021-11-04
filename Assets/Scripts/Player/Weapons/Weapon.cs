using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Player {
	public class Weapon: MonoBehaviour {
		[SerializeField] protected float damage_;
		[SerializeField] protected float range_;
		[SerializeField] protected int costs_;

		public float GetDamage() {
			return damage_;
		}

		public float GetRange() {
			return range_;
		}
		
		public int GetActionPointsCost() {
			return costs_;
		}
	}
}
