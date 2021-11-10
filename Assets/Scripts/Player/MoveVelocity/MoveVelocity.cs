using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OperationBlackwell.Player {
	public class MoveVelocity : MonoBehaviour, IMoveVelocity {
		[SerializeField] private float moveSpeed_;
		[SerializeField] Animator animator;

		private Vector3 velocityVector_;
		private Rigidbody2D rigidbody2D_;
		private PlayerBase characterBase_;

		private void Awake() {
			rigidbody2D_ = GetComponent<Rigidbody2D>();
			characterBase_ = GetComponent<PlayerBase>();
		}

		public void SetVelocity(Vector3 velocityVector) {
			this.velocityVector_ = velocityVector;
		}

		private void FixedUpdate() {
			rigidbody2D_.velocity = velocityVector_ * moveSpeed_;
			if (this.name == "Player")
				updateAnimation();
		}

		public void Disable() {
			this.enabled = false;
			rigidbody2D_.velocity = Vector3.zero;
		}

		public void Enable() {
			this.enabled = true;
		}
		
		private void updateAnimation()
        {
			Debug.Log(velocityVector_);

			animator.SetFloat("Horizontal", velocityVector_.x);
			animator.SetFloat("Vertical", velocityVector_.y);

			if ((velocityVector_.x == 0) && (velocityVector_.y == 0))
				animator.SetBool("isWalking", false);
			else
				animator.SetBool("isWalking", true);
		}
	}
}
