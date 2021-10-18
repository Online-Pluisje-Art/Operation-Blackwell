using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class Tilemap {
		public event System.EventHandler OnLoaded;

		private Grid<Node> grid_;
		// public int gCost;
		// public int hCost;

		// public int fCost {
		// 	get {
		// 		return gCost + hCost;
		// 	}
		// }

		// public Node parent;

		public Tilemap(Grid<Node> grid) {
			this.grid_ = grid;
		}

		public Grid<Node> GetGrid() {
			return grid_;
		}

		public void SetNodeSprite(Vector3 worldPosition, Node.NodeSprite nodeSprite) {
			Node node = grid_.GetGridObject(worldPosition);
			if(node != null) {
				node.SetNodeSprite(nodeSprite);
			}
		}

		public void SetTilemapVisual(TilemapVisual tilemapVisual) {
			tilemapVisual.SetGrid(this, grid_);
		}

		public class SaveObject {
			public Node.SaveObject[] nodeSaveObjectArray;
		}
	
		public void Save(System.String name) {
			List<Node.SaveObject> nodeSaveObjectList = new List<Node.SaveObject>();
			for(int x = 0; x < grid_.gridSizeX; x++) {
				for(int y = 0; y < grid_.gridSizeY; y++) {
					Node node = grid_.GetGridObject(x, y);
					nodeSaveObjectList.Add(node.Save());
				}
			}

			SaveObject saveObject = new SaveObject { nodeSaveObjectArray = nodeSaveObjectList.ToArray() };
			SaveSystem.SaveObject(saveObject, name);
		}

		public void Load() {
			SaveObject saveObject = SaveSystem.LoadMostRecentObject<SaveObject>();
			foreach(Node.SaveObject nodeSaveObject in saveObject.nodeSaveObjectArray) {				
				Node node = grid_.GetGridObject(nodeSaveObject.x, nodeSaveObject.y);
				node.Load(nodeSaveObject);
			}
			OnLoaded?.Invoke(this, System.EventArgs.Empty);
		}

		public class Node {
			public enum NodeSprite {
				// Default sprite.
				NONE,
				PIT,
				FLOOR,
				WALL,
				COVER,
			}
			// Holds the amount of cover this tile gives.
			public bool cover {get; private set;}
			// Holds if the tile can be walked over.
			public bool walkable {get; protected set;}
			// Holds if the tile can be shot through.
			public bool shootable {get; private set;}
			public Vector3 worldPosition {get; private set;}
			public int gridX {get; private set;}
			public int gridY {get; private set;}
			private NodeSprite nodeSprite_;

			private bool isValidMovePosition_;
			private IGameObject unitGridCombat_;

			private Grid<Node> grid_;

			public Node(Vector3 worldPosition, int gridX, int gridY, Grid<Node> grid, bool walkable, bool shootable, bool cover) {
				this.worldPosition = worldPosition;
				this.gridX = gridX;
				this.gridY = gridY;
				this.grid_ = grid;
				this.walkable = walkable;
				this.shootable = shootable;
				this.cover = cover;
			}

			[System.Serializable]
			public class SaveObject {
				public NodeSprite nodeSprite;
				public int x;
				public int y;
				public bool walkable;
				public bool shootable;
				public bool cover;
			}

			/*
			* Save - Load
			* */
			public SaveObject Save() {
				return new SaveObject { 
					nodeSprite = this.nodeSprite_,
					x = this.gridX,
					y = this.gridY,
					walkable = this.walkable,
					shootable = this.shootable,
					cover = this.cover,
				};
			}

			public void Load(SaveObject saveObject) {
				this.nodeSprite_ = saveObject.nodeSprite;
				this.walkable = saveObject.walkable;
				this.shootable = saveObject.shootable;
				this.cover = saveObject.cover;
			}

			public NodeSprite GetNodeSprite() {
				return nodeSprite_;
			}

			public override string ToString() {
				return nodeSprite_.ToString();
			}

			public void SetNodeSprite(NodeSprite nodeSprite) {
				this.nodeSprite_ = nodeSprite;
				if(nodeSprite == NodeSprite.NONE) {
					// Should be error state!
					Debug.Log("NodeSprite none will become an hard error in the future!");
					this.walkable = true;
					this.shootable = true;
					this.cover = false;
				} else if(nodeSprite == NodeSprite.PIT) {
					this.walkable = false;
					this.shootable = true;
					this.cover = false;
				} else if(nodeSprite == NodeSprite.FLOOR) {
					this.walkable = true;
					this.shootable = true;
					this.cover = false;
				} else if(nodeSprite == NodeSprite.WALL) {
					this.walkable = false;
					this.shootable = false;
					this.cover = false;
				} else if(nodeSprite == NodeSprite.COVER) {
					this.walkable = false;
					this.shootable = true;
					this.cover = true;
				}
				grid_.TriggerGridObjectChanged(gridX, gridY);
			}

			public void SetIsValidMovePosition(bool set) {
				isValidMovePosition_ = set;
			}

			public bool GetIsValidMovePosition() {
				return isValidMovePosition_;
			}

			public void SetUnitGridCombat(IGameObject unitGridCombat) {
				this.unitGridCombat_ = unitGridCombat;
			}

			public void ClearUnitGridCombat() {
				SetUnitGridCombat(null);
			}

			public IGameObject GetUnitGridCombat() {
				return unitGridCombat_;
			}
		}
	}
}
