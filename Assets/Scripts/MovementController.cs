using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour {

	private const bool DebugMovement = false;
	private Grid grid;
	[SerializeField] private Transform player;

	private void Start() {
		// This creates a grid of 6 wide and 3 high, cell size 10 and no offset (so bottom left is 0,0,0).
		grid = new Grid(6, 3, 10f, new Vector3(0, 0, 0));
	}

	private void Update() {
		if(Input.GetMouseButtonDown(0)) {
			Vector3 gridClicked = GetMouseWorldPosition3d();
			if(gridClicked == Vector3.zero) {
				if(DebugMovement) {
					// Chances of you clicking on _exactly_ 0,0,0 are almost 0, so oob is likely.
					Debug.Log("Clicked at 0,0,0 or out of the map!");
				}
			} else {
				// Get the current position and the position clicked in X, Z coordinates.
				int currentX, currentZ, clickedX, clickedZ, diffX, diffZ;
				Vector3 currentPosition = player.position;
				grid.GetXZ(currentPosition, out currentX, out currentZ);
				grid.GetXZ(gridClicked, out clickedX, out clickedZ);
				if(DebugMovement) {
					Debug.Log("Player current X: " + currentX + ", current Z: " + currentZ);
					Debug.Log("Player clicked X: " + clickedX + ", clicked Z: " + clickedZ);
				}
				// Check if we can move, if we can, do so.
				if(CanMove(currentX, currentZ, clickedX, clickedZ, out diffX, out diffZ)) {
					if(DebugMovement) {
						Debug.Log("Yes we can move");
					}
					/*
					 * Translate our tictac, to make it work as we want we do -diffX and -diffZ.
					 * We multiply by our cellSize to make it not suck and actually move entire cells at once.
					 * Pass 0 as Y, as we don't care about it.
					 */
					player.Translate(-diffX * grid.cellSize, 0, -diffZ * grid.cellSize);
				} else {
					if(DebugMovement) {
						Debug.Log("No we can't move");
					}
				}
			}
		}
	}

	/*
	 * CodeMonkey code, grab me some 3d mouse position.
	 * Requires a collider below the play grid for our use case!
	 * TODO: This collider grid is hacked in now, make this nicer.
	 */
	private Vector3 GetMouseWorldPosition3d() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f)) {
			return raycastHit.point;
		} else {
			return Vector3.zero;
		}
	}

	/*
	 * Checks if the clicked coordinate is valid, returning true if it is.
	 * We need diffX and diffZ to move, so for now they are out parameters.
	 */
	private bool CanMove(int currentX, int currentZ, int clickedX, int clickedZ, out int diffX, out int diffZ) {
		// Move can happen unless exited out early.
		bool ret = true;
		// Unusable, but needs setting for early bail.
		diffX = diffZ = -999;
		// We're already here! Why move?
		if(currentX == clickedX && currentZ == clickedZ) {
			return false;
		}
		// Out of range, we won't move!
		if(clickedX < 0 || clickedZ < 0 || clickedX > grid.width || clickedZ > grid.height) {
			return false;
		}
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
