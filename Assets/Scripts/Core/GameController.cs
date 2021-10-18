using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class GameController : MonoBehaviour {
		private const bool DebugMovement = false;

		public static GameController Instance { get; private set; }

		[SerializeField] private Vector3 gridWorldSize_;
		[SerializeField] private float cellSize_;

		[SerializeField] private MovementTilemapVisual movementTilemapVisual_;
		private MovementTilemap movementTilemap_;

		[SerializeField] private Transform player_;

		public delegate void MoveEvent(Vector3 position);
		public event MoveEvent Moved;

		private Grid<Tilemap.Node> grid_;
		private Tilemap tilemap_;
		public GridPathfinding gridPathfinding { get; private set; }

		[SerializeField] private TilemapVisual tilemapVisual_;
		private Tilemap.Node.NodeSprite nodeSprite_;
		private void Awake() {
			grid_ = new Grid<Tilemap.Node>((int)gridWorldSize_.x, (int)gridWorldSize_.y, cellSize_, new Vector3(0, 0, 0), 
				(Grid<Tilemap.Node> g, Vector3 worldPos, int x, int y) => new Tilemap.Node(worldPos, x, y, g, true, true, false));
			tilemap_ = new Tilemap(grid_);
			Instance = this;
			Vector3 origin = new Vector3(0, 0);

			gridPathfinding = new GridPathfinding(origin + new Vector3(1, 1) * cellSize_ * .5f, new Vector3(gridWorldSize_.x, gridWorldSize_.y) * cellSize_, cellSize_);
			movementTilemap_ = new MovementTilemap((int)gridWorldSize_.x, (int)gridWorldSize_.y, cellSize_, new Vector3(0, 0, 0));
			// Node unwalkableNode = grid_.NodeFromWorldPoint(new Vector3(0, 0, 2));
			// unwalkableNode.walkable = false;
		}

		private void Start() {
			tilemap_.SetTilemapVisual(tilemapVisual_);
			movementTilemap_.SetTilemapVisual(movementTilemapVisual_);
		}

		private void Update() {
			HandlePainting();
			HandleSaveLoad();
			HandleMisc();
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
				Tilemap.Node node = grid_.GetGridObject(mouseWorldPosition);
				node.SetNodeSprite(nodeSprite_);
				grid_.TriggerGridObjectChanged(node.gridX, node.gridY);
			}
		}

		public Grid<Tilemap.Node> GetGrid() {
			return grid_;
		}

		public MovementTilemap GetMovementTilemap() {
			return movementTilemap_;
		}

		private void HandleSaveLoad() {
			if(Input.GetKeyDown(KeyCode.P)) {
				System.String saveName = getSaveName();
				tilemap_.Save(saveName);
			}
			if(Input.GetKeyDown(KeyCode.L)) {
				tilemap_.Load();
			}
		}

		private System.String getSaveName() {
			return "testing";
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
	}
}
