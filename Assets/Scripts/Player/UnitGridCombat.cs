using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using OperationBlackwell.Core;

namespace OperationBlackwell.Player {
	[RequireComponent(typeof(AudioSource))]
	public class UnitGridCombat : CoreUnit {
		[SerializeField] private List<Weapon> weapons_;
		[SerializeField] private String name_;
		private MovePositionPathfinding movePosition_;
		
		private WorldBar healthBar_;

		private Weapon currentWeapon_;

		private AudioSource audioSource_;
		private string animatorClipName_ = "";

		private void Awake() {
			movePosition_ = GetComponent<MovePositionPathfinding>();
			audioSource_ = GetComponent<AudioSource>();
			//SetSelectedVisible(false);
			state_ = State.Normal;
			healthSystem_ = new HealthSystem(100);
			healthBar_ = new WorldBar(transform, new Vector3(0, 6.6f), new Vector3(1, .13f), Color.grey, Color.red, 1f, 10000, new WorldBar.Outline { color = Color.black, size = .05f });
			healthSystem_.OnHealthChanged += HealthSystem_OnHealthChanged;
			actions_ = new WaitingQueue<Actions>();
		}

		private void Update() {
			switch(state_) {
				case State.Normal:
					break;
				case State.Moving:
					break;
				case State.Attacking:
					break;
				default: 
					break;
			}
			updateAnimation();
			shouldPlayAttackAnimation_ = false;
		}

		private void OnDestroy() {
			healthSystem_.OnHealthChanged -= HealthSystem_OnHealthChanged;
		}

		private void HealthSystem_OnHealthChanged(object sender, EventArgs e) {
			healthBar_.SetSize(healthSystem_.GetHealthNormalized());
		}

		public override void SetActiveWeapon(int index) {
			if(index >= 0 && index < weapons_.Count) {
				currentWeapon_ = weapons_[index];
			}
			GridCombatSystem.instance.OnWeaponChanged?.Invoke(this, currentWeapon_.GetName());
		}

		public override string GetActiveWeapon() {
			return currentWeapon_.GetName();
		}

		public override Actions.AttackType GetAttackType() {
			return currentWeapon_.GetAttackType();
		}

		public override bool CanAttackUnit(CoreUnit unitGridCombat, Vector3 attackPos) {
			/* 
			 * If the unit is on the same team, return false.
			 * Calculate the distance between the two units. 
			 * The CalculatePoints method is used to calculate the distance between two points, 
			 * when its the same node it returns 1 so we subtract one from the distance to get the actual distance.
			 * If the distance is less or equal than the weapon range, return true.
			 */
			if(unitGridCombat == null || unitGridCombat.GetTeam() == team_ || actionPoints_ <= currentWeapon_.GetMaxCost()) {
				return false;
			}

			// Calculate the distance between the two units. But due to the -1 we can attack diagonal units, but also sometimes 1 node extra on the range.
			int nodesBetweenPlayers = Utils.CalculatePoints(attackPos, unitGridCombat.GetPosition()).Count - 1;
			return nodesBetweenPlayers <= currentWeapon_.GetRange();
		}

		public override void MoveTo(Vector3 targetPosition, Vector3 originPosition, Action onReachedPosition) {
			state_ = State.Moving;
			movePosition_.SetMovePosition(targetPosition, originPosition, () => {
				state_ = State.Normal;
				onReachedPosition();
			});
		}
		
		public override void AttackUnit(CoreUnit unitGridCombat, Actions.AttackType type, Action onAttackComplete) {
			state_ = State.Attacking;

			StartCoroutine(ShootUnit(unitGridCombat, type, () => {
				onAttackComplete();
			}));
		}

		IEnumerator ShootUnit(CoreUnit unitGridCombat, Actions.AttackType type, Action onShootComplete) {
			GetComponent<IMoveVelocity>().Disable();

			Weapon weapon = weapons_.Find(weapon => weapon.GetAttackType() == type);

			if(unitGridCombat.IsDead()) {
				state_ = State.Normal;
				onShootComplete();
				GetComponent<IMoveVelocity>().Enable();
				yield break;
			}
			DetermineAttackAnimation(unitGridCombat);
			audioSource_.clip = weapon.GetAttackSound();
			// Get active animation duration unity
			if(audioSource_.clip != null) {
				audioSource_.Play();
			}
			yield return null;
			float time = 0.0f;
			RuntimeAnimatorController ac = animator_.runtimeAnimatorController;
			for(int i = 0; i < ac.animationClips.Length; i++) {
				if(ac.animationClips[i].name == animatorClipName_) {
					time = ac.animationClips[i].length;
					break;
				}
			}
			// The 0.15f has been found through meticulous testing.
			yield return new WaitForSeconds(time + 0.15f);
			unitGridCombat.Damage(this, weapon.GetDamage());
			state_ = State.Normal;
			onShootComplete();

			GetComponent<IMoveVelocity>().Enable();
		}

