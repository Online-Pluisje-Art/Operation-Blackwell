using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class Node {
		public event System.EventHandler OnLoaded;

		private Grid<NodeObject> grid_;
		// public int gCost;
		// public int hCost;

		// public int fCost {
		// 	get {
		// 		return gCost + hCost;
		// 	}
		// }

		// public Node parent;

		public Node(int width, int height, float nodeRadius) {
			grid_ = new Grid<NodeObject>(width, height, nodeRadius, new Vector3(0, 0, 0), 
				(Grid<NodeObject> g, Vector3 worldPos, int x, int y) => new NodeObject(worldPos, x, y, g, true, true, Node.NodeObject.CoverStatus.NONE));
		}

		public Grid<NodeObject> GetGrid() {
			return grid_;
		}

		public void SetNodeSprite(Vector3 worldPosition, NodeObject.NodeSprite nodeSprite) {
			NodeObject nodeObject = grid_.GetGridObject(worldPosition);
			if(nodeObject != null) {
				nodeObject.SetNodeSprite(nodeSprite);
			}
		}

		public class SaveObject {
			public NodeObject.SaveObject[] nodeObjectSaveObjectArray;
		}
	
		public void Save() {
			List<NodeObject.SaveObject> nodeSaveObjectList = new List<NodeObject.SaveObject>();
			for(int x = 0; x < grid_.gridSizeX; x++) {
				for(int y = 0; y < grid_.gridSizeY; y++) {
					NodeObject nodeObject = grid_.NodeFromWorldPoint(grid_.GetWorldPosition(x, y));
					nodeSaveObjectList.Add(nodeObject.Save());
				}
			}

			SaveObject saveObject = new SaveObject { nodeObjectSaveObjectArray = nodeSaveObjectList.ToArray() };
			SaveSystem.SaveObject(saveObject);
		}

		public void Load() {
			SaveObject saveObject = SaveSystem.LoadMostRecentObject<SaveObject>();
			foreach(NodeObject.SaveObject nodeObjectSaveObject in saveObject.nodeObjectSaveObjectArray) {				
				NodeObject nodeObject = grid_.GetGridObject(nodeObjectSaveObject.x, nodeObjectSaveObject.y);
				nodeObject.Load(nodeObjectSaveObject);
			}
			OnLoaded?.Invoke(this, System.EventArgs.Empty);
		}

		public class NodeObject {

			public enum CoverStatus {
				NONE,
				HALF,
				FULL,
			}

			public enum NodeSprite {
				NONE,
				GROUND,
				PATH,
				DIRT,
				SAND,
			}
			// Holds the amount of cover this tile gives.
			public CoverStatus cover {get; private set;}
			// Holds if the tile can be walked over.
			public bool walkable; // {get; protected set;}
			// Holds if the tile can be shot through.
			public bool shootable {get; private set;}
			public Vector3 worldPosition {get; private set;}
			public int gridX {get; private set;}
			public int gridY {get; private set;}
			private NodeSprite nodeSprite_;

			private Grid<NodeObject> grid_;

			public NodeObject(Vector3 worldPosition, int gridX, int gridY, Grid<NodeObject> grid, bool walkable, bool shootable, CoverStatus cover) {
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
			}

			/*
			* Save - Load
			* */
			public SaveObject Save() {
				return new SaveObject { 
					nodeSprite = this.nodeSprite_,
					x = this.gridX,
					y = this.gridY,
				};
			}

			public void Load(SaveObject saveObject) {
				this.nodeSprite_ = saveObject.nodeSprite;
			}

			public NodeSprite GetNodeSprite() {
				return nodeSprite_;
			}

			public override string ToString() {
				return nodeSprite_.ToString();
			}

			public void SetNodeSprite(NodeSprite nodeSprite) {
				this.nodeSprite_ = nodeSprite;
				grid_.TriggerGridObjectChanged(gridX, gridY);
			}
		}
	}
}
