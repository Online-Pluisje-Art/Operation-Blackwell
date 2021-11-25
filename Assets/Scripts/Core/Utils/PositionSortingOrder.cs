using UnityEngine;
using System;

namespace OperationBlackwell.Core {
	public class PositionSortingOrder : MonoBehaviour {

		[SerializeField] private int sortingOrderBase_ = 5000;
		[SerializeField] private int offset_ = 0;
		[SerializeField] private bool runOnlyOnce_ = false;

		private float timer_;
		private float timerMax_ = 0.1f;
		private Renderer renderer_;

		private void Awake() {
			renderer_ = gameObject.GetComponent<Renderer>();
		}

		private void LateUpdate() {
			timer_ -= Time.deltaTime;
			if(timer_ <= 0f) {
				timer_ = timerMax_;
				renderer_.sortingOrder = (int)(sortingOrderBase_ - Math.Round(transform.position.y, MidpointRounding.AwayFromZero) - offset_);
				if(runOnlyOnce_) {
					Destroy(this);
				}
			}
		}
	}
}
