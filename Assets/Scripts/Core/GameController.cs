using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OperationBlackwell.Core {
	public class GameController : MonoBehaviour {
		private const bool DebugMovement = false;

		public static GameController Instance { get; private set; }

		[SerializeField] private Vector3 gridWorldSize_;
		[SerializeField] private float cellSize_;

		[SerializeField] private MovementTilemapVisual movementTilemapVisual_;
		private MovementTilemap movementTilemap_;

		public Grid<Tilemap.Node> grid { get; private set; }

		public GridPathfinding gridPathfinding { get; private set; }
		public Tilemap tilemap { get; private set; }

		[SerializeField] private TilemapVisual tilemapVisual_;
		private Tilemap.Node.NodeSprite nodeSprite_;
		private Camera mainCamera_;

		private void Awake() {
			grid = new Grid<Tilemap.Node>((int)gridWorldSize_.x, (int)gridWorldSize_.y, cellSize_, new Vector3(0, 0, 0), 
				(Grid<Tilemap.Node> g, Vector3 worldPos, int x, int y) => new Tilemap.Node(worldPos, x, y, g, true, Tilemap.Node.floorHitChanceModifier, false));
			tilemap = new Tilemap(grid);
			Instance = this;
			Vector3 origin = new Vector3(0, 0);

			gridPathfinding = new GridPathfinding(origin + new Vector3(1, 1) * cellSize_ * .5f, new Vector3(gridWorldSize_.x, gridWorldSize_.y) * cellSize_, cellSize_);
			movementTilemap_ = new MovementTilemap((int)gridWorldSize_.x, (int)gridWorldSize_.y, cellSize_, new Vector3(0, 0, 0));
		
			mainCamera_ = Camera.main;
		}

		private void Start() {
			tilemap.SetTilemapVisual(tilemapVisual_);
			movementTilemap_.SetTilemapVisual(movementTilemapVisual_);
			if(SceneManager.GetActiveScene().name == "TutorialLevel") {
				tilemap.Load("tutoriallevel");
			} else {
				Debug.Log(SceneManager.GetActiveScene().name + " has no level to load!");
			}
		}

		private void Update() {
			HandleMisc();
			HandleCameraMovement();
		}

		public Grid<Tilemap.Node> GetGrid() {
			return grid;
		}

		public MovementTilemap GetMovementTilemap() {
			return movementTilemap_;
		}

		private void HandleMisc() {
			if(Input.GetKeyDown(KeyCode.Escape)) {
				SceneManager.LoadScene("MainMenu");
			}
			if(Input.GetKeyDown(KeyCode.Home)) {
				Application.OpenURL("https://github.com/Online-Pluisje-Art/Operation-Blackwell/tree/development");
			}
			if(Input.GetKeyDown(KeyCode.End)) {
				Application.OpenURL("https://docs.opa.rip/");
			}
		}

		private void HandleCameraMovement() {
			Vector3 moveDir = new Vector3(0, 0);
			if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
				moveDir.y = +1;
			}
			if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
				moveDir.y = -1;
			}
			if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
				moveDir.x = -1;
			}
			if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
				moveDir.x = +1;
			}
			moveDir.Normalize();

			float moveSpeed = 15f;
			mainCamera_.transform.position += moveDir * moveSpeed * Time.deltaTime;
		}
	}
}
