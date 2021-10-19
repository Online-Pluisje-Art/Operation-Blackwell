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

		public Grid<Tilemap.Node> grid { get; private set; }

		public GridPathfinding gridPathfinding { get; private set; }
		public Tilemap tilemap { get; private set; }

		[SerializeField] private TilemapVisual tilemapVisual_;
		private Tilemap.Node.NodeSprite nodeSprite_;
		private void Awake() {
			grid = new Grid<Tilemap.Node>((int)gridWorldSize_.x, (int)gridWorldSize_.y, cellSize_, new Vector3(0, 0, 0), 
				(Grid<Tilemap.Node> g, Vector3 worldPos, int x, int y) => new Tilemap.Node(worldPos, x, y, g, true, Tilemap.Node.floorHitChanceModifier, false));
			tilemap = new Tilemap(grid);
			Instance = this;
			Vector3 origin = new Vector3(0, 0);

			gridPathfinding = new GridPathfinding(origin + new Vector3(1, 1) * cellSize_ * .5f, new Vector3(gridWorldSize_.x, gridWorldSize_.y) * cellSize_, cellSize_);
			movementTilemap_ = new MovementTilemap((int)gridWorldSize_.x, (int)gridWorldSize_.y, cellSize_, new Vector3(0, 0, 0));
			// Node unwalkableNode = grid_.NodeFromWorldPoint(new Vector3(0, 0, 2));
			// unwalkableNode.walkable = false;
		}

		private void Start() {
			tilemap.SetTilemapVisual(tilemapVisual_);
			movementTilemap_.SetTilemapVisual(movementTilemapVisual_);
		}

		private void Update() {
			HandleMisc();
		}

		public Grid<Tilemap.Node> GetGrid() {
			return grid;
		}

		public MovementTilemap GetMovementTilemap() {
			return movementTilemap_;
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
