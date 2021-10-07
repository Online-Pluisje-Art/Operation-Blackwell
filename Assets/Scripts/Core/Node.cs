using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class Node {
		public bool walkable { get; private set; }
		public Vector3 worldPosition { get; private set; }
		public int gridX { get; private set; }
		public int gridY { get; private set; }
		// public int gCost;
		// public int hCost;

		// public int fCost {
		// 	get {
		// 		return gCost + hCost;
		// 	}
		// }

		// public Node parent;

		public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY) {
			this.walkable = walkable;
			this.worldPosition = worldPosition;
			this.gridX = gridX;
			this.gridY = gridY;
		}
	}
}
