using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Player {
	public class MoveTransformVelocity : MonoBehaviour, IMoveVelocity {
		[SerializeField] private float moveSpeed_;

		private Vector3 velocityVector_;

		public void SetVelocity(Vector3 velocityVector) {
			this.velocityVector_ = velocityVector;
		}

		private void Update() {
			transform.position += velocityVector_ * moveSpeed_ * Time.deltaTime;
		}

		public void Disable() {
			this.enabled = false;
		}

		public void Enable() {
			this.enabled = true;
		}
	}
}
