using UnityEngine;
using System;

namespace OperationBlackwell.Core {
	public abstract class CoreUnit : MonoBehaviour {
		public abstract void MoveTo(Vector3 targetPosition, Vector3 originPosition, Action onReachedPosition);
		public abstract Vector3 GetPosition();
		public abstract Team GetTeam();
		public abstract bool IsEnemy(CoreUnit unitGridCombat);
		public abstract int GetActionPoints();
		public abstract bool HasActionPoints();
		public abstract int GetMaxActionPoints();
		public abstract void SetActionPoints(int actionPoints);
		public abstract void ResetActionPoints();
		public abstract bool CanAttackUnit(CoreUnit unitGridCombat, Vector3 attackPos);
		public abstract void AttackUnit(CoreUnit unitGridCombat, Actions.AttackType type, Action onAttackComplete);
		public abstract bool IsDead();
		public abstract void Damage(CoreUnit attacker, float damageAmount);
		public abstract int GetAttackCost();
		public abstract void SetActiveWeapon(int index);
		public abstract string GetActiveWeapon();
		public abstract Actions.AttackType GetAttackType();
		public abstract void SaveAction(Actions action);
		public abstract WaitingQueue<Actions> LoadActions();
		public abstract void ExecuteActions();
		public abstract void ClearActions();
		public abstract bool HasExecuted();
		public abstract bool IsComplete();
	}
}
