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

		public void SetSelectedVisible(bool visible) {
			selectedGameObject_.SetActive(visible);
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
	}
}
