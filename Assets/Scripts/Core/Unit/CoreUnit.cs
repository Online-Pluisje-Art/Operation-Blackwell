using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OperationBlackwell.Core {
	public abstract class CoreUnit : MonoBehaviour {
		public abstract Task<bool> MoveTo(Vector3 targetPosition, Func<bool> onReachedPosition);
		public abstract Vector3 GetPosition();
		public abstract Team GetTeam();
		public abstract bool IsEnemy(CoreUnit unitGridCombat);
		public abstract int GetActionPoints();
		public abstract bool HasActionPoints();
		public abstract int GetMaxActionPoints();
		public abstract void SetActionPoints(int actionPoints);
		public abstract void ResetActionPoints();
		public abstract bool CanAttackUnit(CoreUnit unitGridCombat);
		public abstract void AttackUnit(CoreUnit unitGridCombat, Action onAttackComplete);
		public abstract bool IsDead();
		public abstract void Damage(CoreUnit attacker, float damageAmount);
		public abstract int GetAttackCost();
		public abstract void SetActiveWeapon(int index);
		public abstract string GetActiveWeapon();
		public abstract void SaveAction(Actions action);
		public abstract List<Actions> LoadActions();
		public abstract void ExecuteActions();
		public abstract void ClearActions();
	}
}