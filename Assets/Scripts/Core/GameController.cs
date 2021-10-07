using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class GameController : MonoBehaviour {
		private const bool DebugMovement = false;

		[SerializeField] private Vector3 gridWorldSize_;
		[SerializeField] private float nodeRadius_;

		[SerializeField] private Transform player_;

		private Node[,] nodes_;

		private Grid grid_;
		private void Start() {
			grid_ = new Grid(gridWorldSize_, nodeRadius_);
			nodes_ = grid_.CreateGrid();
		}

		private void Update() {
			if(Input.GetMouseButtonDown(0)) {
				Vector3 gridClicked = Utils.GetMouseWorldPosition3d();
				if(gridClicked == Vector3.zero) {
					if(DebugMovement) {
						// Chances of you clicking on _exactly_ 0,0,0 are almost 0, so oob is likely.
						Debug.Log("Clicked at 0,0,0 or out of the map!");
					}
				} else {
					// Get the current position and the position clicked in X, Z coordinates.
					int currentX, currentZ, clickedX, clickedZ, diffX, diffZ;
					Vector3 currentPosition = player_.position;
					// Vector3 movement;
					grid_.GetXZ(currentPosition, out currentX, out currentZ);
					grid_.GetXZ(gridClicked, out clickedX, out clickedZ);
					if(DebugMovement) {
						Debug.Log("Player current X: " + currentX + ", current Z: " + currentZ);
						Debug.Log("Player clicked X: " + clickedX + ", clicked Z: " + clickedZ);
					}
					// Check if we can move, if we can, do so.
					if(CanMove(currentX, currentZ, clickedX, clickedZ, out diffX, out diffZ)) {
					// if(CanMove(currentPosition, gridClicked, currentX, currentZ, clickedX, clickedZ, out movement)) {
						if(DebugMovement) {
							Debug.Log("Yes we can move");
						}
						/*
						* Translate our tictac, to make it work as we want we do -diffX and -diffZ.
						* We multiply by our cellSize to make it not suck and actually move entire cells at once.
						* Pass 0 as Y, as we don't care about it.
						*/
						player_.Translate(-diffX * grid_.nodeDiameter, 0, -diffZ * grid_.nodeDiameter);
						// player_.Translate(-movement);
					} else {
						if(DebugMovement) {
							Debug.Log("No we can't move");
						}
					}
				}
			}
		}

		/*
		* Checks if the clicked coordinate is valid, returning true if it is.
		* We need diffX and diffZ to move, so for now they are out parameters.
		*/
		private bool CanMove(/*Vector3 playerPosition, Vector3 targetPosition,*/ int currentX, int currentZ, int clickedX, int clickedZ, /*out Vector3 movement) {,*/ out int diffX, out int diffZ) {
			// Move can happen unless exited out early.
			// movement = Vector3.zero;
			bool ret = true;
			// Unusable, but needs setting for early bail.
			diffX = diffZ = -999;
			// We're already here! Why move?
			if(currentX == clickedX && currentZ == clickedZ) {
				return false;
			}
			// Out of range, we won't move!
			if(clickedX > grid_.gridSizeX / 2 || clickedZ > grid_.gridSizeY / 2 || clickedX < -grid_.gridSizeX / 2 || clickedZ < -grid_.gridSizeY / 2) {
				return false;
			}

			// Node playerNode = grid_.NodeFromWorldPoint(playerPosition);
			// Node targetNode = grid_.NodeFromWorldPoint(targetPosition);
			// if(playerNode.worldPosition == targetNode.worldPosition) {
			// 	Debug.Log("Check 2");
			// 	return false;
			// }
			// List<Node> neighbours = grid_.GetNeighbours(playerNode);
			// if(neighbours.Contains(targetNode)) {
			// 	// TODO: Check if node is walkable!
			// 	movement = playerNode.worldPosition - targetNode.worldPosition;
			// 	return true;
			// } else {
			// 	Debug.Log("Check 3");
			// 	return false;
			// }
			/*
			* Calculate the actual diff, a negative Z is up, positive Z is down.
			* Negative X is to the right, positive X is to the left, yes confusing.
			* Catch invalid movements (so either X or Z diff bigger than 1 or minus 1).
			*/
			diffX = currentX - clickedX;
			diffZ = currentZ - clickedZ;
			if(DebugMovement) {
				Debug.Log("diffX: " + diffX + ", diffZ: " + diffZ);
			}
			if(diffX < -1 || diffX > 1) {
				if(DebugMovement) {
					Debug.Log("Can't move on invalid X (diffX: " + diffX + ")!");
				}
				ret = false;
			}
			if(diffZ < -1 || diffZ > 1) {
				if(DebugMovement) {
					Debug.Log("Can't move on invalid Z (diffZ: " + diffZ + ")!");
				}
				ret = false;
			}
			return ret;
		}
	}
}
