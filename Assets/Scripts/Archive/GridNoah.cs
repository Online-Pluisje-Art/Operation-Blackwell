// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace OperationBlackwell.Archive {
// 	public class Grid {
// 		private const bool DebugGrid = false;
// 		Node[,] grid;

// 		// [SerializeField] private Transform player, targetPos;
// 		private Vector3 gridWorldSize_;
// 		private float nodeRadius_;
// 		// [SerializeField] private LayerMask unwalkableMask;

// 		public float nodeDiameter {get; private set;}
// 		public int gridSizeX {get; private set;}
// 		public int gridSizeY {get; private set;}

// 		// public List<Node> path;

// 		public Grid(Vector3 gridWorldsize, float nodeRadius) {
// 			this.gridWorldSize_ = gridWorldsize;
// 			this.nodeRadius_ = nodeRadius;
// 			this.nodeDiameter = nodeRadius_ * 2;
// 			this.gridSizeX = Mathf.RoundToInt(gridWorldSize_.x / nodeDiameter);
// 			this.gridSizeY = Mathf.RoundToInt(gridWorldSize_.z / nodeDiameter);
// 		}

// 		public Node[,] CreateGrid() {
// 			grid = new Node[gridSizeX, gridSizeY];
// 			Vector3 worldBottomLeft = Vector3.zero - Vector3.right * gridWorldSize_.x / 2 - Vector3.forward * gridWorldSize_.z / 2;

// 			for(int x = 0; x < gridSizeX; x++) {
// 				for(int y = 0; y < gridSizeY; y++) {
// 					Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius_) + Vector3.forward * (y * nodeDiameter + nodeRadius_);
// 					// bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius_, unwalkableMask));
// 					grid[x, y] = new Node(/*walkable*/true, worldPoint, x, y);
// 					Utils.DrawLine(worldPoint, new Vector3(worldPoint.x, worldPoint.y, worldPoint.z + 1));
// 					Utils.DrawLine(worldPoint, new Vector3(worldPoint.x + 1, worldPoint.y, worldPoint.z));
// 				}
// 			}
// 			// Draw the outer lines too please.
// 			Utils.DrawLine(new Vector3(Mathf.RoundToInt(-gridSizeX / 2f) + nodeDiameter, 0, gridSizeY / 2 + nodeDiameter), new Vector3(gridSizeX / 2 + nodeDiameter, 0, gridSizeY / 2 + nodeDiameter));
// 			Utils.DrawLine(new Vector3(gridSizeX / 2 + nodeDiameter, 0, Mathf.RoundToInt(-gridSizeY / 2f) + nodeDiameter), new Vector3(gridSizeX / 2 + nodeDiameter, 0, gridSizeY / 2 + nodeDiameter));
// 			return grid;
// 		}

// 		public List<Node> GetNeighbours(Node node) {
// 			List<Node> neighbours = new List<Node>();

// 			for(int x = -1; x <= 1; x++) {
// 				for(int y = -1; y <= 1; y++) {
// 					if(x == 0 && y == 0) {
// 						continue;
// 					}

// 					int checkX = node.gridX + x;
// 					int checkY = node.gridY + y;
// 					Debug.Log("checkX: " + checkX + ", checkY: " + checkY);

// 					if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
// 						neighbours.Add(grid[checkX, checkY]);
// 						Debug.Log("Adding x: " + x + ", y: " + y + " as neighbours!");
// 					}
// 				}
// 			}
// 			return neighbours;
// 		}

// 		public Node NodeFromWorldPoint(Vector3 worldPosition) {
// 			float percentX = (worldPosition.x + gridWorldSize_.x / 2) / gridWorldSize_.x;
// 			float percentY = (worldPosition.z + gridWorldSize_.y / 2) / gridWorldSize_.y;
// 			percentX = Mathf.Clamp01(percentX);
// 			percentY = Mathf.Clamp01(percentY);

// 			int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
// 			int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
// 			return grid[x, y];
// 		}

// 		// CodeMonkey code, but slightly modified for our use case.
// 		public void GetXZ(Vector3 worldPosition, out int x, out int z) {
// 			if(DebugGrid) {
// 				Debug.Log("GetXZ got a vector 3 with x: " + worldPosition.x + ", y: " + worldPosition.y + " and z: " + worldPosition.z + "!");
// 				// Debug.Log("GetXZ got a vector 3 (including origin) with x: " + (worldPosition - originPosition).x + ", y: " + (worldPosition - originPosition).y + " and z: " + (worldPosition - originPosition).z + "!");
// 			}
// 			x = Mathf.FloorToInt(worldPosition.x / nodeDiameter);
// 			z = Mathf.FloorToInt(worldPosition.z / nodeDiameter);
// 		}
		
// 		// private void OnDrawGizmos() {
// 		// 	Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

// 		// 	Node start = NodeFromWorldPoint(player.position);
// 		// 	Node end = NodeFromWorldPoint(targetPos.position);

// 		// 	if(grid != null) {
// 		// 		foreach(Node n in grid) {
// 		// 			Gizmos.color = (n.walkable) ? Color.white : Color.red;
// 		// 			if(path != null) {
// 		// 				if(path.Contains(n))
// 		// 					Gizmos.color = Color.cyan;
// 		// 			}
// 		// 			if(n == start)
// 		// 				Gizmos.color = Color.yellow;
// 		// 			if(n == end)
// 		// 				Gizmos.color = Color.blue;
// 		// 			Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
// 		// 		}
// 		// 	}
// 		// }
// 	}
// }
