using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class GameController : MonoBehaviour {
		private const bool DebugMovement = false;

		[SerializeField] private Vector3 gridWorldSize_;
		[SerializeField] private float nodeRadius_;

		[SerializeField] private Transform player_;

		public delegate void MoveEvent(Vector3 position);
		public event MoveEvent Moved;

		private Grid<Tilemap.Node> grid_;
		private Tilemap tilemap_;

		[SerializeField] private TilemapVisual tilemapVisual_;
		private Tilemap.Node.NodeSprite nodeSprite_;
		private void Start() {
			grid_ = new Grid<Tilemap.Node>((int)gridWorldSize_.x, (int)gridWorldSize_.y, nodeRadius_, new Vector3(0, 0, 0), 
				(Grid<Tilemap.Node> g, Vector3 worldPos, int x, int y) => new Tilemap.Node(worldPos, x, y, g, true, true, false));
			tilemap_ = new Tilemap(grid_);
			tilemap_.SetTilemapVisual(tilemapVisual_);
			// Node unwalkableNode = grid_.NodeFromWorldPoint(new Vector3(0, 0, 2));
			// unwalkableNode.walkable = false;
		}

		private void Update() {
			HandleMovement();
			HandlePainting();
			HandleSaveLoad();
			HandleMisc();
		}

		private void HandleMovement() {
			if(Input.GetMouseButtonDown(0)) {
				Vector3 gridClicked = Utils.GetMouseWorldPosition();
				if(gridClicked == Vector3.zero) {
					if(DebugMovement) {
						// Chances of you clicking on _exactly_ 0,0,0 are almost 0, so oob is likely.
						Debug.Log("Clicked at 0,0,0 or out of the map!");
					}
				} else {
					// Get the current position and the position clicked in X, Y coordinates.
					int currentX, currentY, clickedX, clickedY;
					Vector3 currentPosition = player_.position;
					Vector3 movement;
					grid_.GetXY(currentPosition, out currentX, out currentY);
					grid_.GetXY(gridClicked, out clickedX, out clickedY);
					if(DebugMovement) {
						Debug.Log("Player current X: " + currentX + ", current Y: " + currentY);
						Debug.Log("Player clicked X: " + clickedX + ", clicked Y: " + clickedY);
					}
					// Check if we can move, if we can, do so.
					if(CanMove(currentPosition, gridClicked, currentX, currentY, clickedX, clickedY, out movement)) {
						if(DebugMovement) {
							Debug.Log("Yes we can move");
						}
						// Translate our tictac.
						player_.Translate(movement);
						// Notify listeners.
						Moved?.Invoke(player_.position);
					} else {
						if(DebugMovement) {
							Debug.Log("No we can't move");
						}
					}
				}
			}
		}

		private void HandlePainting() {
			// Tilemap code, rightclick please!
			if(Input.GetKeyDown(KeyCode.T)) {
				nodeSprite_ = Tilemap.Node.NodeSprite.NONE;
			}
			if(Input.GetKeyDown(KeyCode.Y)) {
				nodeSprite_ = Tilemap.Node.NodeSprite.PIT;
			}
			if(Input.GetKeyDown(KeyCode.U)) {
				nodeSprite_ = Tilemap.Node.NodeSprite.FLOOR;
			}
			if(Input.GetKeyDown(KeyCode.I)) {
				nodeSprite_ = Tilemap.Node.NodeSprite.WALL;
			}
			if(Input.GetKeyDown(KeyCode.O)) {
				nodeSprite_ = Tilemap.Node.NodeSprite.COVER;
			}
			if(Input.GetMouseButtonDown(1)) {
				Vector3 mouseWorldPosition = Utils.GetMouseWorldPosition();
				Tilemap.Node node = grid_.NodeFromWorldPoint(mouseWorldPosition);
				node.SetNodeSprite(nodeSprite_);
				grid_.TriggerGridObjectChanged(node.gridX, node.gridY);
			}
		}

		private void HandleSaveLoad() {
			if(Input.GetKeyDown(KeyCode.P)) {
				tilemap_.Save();
			}
			if(Input.GetKeyDown(KeyCode.L)) {
				tilemap_.Load();
			}
		}

		private void HandleMisc() {
			if(Input.GetKeyDown(KeyCode.Escape)) {
				Application.Quit(0);
			}
			if(Input.GetKeyDown(KeyCode.Home)) {
				Application.OpenURL("https://github.com/Online-Pluisje-Art/Operation-Blackwell/tree/development");
			}
			if(Input.GetKeyDown(KeyCode.End)) {
				Application.OpenURL("https://docs.opa.rip/");
			}
		}

		/*
		 * Checks if the clicked coordinate is valid, returning true if it is.
		 */
		private bool CanMove(Vector3 playerPosition, Vector3 targetPosition, int currentX, int currentY, int clickedX, int clickedY, out Vector3 movement) {
			// Move can happen unless exited out early.
			movement = Vector3.zero;
			// We're already here! Why move?
			if(currentX == clickedX && currentY == clickedY) {
				return false;
			}
			if(clickedX < 0 || clickedY < 0 || clickedX > grid_.gridSizeX || clickedY > grid_.gridSizeY) {
				return false;
			}

			Tilemap.Node playerNode = grid_.GetGridObject(playerPosition);
			Tilemap.Node targetNode = grid_.GetGridObject(targetPosition);

			if(playerNode.worldPosition == targetNode.worldPosition) {
				return false;
			}
			// We're moving, so we need to check if we can move.
			List<Tilemap.Node> neighbours = grid_.GetNeighbours(playerNode);
			if(neighbours.Contains(targetNode)) {
				if(targetNode.walkable) {
					movement = targetNode.worldPosition - playerNode.worldPosition;
					return true;	
				} else {
					return false;
				}
			} else {
				return false;
			}
		}
	}
}
