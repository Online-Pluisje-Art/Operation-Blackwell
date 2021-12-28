using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using OperationBlackwell.Core;

namespace OperationBlackwell.Player {
	public class MovePositionPathfinding : MonoBehaviour {
		private Action onReachedTargetPosition_;
		private List<Vector3> pathVectorList_;
		private int pathIndex_ = -1;

		public void SetMovePosition(Vector3 movePosition, Vector3 originPos, Action onReachedTargetPosition) {
			this.onReachedTargetPosition_ = onReachedTargetPosition;
			pathVectorList_ = GridPathfinding.Instance.GetPathRouteWithShortcuts(originPos, movePosition).pathVectorList;

			if(pathVectorList_.Count > 0) {
				pathIndex_ = 0;
			} else {
				pathIndex_ = -1;
			}
		}

		private void Update() {
			if(pathIndex_ != -1) {
				// Move to next path position
				Vector3 nextPathPosition = pathVectorList_[pathIndex_];
				Vector3 moveVelocity = (nextPathPosition - transform.position).normalized;
				GetComponent<IMoveVelocity>().SetVelocity(moveVelocity);

				float reachedPathPositionDistance = .1f;
				if(Vector3.Distance(transform.position, nextPathPosition) < reachedPathPositionDistance) {
					pathIndex_++;
					if(pathIndex_ >= pathVectorList_.Count) {
						// End of path
						pathIndex_ = -1;
						pathVectorList_ = null;
						onReachedTargetPosition_();
					}
				}
			} else {
				// Idle
				GetComponent<IMoveVelocity>().SetVelocity(Vector3.zero);
			}
		}
	}
}
