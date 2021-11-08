using UnityEngine;

namespace OperationBlackwell.Player {
	public class Sword : Weapon {
		public override float GetDamage() {
			return Random.Range(damage_ - 30, damage_);
		}
	}
}
