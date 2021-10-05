using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell {
	public class Grid {
		private const bool DebugGrid = false;

		public int width { get; private set; }
		public int height { get; private set; }
		public float cellSize { get; private set; }
		private Vector3 originPosition;
		private int[,] gridArray;

		// We can probably offset, but for now origin is 0,0,0.
		public Grid(int width, int height, float cellSize, Vector3 originPosition) {
			this.width = width;
			this.height = height;
			this.cellSize = cellSize;
			this.originPosition = originPosition;

			gridArray = new int[width, height];
			
			for(int x = 0; x < gridArray.GetLength(0); x++) {
				for(int z = 0; z < gridArray.GetLength(1); z++) {
					// Draw me some boxes.
					DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1));
					DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z));
				}
			}

			// Draw the outer lines too please.
			DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height));
			DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height));
		}

		// CodeMonkey code, but slightly modified for our use case.
		private Vector3 GetWorldPosition(int x, int z) {
			return new Vector3(x, 0, z) * cellSize + originPosition;
		}

		// CodeMonkey code, but slightly modified for our use case.
		public void GetXZ(Vector3 worldPosition, out int x, out int z) {
			if(DebugGrid) {
				Debug.Log("GetXZ got a vector 3 with x: " + worldPosition.x + ", y: " + worldPosition.y + " and z: " + worldPosition.z + "!");
				Debug.Log("GetXZ got a vector 3 (including origin) with x: " + (worldPosition - originPosition).x + ", y: " + (worldPosition - originPosition).y + " and z: " + (worldPosition - originPosition).z + "!");
			}
			x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
			z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
		}

		// Graciously taken from the interwebs, draws a line like Debug.DrawLine does.
		private void DrawLine(Vector3 start, Vector3 end, float duration = 0.2f) {
			GameObject myLine = new GameObject();
			myLine.transform.position = start;
			myLine.AddComponent<LineRenderer>();
			myLine.name = "GridLine" + start.x + "|" + start.z + "|" + end.x + "|" + end.z;
			LineRenderer lr = myLine.GetComponent<LineRenderer>();
			lr.material = (Material)Resources.Load("Materials/Line");
			lr.startColor = Color.white;
			lr.endColor = Color.white;
			lr.SetWidth(0.1f, 0.1f);
			lr.SetPosition(0, start);
			lr.SetPosition(1, end);
		}
	}
}
