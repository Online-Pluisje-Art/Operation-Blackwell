using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Player {
	public class MovePositionDirect : MonoBehaviour, IMovePosition {
		private Vector3 movePosition_;

		private void Awake() {
			movePosition_ = transform.position;
		}

		public void SetMovePosition(Vector3 movePosition) {
			this.movePosition_ = movePosition;
		}

		private void Update() {
			Vector3 moveDir = (movePosition_ - transform.position).normalized;
			if(Vector3.Distance(movePosition_, transform.position) < 1f) {
				moveDir = Vector3.zero; // Stop moving when near
			}
			GetComponent<IMoveVelocity>().SetVelocity(moveDir);
		}
	}
}
