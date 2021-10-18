using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace OperationBlackwell.Core {
	public class Grid<TGridObject> {
		public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
		public class OnGridObjectChangedEventArgs : EventArgs {
			public int x;
			public int y;
		}

		public int gridSizeX { get; private set; }
		public int gridSizeY { get; private set; }
		public float cellSize { get; private set; }
		private Vector3 originPosition_;
		private TGridObject[,] gridArray_;

		public Grid(int gridSizeX, int gridSizeY, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, Vector3, int, int, TGridObject> createGridObject) {
			this.gridSizeX = gridSizeX;
			this.gridSizeY = gridSizeY;
			this.cellSize = cellSize;
			this.originPosition_ = originPosition;

			gridArray_ = new TGridObject[gridSizeX, gridSizeY];

			for(int x = 0; x < gridArray_.GetLength(0); x++) {
				for(int y = 0; y < gridArray_.GetLength(1); y++) {
					gridArray_[x, y] = createGridObject(this, new Vector3(x, y), x, y);
					// Draw me some boxes.
					Utils.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1));
					Utils.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y));
				}
			}	

			// Draw the outer lines too please.
			Utils.DrawLine(GetWorldPosition(0, gridSizeY), GetWorldPosition(gridSizeX, gridSizeY));
			Utils.DrawLine(GetWorldPosition(gridSizeX, 0), GetWorldPosition(gridSizeX, gridSizeY));
		}

		public int GetWidth() {
			return gridSizeX;
		}

		public int GetHeight() {
			return gridSizeY;
		}

		public float GetCellSize() {
			return cellSize;
		}

		public Vector3 GetWorldPosition(int x, int y) {
			return new Vector3(x, y) * cellSize + originPosition_;
		}

		public void GetXY(Vector3 worldPosition, out int x, out int y) {
			x = Mathf.FloorToInt((worldPosition - originPosition_).x / cellSize);
			y = Mathf.FloorToInt((worldPosition - originPosition_).y / cellSize);
		}

		public void SetGridObject(int x, int y, TGridObject value) {
			if (x >= 0 && y >= 0 && x < gridSizeX && y < gridSizeY) {
				gridArray_[x, y] = value;
				TriggerGridObjectChanged(x, y);
			}
		}

		public void TriggerGridObjectChanged(int x, int y) {
			OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, y = y });
		}

		public void SetGridObject(Vector3 worldPosition, TGridObject value) {
			GetXY(worldPosition, out int x, out int y);
			SetGridObject(x, y, value);
		}

		public TGridObject GetGridObject(int x, int y) {
			if (x >= 0 && y >= 0 && x < gridSizeX && y < gridSizeY) {
				return gridArray_[x, y];
			} else {
				return default(TGridObject);
			}
		}

		public void SetGridObject(int x, int y, TGridObject value) {
			if(x >= 0 && y >= 0 && x < gridSizeX && y < gridSizeY) {
				gridArray_[x, y] = value;
				if (OnGridObjectChanged != null) {
					OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
				}
			}
		}

		public void SetGridObject(Vector3 worldPosition, TGridObject value) {
			int x, y;
			GetXY(worldPosition, out x, out y);
			SetGridObject(x, y, value);
		}

		public TGridObject GetGridObject(int x, int y) {
			if(x >= 0 && y >= 0 && x < gridSizeX && y < gridSizeY) {
				return gridArray_[x, y];
			} else {
				return default(TGridObject);
			}
		}

		public TGridObject GetGridObject(Vector3 worldPosition) {
			int x, y;
			GetXY(worldPosition, out x, out y);
			return GetGridObject(x, y);
		}
	}
}
