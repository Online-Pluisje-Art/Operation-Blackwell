using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class MovementTilemap {

		public event EventHandler OnLoaded;
		private Grid<TilemapObject> grid_;

		public MovementTilemap(int width, int height, float cellSize, Vector3 originPosition) {
			grid_ = new Grid<TilemapObject>(width, height, cellSize, originPosition, (Grid<TilemapObject> g, Vector3 v, int x, int y) => new TilemapObject(g, x, y));
		}

		public void SetTilemapSprite(Vector3 worldPosition, TilemapObject.TilemapSprite tilemapSprite) {
			TilemapObject tilemapObject = grid_.GetGridObject(worldPosition);
			if(tilemapObject != null) {
				tilemapObject.SetTilemapSprite(tilemapSprite);
			}
		}

		public void SetTilemapSprite(int x, int y, TilemapObject.TilemapSprite tilemapSprite) {
			TilemapObject tilemapObject = grid_.GetGridObject(x, y);
			if(tilemapObject != null) {
				tilemapObject.SetTilemapSprite(tilemapSprite);
			}
		}

		public void SetRotation(int x, int y, float rotation) {
			TilemapObject tilemapObject = grid_.GetGridObject(x, y);
			if(tilemapObject != null) {
				tilemapObject.SetRotation(rotation);
			}
		}

		public void SetAllTilemapSprite(TilemapObject.TilemapSprite tilemapSprite) {
			for(int x = 0; x < grid_.GetWidth(); x++) {
				for(int y = 0; y < grid_.GetHeight(); y++) {
					SetTilemapSprite(x, y, tilemapSprite);
				}
			}
		}

		public void SetTilemapVisual(MovementTilemapVisual tilemapVisual) {
			tilemapVisual.SetGrid(this, grid_);
		}

		/*
		* Represents a single Tilemap Object that exists in each Grid Cell Position
		* */
		public class TilemapObject {

			public enum TilemapSprite {
				None,
				Move,
				ArrowEnd,
				ArrowCorner,
				ArrowStraight,
				ArrowEndStr,
				ArrowEndCorner,
				ArrowStraightStr,
				ArrowStraightCorner,
				ArrowCornerStr,
				ArrowCornerCorner,
			}

			private Grid<TilemapObject> grid_;
			private int x_;
			private int y_;
			private TilemapSprite tilemapSprite_;
			private float rotation_;

			public TilemapObject(Grid<TilemapObject> grid_, int x, int y) {
				this.grid_ = grid_;
				this.x_ = x;
				this.y_ = y;
			}

			public void SetTilemapSprite(TilemapSprite tilemapSprite) {
				this.tilemapSprite_ = tilemapSprite;
				grid_.TriggerGridObjectChanged(x_, y_);
			}

			public TilemapSprite GetTilemapSprite() {
				return tilemapSprite_;
			}

			public float GetRotation() {
				return rotation_;
			}

			public void SetRotation(float rotation) {
				this.rotation_ = rotation;
				grid_.TriggerGridObjectChanged(x_, y_);
			}

			public override string ToString() {
				return tilemapSprite_.ToString();
			}
		}

	}
}
