using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationBlackwell.Core;

namespace OperationBlackwell.Player {
	public class UnitGridCombat : CoreUnit {

		[SerializeField] private Team team_;
		[SerializeField] private int actionPoints_;
		[SerializeField] private int maxActionPoints_;
		
		private PlayerBase characterBase_;
		private GameObject selectedGameObject_;
		private MovePositionPathfinding movePosition_;
		private State state_;
		private HealthSystem healthSystem_;

		private enum State {
			Normal,
			Moving,
			Attacking
		}

		private void Awake() {
			characterBase_ = GetComponent<PlayerBase>();
			selectedGameObject_ = transform.Find("Selected").gameObject;
			movePosition_ = GetComponent<MovePositionPathfinding>();
			//SetSelectedVisible(false);
			state_ = State.Normal;
			healthSystem_ = new HealthSystem(100);
			healthSystem_.OnHealthChanged += HealthSystem_OnHealthChanged;
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
		}

		private void HealthSystem_OnHealthChanged(object sender, EventArgs e) {
			// healthBar.SetSize(healthSystem.GetHealthNormalized());
		}

		public void SetSelectedVisible(bool visible) {
			selectedGameObject_.SetActive(visible);
		}

		public override bool CanAttackUnit(CoreUnit unitGridCombat) {
			return Vector3.Distance(GetPosition(), unitGridCombat.GetPosition()) < 50f;
		}

		public override void MoveTo(Vector3 targetPosition, Action onReachedPosition) {
			state_ = State.Moving;
			movePosition_.SetMovePosition(targetPosition, () => {
				state_ = State.Normal;
				onReachedPosition();
			});
		}

		public override Vector3 GetPosition() {
			return transform.position;
		}

		public override Team GetTeam() {
			return team_;
		}

		public override bool IsEnemy(CoreUnit unitGridCombat) {
			return unitGridCombat.GetTeam() != team_;
		}

		public override void SetActionPoints(int actionPoints) {
			actionPoints_ = actionPoints;
		}

		public override int GetActionPoints() {
			return actionPoints_;
		}

		public override void ResetActionPoints() {
			actionPoints_ = maxActionPoints_;
		}

		public override void AttackUnit(CoreUnit unitGridCombat, Action onAttackComplete) {
			state_ = State.Attacking;

			ShootUnit(unitGridCombat, () => {
				if (!unitGridCombat.IsDead()) {
					ShootUnit(unitGridCombat, () => {
						if (!unitGridCombat.IsDead()) {
							ShootUnit(unitGridCombat, () => {
								if (!unitGridCombat.IsDead()) {
									ShootUnit(unitGridCombat, () => {
										state_ = State.Normal;
										onAttackComplete();
									});
								} else { 
									state_ = State.Normal; 
									onAttackComplete(); 
								}
							});
						} else { 
							state_ = State.Normal; 
							onAttackComplete(); 
						}
					});
				} else { 
					state_ = State.Normal; 
					onAttackComplete(); 
				}
			});
		}

		private void ShootUnit(CoreUnit unitGridCombat, Action onShootComplete) {
			GetComponent<IMoveVelocity>().Disable();
			Vector3 attackDir = (unitGridCombat.GetPosition() - transform.position).normalized;

			// characterBase_.PlayShootAnimation(attackDir, (Vector3 vec) => {
			// 	Shoot_Flash.AddFlash(vec);
			// 	WeaponTracer.Create(vec, unitGridCombat.GetPosition() + UtilsClass.GetRandomDir() * UnityEngine.Random.Range(-2f, 4f));
			// 	unitGridCombat.Damage(this, UnityEngine.Random.Range(4, 12));
			// }, () => {
			// 	characterBase.PlayIdleAnim();
			// 	GetComponent<IMoveVelocity>().Enable();

			onShootComplete();
			// });
		}

		public void Damage(CoreUnit attacker, int damageAmount) {
			Vector3 bloodDir = (GetPosition() - attacker.GetPosition()).normalized;
			
			healthSystem_.Damage(damageAmount);
			if (healthSystem_.IsDead()) {
				Destroy(gameObject);
			} else {
				// Knockback
				//transform.position += bloodDir * 5f;
			}
		}

		public override bool IsDead() {
			return healthSystem_.IsDead();
		}
	}
}
