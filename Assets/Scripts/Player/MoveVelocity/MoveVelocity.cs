using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OperationBlackwell.Player {
	public class MoveVelocity : MonoBehaviour, IMoveVelocity {
		[SerializeField] private float moveSpeed_;
		[SerializeField] private Animator animator_;

		private Vector3 velocityVector_;
		private Rigidbody2D rigidbody2D_;

		private void Awake() {
			rigidbody2D_ = GetComponent<Rigidbody2D>();
		}

		public void SetVelocity(Vector3 velocityVector) {
			this.velocityVector_ = velocityVector;
		}

		private void FixedUpdate() {
			rigidbody2D_.velocity = velocityVector_ * moveSpeed_;
			if(this.name == "Adam" || this.name == "Adam (1)") {
				updateAnimation();
			}
		}

		public void Disable() {
			this.enabled = false;
			rigidbody2D_.velocity = Vector3.zero;
			if(this.name == "Adam" || this.name == "Adam (1)") {
				updateAnimation();
			}
		}

		public void Enable() {
			this.enabled = true;
		}
		
		private void updateAnimation() {
			animator_.SetFloat("Horizontal", velocityVector_.x);
			animator_.SetFloat("Vertical", velocityVector_.y);

			if((velocityVector_.x == 0) && (velocityVector_.y == 0)) {
				animator_.SetBool("isWalking", false);
			} else {
				animator_.SetBool("isWalking", true);
			}
		}
	}
}
