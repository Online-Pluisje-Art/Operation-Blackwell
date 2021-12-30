using UnityEngine;
using System;

namespace OperationBlackwell.Core {
	public abstract class CoreUnit : MonoBehaviour {
		[SerializeField] protected Team team_;
		[SerializeField] protected int maxActionPoints_;
		[SerializeField] protected Animator animator_;
		protected int actionPoints_;
		protected State state_;

		protected HealthSystem healthSystem_;

		protected enum State {
			Normal,
			Moving,
			Attacking
		}

		protected WaitingQueue<Actions> actions_;
		protected bool hasExecuted_;
		protected bool isComplete_;

		

		protected void Start() {
			GameController.instance.GetGrid().GetGridObject(transform.position).SetUnitGridCombat(this);
			ResetActionPoints();
			SetActiveWeapon(0);
		}

		public virtual bool IsDead() {
			return healthSystem_.IsDead();
		}

		public virtual Vector3 GetPosition() {
			return transform.position;
		}
		public virtual Team GetTeam() {
			return team_;
		}

		public virtual int GetActionPoints() {
			return actionPoints_;
		}

		public virtual bool HasActionPoints() {
			return actionPoints_ > 0;
		}

		public virtual int GetMaxActionPoints() {
			return maxActionPoints_;
		}

		public virtual void SetActionPoints(int actionPoints) {
			actionPoints_ = actionPoints;
		}

		public virtual void ResetActionPoints() {
			actionPoints_ = maxActionPoints_;
		}

		public virtual void SaveAction(Actions action) {
			actionPoints_ -= action.cost;
			actions_.Enqueue(action);
		}

		public virtual WaitingQueue<Actions> LoadActions() {
			return actions_;
		}

		public virtual int GetActionCount() {
			return actions_.Count();
		}

		public virtual void ClearActions() {
			actions_.Clear();
			ResetActionPoints();
		}
		public virtual bool HasExecuted() {
			return hasExecuted_;
		}
		public virtual bool IsComplete() {
			return isComplete_;
		}
		public virtual void ResetComplete() {
			isComplete_ = false;
			hasExecuted_ = false;
		}

		public virtual bool IsEnemy(CoreUnit unitGridCombat) {
			return unitGridCombat.GetTeam() != team_;
		}

		public virtual void Damage(CoreUnit attacker, float damageAmount) {	
			healthSystem_.Damage((int)damageAmount);
			if(healthSystem_.IsDead()) {
				GridCombatSystem.Instance.OnUnitDeath?.Invoke(this, EventArgs.Empty);
				Destroy(gameObject);
			}
		}

		public abstract void MoveTo(Vector3 targetPosition, Vector3 originPosition, Action onReachedPosition);
		public abstract bool CanAttackUnit(CoreUnit unitGridCombat, Vector3 attackPos);
		public abstract void AttackUnit(CoreUnit unitGridCombat, Actions.AttackType type, Action onAttackComplete);
		public abstract int GetAttackCost();
		public abstract void SetActiveWeapon(int index);
		public abstract string GetActiveWeapon();
		public abstract Actions.AttackType GetAttackType();
		public abstract void ExecuteActions();
		public abstract String GetName();
	}
}
