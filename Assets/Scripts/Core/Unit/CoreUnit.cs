using UnityEngine;
using System;

namespace OperationBlackwell.Core {
	[RequireComponent(typeof(AudioSource))]
	public abstract class CoreUnit : MonoBehaviour {
		[SerializeField] protected String name_;
		[SerializeField] protected Team team_;
		[SerializeField] protected int maxActionPoints_;
		[SerializeField] protected Animator animator_;
		protected int actionPoints_;
		protected State state_;

		protected HealthSystem healthSystem_;

		protected AudioSource audioSource_;

		protected enum State {
			Normal,
			Moving,
			Attacking
		}

		protected enum Direction {
			Up,
			Down,
			Left,
			Right,
			Null
		}

		protected Direction direction_;
		protected bool shouldPlayAttackAnimation_;

		protected WaitingQueue<Actions> actions_;
		protected bool hasExecuted_;
		protected bool isComplete_;

		public EventHandler<float> HealthChanged;

		protected virtual void Awake() {
			audioSource_ = GetComponent<AudioSource>();
			//SetSelectedVisible(false);
			state_ = State.Normal;
			healthSystem_ = new HealthSystem(100);
			healthSystem_.OnHealthChanged += HealthSystem_OnHealthChanged;
			actions_ = new WaitingQueue<Actions>();
		}		

		protected void Start() {
			GameController.instance.GetGrid().GetGridObject(transform.position).SetUnitGridCombat(this);
			direction_ = Direction.Null;
			shouldPlayAttackAnimation_ = false;
			ResetActionPoints();
			SetActiveWeapon(0);
		}

		protected void OnDestroy() {
			healthSystem_.OnHealthChanged -= HealthSystem_OnHealthChanged;
		}

		public virtual String GetName() {
			return name_;
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
				animator_.SetBool("isDead", true);
				GridCombatSystem.instance.OnUnitDeath?.Invoke(this, EventArgs.Empty);
				Destroy(gameObject, 1.4f);
			}
		}

		public virtual void Die() {
			healthSystem_.Damage((int)healthSystem_.GetHealthMax());
			if(healthSystem_.IsDead()) {
				GridCombatSystem.instance.OnUnitDeath?.Invoke(this, EventArgs.Empty);
				Destroy(gameObject);
			}
		}

		protected void DetermineAttackAnimation(CoreUnit unitGridCombat) {
			shouldPlayAttackAnimation_ = true;
			Vector3 attackerPosition = this.GetPosition();
			Vector3 enemyPosition = unitGridCombat.GetPosition();
			direction_ = Direction.Null;
			int diffX = 0;
			int diffY = 0;
			// 0 is neither, 1 is yes, 2 is false.
			int isBelow = 0;
			int isLeft = 0;
			if(attackerPosition.x == enemyPosition.x) {
				// Either above or below us.
			} else if(attackerPosition.x > enemyPosition.x) {
				// Left of us.
				diffX = (int)(attackerPosition.x - enemyPosition.x);
				isLeft = 1;
			} else {
				// Right of us.
				diffX = (int)(enemyPosition.x - attackerPosition.x);
				isLeft = 2;
			}
			if(attackerPosition.y == enemyPosition.y) {
				// Either left or right of us.
			} else if(attackerPosition.y > enemyPosition.y) {
				// Below us.
				diffY = (int)(attackerPosition.y - enemyPosition.y);
				isBelow = 1;
			} else {
				// Above us.
				diffY = (int)(enemyPosition.y - attackerPosition.y);
				isBelow = 2;
			}
			// Depending on which diff is bigger, trigger the right direction.
			if(diffX > diffY) {
				// Left/Right is bigger than up/down, use left/right.
				if(isLeft == 1) {
					// Left it is!
					direction_ = Direction.Left;
				} else if(isLeft == 2) {
					// Right it is!
					direction_ = Direction.Right;
				} else {
					// Should never happen, signal an error.
					direction_ = Direction.Null;
				}
			} else if(diffX <= diffY) {
				// Up/down is bigger or equal to left/right, use up/down.
				if(isBelow == 1) {
					// Below it is!
					direction_ = Direction.Down;
				} else if(isBelow == 2) {
					// Up it is!
					direction_ = Direction.Up;
				} else {
					// Should never happen, signal an error.
					direction_ = Direction.Null;
				}
			} else {
				// Should never happen, signal an error.
				direction_ = Direction.Null;
			}
		}

		public virtual int GetHealth() {
			return healthSystem_.GetHealth();
		}

		public abstract void MoveTo(Vector3 targetPosition, Vector3 originPosition, Action onReachedPosition);
		public abstract bool CanAttackUnit(CoreUnit unitGridCombat, Vector3 attackPos);
		public abstract void AttackUnit(CoreUnit unitGridCombat, Actions.AttackType type, Action onAttackComplete);
		public abstract int GetAttackCost();
		public abstract void SetActiveWeapon(int index);
		public abstract string GetActiveWeapon();
		public abstract Actions.AttackType GetAttackType();
		public abstract void ExecuteActions();
		protected abstract void HealthSystem_OnHealthChanged(object sender, EventArgs e);
	}
}
