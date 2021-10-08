using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class Node {
		public enum CoverStatus {
			NONE,
			HALF,
			FULL,
		}
		// Holds the amount of cover this tile gives.
		public CoverStatus cover {get; protected set;}
		// Holds if the tile can be walked over.
		public bool walkable {get; protected set;}
		// Holds if the tile can be shot through.
		public bool shootable {get; protected set;}
		public Vector3 worldPosition { get; private set; }
		public int gridX { get; private set; }
		public int gridY { get; private set; }

		private Grid<Node> grid_;
		// public int gCost;
		// public int hCost;

		// public int fCost {
		// 	get {
		// 		return gCost + hCost;
		// 	}
		// }

		// public Node parent;

		public Node(Vector3 worldPosition, int gridX, int gridY, Grid<Node> grid, bool walkable, bool shootable, CoverStatus cover) {
			this.worldPosition = worldPosition;
			this.gridX = gridX;
			this.gridY = gridY;
			this.grid_ = grid;
			this.walkable = walkable;
			this.shootable = shootable;
			this.cover = cover;
		}
	}
}
