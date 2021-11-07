using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace OperationBlackwell.Core {

	public class GridPathfinding {

		public static GridPathfinding Instance { get; private set; }

		// I dont know why it is this number, copied from CodeMonkey example. Used to check for a wall in old functions.
		public const int WALL_WEIGHT = 56000;

		public enum UnitMovementCallbackType {
			Simple,
		}

		//private List<PathNode> openList;
		private BinaryTree binaryTree_;
		private int openListCount_;
		private PathNode[][] mapNodes_;
		private int widthMax_, heightMax_;

		private float nodeSize_;
		private Vector3 worldOrigin_;

		private bool foundTarget_;
		/*
		 * This is used for movement.
		 * The value only matters if we allow direct diagonal movement, but leave it at 10 for now as it might break.
		 */
		private int movementCost_ = 10;
		private float timer_;

		public delegate void OnPathCallback(List<PathNode> path, MapPos finalPos);
		public delegate void OnVoidDelegate();
		public OnVoidDelegate callbacks;

		public GridPathfinding(Vector3 worldLowerLeft, Vector3 worldUpperRight, float nodeSize_) {
			Instance = this;
			worldOrigin_ = worldLowerLeft;
			this.nodeSize_ = nodeSize_;

			float worldWidth = worldUpperRight.x - worldLowerLeft.x + 1;
			float worldHeight = worldUpperRight.y - worldLowerLeft.y + 1;

			int mapWidth = (int)(worldWidth / nodeSize_);
			int mapHeight = (int)(worldHeight / nodeSize_);

			mapNodes_ = new PathNode[mapWidth][];
			for(int i = 0; i < mapWidth; i++) {
				mapNodes_[i] = new PathNode[mapHeight];
			}
			widthMax_ = mapWidth;
			heightMax_ = mapHeight;

			Initialize(mapWidth, mapHeight);
			GameController.Instance.grid.OnGridObjectChanged += OnNodeChanged;
			GameController.Instance.tilemap.OnLoaded += OnLevelChanged;
		}

		public GridPathfinding(int mapWidth, int mapHeight, float nodeSize_, Vector3 worldOrigin_) {//, Texture2D map) {
			this.nodeSize_ = nodeSize_;
			this.worldOrigin_ = worldOrigin_;

			mapNodes_ = new PathNode[mapWidth][];
			for(int i = 0; i < mapWidth; i++) {
				mapNodes_[i] = new PathNode[mapHeight];
			}
			widthMax_ = mapWidth;
			heightMax_ = mapHeight;

			Initialize(mapWidth, mapHeight);
		}

		public void RaycastWalkable() {
			for(int i = 0; i < widthMax_; i++) {
				for(int j = 0; j < heightMax_; j++) {
					Vector3 nodeWorldPosition = mapNodes_[i][j].GetWorldVector(worldOrigin_, nodeSize_);
					RaycastHit2D raycastHit = Physics2D.Raycast(nodeWorldPosition, Vector2.zero, 0f);
					if(raycastHit.collider != null) {
						mapNodes_[i][j].SetWalkable(false);
					}
				}
			}
		}

		private void OnNodeChanged(object grid, Grid<Tilemap.Node>.OnGridObjectChangedEventArgs args) {
			int x = args.x;
			int y = args.y;
			Tilemap.Node node = GameController.Instance.grid.GetGridObject(x, y);
			if(node != null && x >= 0 && x < widthMax_ && y >= 0 && y < heightMax_) {
				mapNodes_[x][y].SetWalkable(node.walkable);
			}
		}

		private void OnLevelChanged(object grid, System.EventArgs args) {
			Grid<Tilemap.Node> grid_ = GameController.Instance.grid;
			foreach(Tilemap.Node node in grid_.GetAllGridObjects()) {
				if(node.walkable) {
					SetWalkable(node.gridX, node.gridY, true);
				} else {
					SetWalkable(node.gridX, node.gridY, false);
				}
			}
		}
		
		public void ModifySize(int modifyX, int modifyY, int newPathNodeWeight) {
			if(modifyX == 0 && modifyY == 0) {
				return;
			}

			int newWidth = widthMax_ + modifyX;
			int newHeight = heightMax_ + modifyY;
		
			PathNode[][] newMapNodes = new PathNode[newWidth][];
			for(int i = 0; i < newWidth; i++) {
				newMapNodes[i] = new PathNode[newHeight];
				for(int j = 0; j < newHeight; j++) {
					if(i < mapNodes_.Length && j < mapNodes_[0].Length) {
						newMapNodes[i][j] = mapNodes_[i][j];
					} else {
						newMapNodes[i][j] = new PathNode(i, j);
						newMapNodes[i][j].SetWeight(newPathNodeWeight);
					}
				}
			}
			widthMax_ = newWidth;
			heightMax_ = newHeight;

			mapNodes_ = newMapNodes;

			UpdateNodeConnections();

			//if (OnSizeModified != null) OnSizeModified(this, EventArgs.Empty);
		}

		public Vector3 GetWorldOffset() {
			return worldOrigin_;
		}

		public float GetNodeSize() {
			return nodeSize_;
		}

		public void SetWalkable(int x, int y, bool walkable) {
			mapNodes_[x][y].SetWalkable(walkable);
		}

		public void SetAllWalkable(bool walkable) {
			for(int x = 0; x < mapNodes_.Length; x++) {
				for(int y = 0; y < mapNodes_[x].Length; y++) {
					mapNodes_[x][y].SetWalkable(walkable);
				}
			}
		}

		public void SetWeight(int x, int y, int weight) {
			mapNodes_[x][y].SetWeight(weight);
		}

		public void SetAllWeight(int weight) {
			for(int x = 0; x < mapNodes_.Length; x++) {
				for(int y = 0; y < mapNodes_[x].Length; y++) {
					mapNodes_[x][y].SetWeight(weight);
				}
			}
		}

		public int GetMapWidth() {
			return widthMax_;
		}

		public int GetMapHeight() {
			return heightMax_;
		}

		public void Initialize(int mapWidth, int mapHeight) {
			// Creates PathNodes
			for(int x = 0; x < mapNodes_.Length; x++) {
				for(int y = 0; y < mapNodes_[x].Length; y++) {
					mapNodes_[x][y] = new PathNode(x, y);
				}
			}
			UpdateNodeConnections();
		}

		private void UpdateNodeConnections() {
			for(int x = 0; x < mapNodes_.Length; x++) {
				for(int y = 0; y < mapNodes_[x].Length; y++) {
					if(y < mapNodes_[x].Length - 1) {
						mapNodes_[x][y].north = mapNodes_[x][y + 1];
					}
					if(y > 0) {
						mapNodes_[x][y].south = mapNodes_[x][y - 1];
					}

					if(x < mapNodes_.Length - 1) {
						mapNodes_[x][y].east = mapNodes_[x + 1][y];
					}
					if(x > 0) {
						mapNodes_[x][y].west = mapNodes_[x - 1][y];
					}
				}
			}
		}

		public void PrintMap(Transform prefabWalkable, Transform prefabUnwalkable) {
			for(int x = 0; x < mapNodes_.Length; x++) {
				for(int y = 0; y < mapNodes_[x].Length; y++) {
					PathNode pathNode = mapNodes_[x][y];
					UnityEngine.Object.Instantiate(pathNode.IsWalkable() ? prefabWalkable : prefabUnwalkable, new Vector3(x * nodeSize_, y * nodeSize_), Quaternion.identity);
				}
			}
		}

		public void PrintMap(Action<int, int> printNode) {
			for(int x = 0; x < mapNodes_.Length; x++) {
				for(int y = 0; y < mapNodes_[x].Length; y++) {
					printNode(x, y);
				}
			}
		}

		public void PrintMap(Action<int, int, Vector3> printNode) {
			for(int x = 0; x < mapNodes_.Length; x++) {
				for(int y = 0; y < mapNodes_[x].Length; y++) {
					printNode(x, y, worldOrigin_ + new Vector3(x * nodeSize_, y * nodeSize_));
				}
			}
		}

		public void PrintMap(Action<int, int> printWalkable, Action<int, int> printUnwalkable) {
			for(int x = 0; x < mapNodes_.Length; x++) {
				for(int y = 0; y < mapNodes_[x].Length; y++) {
					PathNode pathNode = mapNodes_[x][y];
					if(pathNode.IsWalkable()) {
						printWalkable(x, y);
					} else {
						printUnwalkable(x, y);
					}
				}
			}
		}

		public void PrintMap(Action<Vector3, Vector3, Color> createSprite) {
			PrintMap(
				(int x, int y) => {
					createSprite(worldOrigin_ + new Vector3(x * nodeSize_, y * nodeSize_), new Vector3(2, 2), Color.green);
				//MyUtils.World_Sprite.Create(worldOrigin_ + new Vector3(x * nodeSize_, y * nodeSize_), new Vector3(2, 2), SpriteHolder.instance.s_White, Color.green);
				},
				(int x, int y) => {
					createSprite(worldOrigin_ + new Vector3(x * nodeSize_, y * nodeSize_), new Vector3(2, 2), Color.red);
				//MyUtils.World_Sprite.Create(worldOrigin_ + new Vector3(x * nodeSize_, y * nodeSize_), new Vector3(2, 2), SpriteHolder.instance.s_White, Color.red);
				}
			);
		}

		/*public void PrintMapUpdateable() {
		}*/
		private bool IsValidShortcut(int startX, int startY, int endX, int endY) {
			//Debug.Log("Testing Shortcut: " + startX + ", " + startY + " -> " + endX + ", " + endY);
			int shortcutWeight = mapNodes_[startX][startY].weight;
			Vector3 dir = (new Vector3(endX, endY) - new Vector3(startX, startY)).normalized;
			Vector3 test = new Vector3(startX, startY) + dir;
			int testX = Mathf.RoundToInt(test.x);
			int testY = Mathf.RoundToInt(test.y);
			// Check if shortcut is walkable
			//Debug.Log("Testing: "+testX+","+testY);
			while(!(testX == endX && testY == endY)) {
				if(!IsWalkable(testX, testY) || mapNodes_[testX][testY].weight != shortcutWeight) {
					// Not walkable
					//Debug.Log("Shortcut invalid!");
					return false;
				} else {
					test += dir;
					testX = Mathf.RoundToInt(test.x);
					testY = Mathf.RoundToInt(test.y);
					//Debug.Log("Testing: "+testX+","+testY);
				}
			}
			// Shortcut walkable
			//Debug.Log("Shortcut valid!");
			return true;
		}

		public List<PathNode> GetFindPath(MapPos startPos, MapPos finalPos) {
			int width = widthMax_;
			int height = heightMax_;

			startPos = GetClosestValidPos(startPos.x, startPos.y);

			if(startPos.x < 0 || startPos.y < 0 || finalPos.x < 0 || finalPos.y < 0 ||
				startPos.x >= width || finalPos.x >= width ||
				startPos.y >= height || finalPos.y >= height) {
				return null; //Out of bounds!
			}
			if(mapNodes_[finalPos.x][finalPos.y].weight == WALL_WEIGHT ||
				mapNodes_[startPos.x][startPos.y].weight == WALL_WEIGHT) {
				return null; //Wall
			}
			return findPath(startPos.x, startPos.y, finalPos.x, finalPos.y);
		}

		public List<PathNode> GetFindPathClosest(MapPos startPos, List<MapPos> allFinalPos) {
			List<PathNode> closest = null;

			for(int i = 0; i < allFinalPos.Count; i++) {
				List<PathNode> path = GetFindPath(startPos, allFinalPos[i]);
				if(path != null) {
					if(closest == null) {
						closest = path; 
					} else {
						if(path.Count < closest.Count) {
							closest = path;
						}
					}
				}
			}

			return closest;
		}

		public bool HasPath(int startX, int startY, int endX, int endY) {
			return FindPath(startX, startY, new MapPos(endX, endY), (List<PathNode> path, MapPos finalPos) => { });
		}

		public bool FindPath(int startX, int startY, MapPos finalPos, OnPathCallback callback) {
			return FindPath(startX, startY, new List<MapPos>() { finalPos }, callback);
		}

		public bool FindPath(int startX, int startY, List<MapPos> finalPositions, OnPathCallback callback) {
			int width = widthMax_;
			int height = heightMax_;

			MapPos start = GetClosestValidPos(startX, startY);
			startX = start.x;
			startY = start.y;
			List<PathRoute> paths = new List<PathRoute>();

			foreach(MapPos finalPos in finalPositions) {
				if(startX < 0 || startY < 0 || finalPos.x < 0 || finalPos.y < 0 ||
					startX >= width || finalPos.x >= width ||
					startY >= height || finalPos.y >= height) {
					continue; // Out of bounds!
				}
				if(mapNodes_[finalPos.x][finalPos.y].weight == WALL_WEIGHT ||
					mapNodes_[startX][startY].weight == WALL_WEIGHT) {
					// Find close non-wall start/end
					continue; // Wall
				}

				List<PathNode> currentPath = findPath(startX, startY, finalPos.x, finalPos.y);
				if(currentPath.Count <= 0 && (startX != finalPos.x || startY != finalPos.y)) {
					// Don't add path if there's no path
				} else {
					if(!finalPos.straightToOffset) {
						// Don't go straight to offset, add dummy
						currentPath.Add(currentPath[currentPath.Count - 1]);
					}
					paths.Add(new PathRoute(currentPath, worldOrigin_, nodeSize_, finalPos));
				}
			}
			int smallest = 0;
			for(int i = 1; i < paths.Count; i++) {
				if(paths[i].pathNodeList.Count < paths[smallest].pathNodeList.Count)
					smallest = i;
			}

			if(paths.Count <= 0 || (paths.Count > 0 && paths[smallest].pathNodeList.Count <= 0)) {
				// No path
				return false;
			} else {
				callback(paths[smallest].pathNodeList, paths[smallest].finalPos);
			}
			return true;
		}

		public Vector3 GetClosestValidPosition(Vector3 position) {
			int mapX, mapY;
			ConvertVectorPositionValidate(position, out mapX, out mapY);
			MapPos closestValidMapPos = GetClosestValidPos(mapX, mapY);
			PathNode pathNode = mapNodes_[closestValidMapPos.x][closestValidMapPos.y];
			return pathNode.GetWorldVector(worldOrigin_, nodeSize_);
		}

		private MapPos GetClosestValidPos(int mapX, int mapY) {
			int width = widthMax_;
			int height = heightMax_;
			// Inside bounds
			while(mapX < 0) { 
				mapX++; 
			}
			while(mapY < 0) {
				mapY++;
			}
			while(mapX >= width) { 
				mapX--; 
			}
			while(mapY >= height) {
				mapY--;
			}

			// Check inside walls
			if(mapNodes_[mapX][mapY].weight == WALL_WEIGHT) {
				int radius = 1;
				MapPos valid = null;
				do {
					valid = GetValidPosRadius(mapX, mapY, radius);
					radius++;
				} while(valid == null && radius < 100);
				if(radius == 100) {
					return new MapPos(0, 0);
				}
				return valid;
			}
			return new MapPos(mapX, mapY);
		}

		private MapPos GetValidPosRadius(int mapX, int mapY, int radius) {
			int width = widthMax_;
			int height = heightMax_;

			int endX = mapX + radius;
			for(int i = mapX - radius; i <= endX; i++) {
				int j = mapY + radius;
				if(i < 0 || i >= width || j < 0 || j >= height) {
					//Out of bounds
				} else {
					if(mapNodes_[i][j].weight != WALL_WEIGHT) {
						return new MapPos(i, j);
					}
				}

				j = mapY - radius;
				if(i < 0 || i >= width || j < 0 || j >= height) {
					//Out of bounds
				} else {
					if(mapNodes_[i][j].weight != WALL_WEIGHT) {
						return new MapPos(i, j);
					}
				}

			}
			int endY = mapY + radius;
			for(int j = mapY - radius + 1; j < endY; j++) {
				int i = mapX - radius;
				if(i < 0 || i >= width || j < 0 || j >= height) {
					//Out of bounds
				} else {
					if(mapNodes_[i][j].weight != WALL_WEIGHT) {
						return new MapPos(i, j);
					}
				}

				i = mapX + radius;
				if(i < 0 || i >= width || j < 0 || j >= height) {
					//Out of bounds
				} else {
					if(mapNodes_[i][j].weight != WALL_WEIGHT) {
						return new MapPos(i, j);
					}
				}

			}
			return null;
		}

		public void ApplyShortcuts(ref List<PathNode> pathNodeList) {
			if(pathNodeList.Count > 1) {
				int testStartNodeIndex = 1;
				while(testStartNodeIndex < pathNodeList.Count - 2) { // Only test untils there's 3 nodes left
					PathNode testStartNode = pathNodeList[testStartNodeIndex];
					int testEndNodeIndex = testStartNodeIndex + 2;
					// Test start node with node 2 indexes in front
					PathNode testEndNode = pathNodeList[testEndNodeIndex];
					while(IsValidShortcut(testStartNode.xPos, testStartNode.yPos, testEndNode.xPos, testEndNode.yPos)) {
						// Valid shortcut
						// Remove in between node
						pathNodeList.RemoveAt(testStartNodeIndex + 1);
						if(testEndNodeIndex >= pathNodeList.Count - 1) {
							// No more nodes
							break;
						} else {
							// Test next node
							testEndNode = pathNodeList[testEndNodeIndex];
						}
					}
					// Start next shortcut test from this end node
					testStartNodeIndex = testEndNodeIndex;
				}
			}
		}

		public PathRoute GetPathRouteWithShortcuts(Vector3 start, Vector3 end) {
			List<PathNode> pathNodeList = GetPath(start, end);
			ApplyShortcuts(ref pathNodeList);
			return new PathRoute(pathNodeList, worldOrigin_, nodeSize_, null);
		}

		public PathRoute GetPathRoute(Vector3 start, Vector3 end) {
			List<PathNode> pathNodeList = GetPath(start, end);
			return new PathRoute(pathNodeList, worldOrigin_, nodeSize_, null);
		}

		public List<PathNode> GetPath(int startX, int startY, int endX, int endY) {
			return findPath(startX, startY, endX, endY);
		}

		public List<PathNode> GetPath(Vector3 start, Vector3 end) {
			start = start - worldOrigin_;
			end = end - worldOrigin_;
			start = start / nodeSize_;
			end = end / nodeSize_;
			MapPos startMapPos = GetClosestValidPos(Mathf.RoundToInt(start.x), Mathf.RoundToInt(start.y));
			MapPos endMapPos = GetClosestValidPos(Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y));
			return findPath(startMapPos.x, startMapPos.y, endMapPos.x, endMapPos.y);
		}

		public List<PathNode> findPath(int startX, int startY, int endX, int endY) {
			List<PathNode> ret = new List<PathNode>();
			// Calculate H for all nodes
			CalculateAllHeuristics(endX, endY);

			// Start finding target
			foundTarget_ = false;
			binaryTree_ = new BinaryTree();
			openListCount_ = 1;

			PathNode currentNode = mapNodes_[startX][startY];
			PathNode targetNode = mapNodes_[endX][endY];

			if(currentNode == targetNode) {
				return new List<PathNode> { currentNode };
			}
			int iterations = 0;
			do {
				iterations++;
				currentNode = FindTarget(currentNode, targetNode);
			} while(!foundTarget_ && openListCount_ > 0 && iterations < 60000);
			if(iterations >= 60000) {
				UnityEngine.Debug.Log("iteration overload");
			}

			if(foundTarget_) {
				// Get path
				currentNode = targetNode;
				ret.Add(currentNode);
				while(currentNode.parent != null && currentNode.parent != currentNode) {
					ret.Add(currentNode.parent);
					currentNode = currentNode.parent;
				}
				if(currentNode.parent == currentNode) {
					UnityEngine.Debug.Log("parent == child");
				}
			} else {
				// No path possible
			}
			ret.Reverse();

			return ret;
		}

		private PathNode FindTarget(PathNode currentNode, PathNode targetNode) {
			// Check the north node
			if(currentNode.moveNorth) {
				DetermineNodeValues(currentNode, currentNode.north, targetNode);
			}
			// Check the east node
			if(currentNode.moveEast) {
				DetermineNodeValues(currentNode, currentNode.east, targetNode);
			}
			// Check the south node
			if(currentNode.moveSouth) {
				DetermineNodeValues(currentNode, currentNode.south, targetNode);
			}
			// Check the west node
			if(currentNode.moveWest) {
				DetermineNodeValues(currentNode, currentNode.west, targetNode);
			}

			if(!foundTarget_) {
				// Once done checking add to the closed list and remove from the open list
				AddToClosedList(currentNode);
				RemoveFromOpenList(currentNode);

				// Get the next node with the smallest F value
				return GetSmallestFValueNode();
			} else {
				return null;
			}
		}

		private void DetermineNodeValues(PathNode currentNode, PathNode testing, PathNode targetNode) {
			// Dont work on null nodes
			if(testing == null) {
				return;
			}

			// Check to see if the node is the target
			if(testing == targetNode) {
				targetNode.parent = currentNode;
				foundTarget_ = true;
				return;
			}

			// Ignore Walls
			if(currentNode.weight == WALL_WEIGHT || testing.weight == WALL_WEIGHT) {
				return;
			}

			// While the node has not already been tested
			if(!testing.isOnClosedList) {
				// Check to see if the node is already on the open list
				if(testing.isOnOpenList) {
					// Get a Gcost to move from this node to the testing node
					int newGcost = currentNode.gValue + currentNode.weight + movementCost_;

					// If the G cost is better then change the nodes parent and update its costs.
					if(newGcost < testing.gValue) {
						testing.parent = currentNode;
						testing.gValue = newGcost;
						binaryTree_.RemoveNode(testing);
						testing.CalculateFValue();
						binaryTree_.AddNode(testing);
					}
				} else {
					// Set the testing nodes parent to the current location, calculate its costs, and add it to the open list
					testing.parent = currentNode;
					testing.gValue = currentNode.gValue + currentNode.weight + movementCost_;
					testing.CalculateFValue();
					AddToOpenList(testing);
				}
			}
		}

		private void AddToOpenList(PathNode node) {
			binaryTree_.AddNode(node);
			openListCount_++;
			node.isOnOpenList = true;
		}

		private void AddToClosedList(PathNode currentNode) {
			currentNode.isOnClosedList = true;
		}

		private void RemoveFromOpenList(PathNode currentNode) {
			binaryTree_.RemoveNode(currentNode);
			openListCount_--;
			currentNode.isOnOpenList = false;
		}

		private PathNode GetSmallestFValueNode() {
			return binaryTree_.GetSmallest();
		}

		private void CalculateManhattanDistance(PathNode currentNode, int currX, int currY, int targetX, int targetY) {
			currentNode.parent = null;
			currentNode.hValue = (Mathf.Abs(currX - targetX) + Mathf.Abs(currY - targetY));
			currentNode.isOnOpenList = false;
			currentNode.isOnClosedList = false;
		}
		
		private void CalculateAllHeuristics(int endX, int endY) {
			int rows = heightMax_;
			int cols = widthMax_;
			for(int x = 0; x < cols; x++) {
				for(int y = 0; y < rows; y++) {
					CalculateManhattanDistance(mapNodes_[x][y], x, y, endX, endY);
				}
			}
		}

		public void ResetRestrictions() {
			for(int i = 0; i < GetMapWidth(); i++) {
				for(int j = 0; j < GetMapHeight(); j++) {
					mapNodes_[i][j].ResetRestrictions();
				}
			}
		}

		public void RefreshAllHitboxes() {
			for(int x = 0; x < widthMax_; x++) {
				for(int y = 0; y < heightMax_; y++) {
					mapNodes_[x][y].TestHitbox();
				}
			}
			//Event_Speaker.Broadcast(Event_Trigger.Pathfinding_Refresh);
		}

		public void RefreshHitbox(MapPos mapPos) {
			mapNodes_[mapPos.x][mapPos.y].TestHitbox();
			//Event_Speaker.Broadcast(Event_Trigger.Pathfinding_Refresh);
		}

		public bool IsWalkable(MapPos mapPos) {
			return mapNodes_[mapPos.x][mapPos.y].weight != WALL_WEIGHT;
		}

		public bool IsWalkable(int x, int y) {
			if(x < 0 || x >= widthMax_ || y < 0 || y >= heightMax_) {
				return false;
			}
			return mapNodes_[x][y].weight != WALL_WEIGHT;
		}

		public bool IsWall(int x, int y) {
			if(x < 0 || x > widthMax_ || y < 0 || y > heightMax_) {
				return false;
			}
			return mapNodes_[x][y].weight == WALL_WEIGHT;
		}

		public bool HasWeight(int x, int y) {
			return mapNodes_[x][y].weight > 0;
		}


		private void ConvertVectorPosition(Vector3 position, out int x, out int y) {
			x = (int)((position.x - worldOrigin_.x) / nodeSize_);
			y = (int)((position.y - worldOrigin_.y) / nodeSize_);
		}
		
		private void ConvertVectorPositionValidate(Vector3 position, out int x, out int y) {
			ConvertVectorPosition(position, out x, out y);

			if(x < 0) {
				x = 0;
			}
			if(y < 0) {
				y = 0;
			}
			if(x >= widthMax_) {
				x = widthMax_ - 1;
			}
			if(y >= heightMax_) {
				y = heightMax_ - 1;
			}
		}
	}
}