		[ContextMenu("Test")]
		public void Die() {
			healthSystem_.Damage(100);
			GridCombatSystem.instance.OnUnitDeath?.Invoke(this, EventArgs.Empty);
			Destroy(gameObject);
		}

		public override bool IsDead() {
			return healthSystem_.IsDead();
		}

		public override int GetAttackCost() {
			return currentWeapon_.GetActionPointsCost();
		}

		/*	
		 *	This method calculates and returns the hit chance between two Vector3's 
		 *	It is possible to add another float variable WeaponModifierHitChance to this method, then adjust float hitChance accordingly.
		 */
		private float RangedHitChance(Vector3 player, Vector3 target) {
			Grid<Tilemap.Node> grid = GameController.instance.GetGrid();
			List<Vector3> points = Utils.CalculatePoints(player, target);

			float hitChance = currentWeapon_.GetBaseHitchance();

			if(currentWeapon_.GetRange() >= points.Count - 1) {
				// Target is in range
				foreach(Vector3 v in points) {
					// Calculates hitchance
					hitChance -= grid.GetGridObject(v).hitChanceModifier;
				}
			}
			return hitChance;
		}

		public override void ExecuteActions() {
			StartCoroutine(ExecuteActionsCoroutine());
			hasExecuted_ = true;
			isComplete_ = false;
		}

		IEnumerator ExecuteActionsCoroutine() {
			bool hasExecuted = false;
			bool isComplete = false;
			while(!actions_.IsEmpty()) {
				hasExecuted = actions_.Peek().HasExecuted();
				isComplete = actions_.Peek().IsComplete();
				if(!hasExecuted) {
					actions_.Peek().Execute();
				} 
				if(isComplete) {
					actions_.Dequeue();
				}
				yield return null;
			}
			isComplete_ = true;
			ClearActions();
		}

		private void updateAnimation() {
			if(animator_ == null) {
				return;
			}

			if(shouldPlayAttackAnimation_) {
				ResetAllAttackAnimations();
				animatorClipName_ = "";
				if(currentWeapon_.GetRange() > 1) {
					switch(direction_) {
						case Direction.Up:
							animator_.SetBool("isShootingUp", true);
							animatorClipName_ = "ShootingUpAnimation";
							break;
						case Direction.Down:
							animator_.SetBool("isShootingDown", true);
							animatorClipName_ = "ShootingDownAnimation";
							break;
						case Direction.Left:
							animator_.SetBool("isShootingLeft", true);
							animatorClipName_ = "ShootingLeftAnimation";
							break;
						case Direction.Right:
							animator_.SetBool("isShootingRight", true);
							animatorClipName_ = "ShootingRightAnimation";
							break;
						default:
							// Oops, something went wrong, just return.
							return;
					}
				} else {
					switch(direction_) {
						case Direction.Up:
							animator_.SetBool("isMeleeUp", true);
							animatorClipName_ = "MeleeUpAnimation";
							break;
						case Direction.Down:
							animator_.SetBool("isMeleeDown", true);
							animatorClipName_ = "MeleeDownAnimation";
							break;
						case Direction.Left:
							animator_.SetBool("isMeleeLeft", true);
							animatorClipName_ = "MeleeLeftAnimation";
							break;
						case Direction.Right:
							animator_.SetBool("isMeleeRight", true);
							animatorClipName_ = "MeleeRightAnimation";
							break;
						default:
							// Oops, something went wrong, just return.
							return;
					}
				}
			} else {
				// Reset all animation states, not attacking.
				ResetAllAttackAnimations();
				direction_ = Direction.Null;
			}
		}

		private void ResetAllAttackAnimations() {
			animator_.SetBool("isMeleeUp", false);
			animator_.SetBool("isMeleeDown", false);
			animator_.SetBool("isMeleeLeft", false);
			animator_.SetBool("isMeleeRight", false);
			animator_.SetBool("isShootingUp", false);
			animator_.SetBool("isShootingDown", false);
			animator_.SetBool("isShootingLeft", false);
			animator_.SetBool("isShootingRight", false);
		}

		public override String GetName() {
			return name_;
		}
	}
}
