using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class Grid<TGridObject> {
		private const bool DebugGrid = false;

		public int gridSizeX {get; private set;}
		public int gridSizeY {get; private set;}
		public float cellSize {get; private set;}
		private Vector3 originPosition_;
		private TGridObject[,] gridArray_;

		// We can probably offset, but for now origin is 0,0,0.
		public Grid(int width, int height, float cellSize, Vector3 originPosition, System.Func<Grid<TGridObject>, Vector3, int, int, TGridObject> createGridObject) {
			this.gridSizeX = Mathf.RoundToInt(width / cellSize);
			this.gridSizeY = Mathf.RoundToInt(height / cellSize);
			this.cellSize = cellSize;
			this.originPosition_ = originPosition;

			gridArray_ = new TGridObject[width, height];
			
			for(int x = 0; x < gridArray_.GetLength(0); x++) {
				for(int z = 0; z < gridArray_.GetLength(1); z++) {
					gridArray_[x, z] = createGridObject(this, new Vector3(x, 0, z), x, z);
					// Draw me some boxes.
					Utils.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1));
					Utils.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z));
				}
			}	

			// Draw the outer lines too please.
			Utils.DrawLine(GetWorldPosition(0, gridSizeY), GetWorldPosition(gridSizeX, gridSizeY));
			Utils.DrawLine(GetWorldPosition(gridSizeX, 0), GetWorldPosition(gridSizeX, gridSizeY));
		}


		// CodeMonkey code, but slightly modified for our use case.
		private Vector3 GetWorldPosition(int x, int z) {
			return new Vector3(x, 0, z) * cellSize + originPosition_;
		}

		public List<TGridObject> GetNeighbours(TGridObject node) {
			List<TGridObject> neighbours = new List<TGridObject>();

			for(int x = -1; x <= 1; x++) {
				for(int y = -1; y <= 1; y++) {
					if(x == 0 && y == 0) {
						continue;
					}
					int gridX = -1, gridY = -1;
					for(int i = 0; i < gridArray_.GetLength(0); i++) {
						for(int j = 0; j < gridArray_.GetLength(1); j++) {
							if(gridArray_[i, j].Equals(node)) {
								gridX = i;
								gridY = j;
							}
						}
					}
					int checkX = gridX + x;
					int checkY = gridY + y;
					if(DebugGrid) {
						Debug.Log("checkX: " + checkX + ", checkY: " + checkY);
					}

					if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
						neighbours.Add(gridArray_[checkX, checkY]);
						if(DebugGrid) {
							Debug.Log("Adding x: " + x + ", y: " + y + " as neighbours!");
						}
					}
				}
			}
			return neighbours;
		}

		public TGridObject NodeFromWorldPoint(Vector3 worldPosition) {
			float percentX = (worldPosition.x - .5f) / gridSizeX;
			float percentY = (worldPosition.z - .5f) / gridSizeY;
			percentX = Mathf.Clamp01(percentX);
			percentY = Mathf.Clamp01(percentY);

			int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
			int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
			if(DebugGrid) {
				Debug.Log("x: " + x + ", y: " + y);
			}

			return gridArray_[x, y];
		}

		// CodeMonkey code, but slightly modified for our use case.
		public void GetXZ(Vector3 worldPosition, out int x, out int z) {
			if(DebugGrid) {
				Debug.Log("GetXZ got a vector 3 with x: " + worldPosition.x + ", y: " + worldPosition.y + " and z: " + worldPosition.z + "!");
				Debug.Log("GetXZ got a vector 3 (including origin) with x: " + (worldPosition - originPosition_).x + ", y: " + (worldPosition - originPosition_).y + " and z: " + (worldPosition - originPosition_).z + "!");
			}
			x = Mathf.FloorToInt((worldPosition - originPosition_).x / cellSize);
			z = Mathf.FloorToInt((worldPosition - originPosition_).z / cellSize);
		}
	}
}
