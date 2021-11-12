using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class MovementTilemapVisual : MonoBehaviour {

		[System.Serializable]
		public struct TilemapSpriteUV {
			public MovementTilemap.TilemapObject.TilemapSprite tilemapSprite;
			public Vector2Int uv00Pixels;
			public Vector2Int uv11Pixels;
		}

		private struct UVCoords {
			public Vector2 uv00;
			public Vector2 uv11;
		}

		[SerializeField] private TilemapSpriteUV[] tilemapSpriteUVArray_;
		private Grid<MovementTilemap.TilemapObject> grid_;
		private Mesh mesh_;
		private bool updateMesh_;
		private Dictionary<MovementTilemap.TilemapObject.TilemapSprite, UVCoords> uvCoordsDictionary_;

		private void Awake() {
			mesh_ = new Mesh();
			GetComponent<MeshFilter>().mesh = mesh_;

			Texture texture = GetComponent<MeshRenderer>().material.mainTexture;
			float textureWidth = texture.width;
			float textureHeight = texture.height;

			uvCoordsDictionary_ = new Dictionary<MovementTilemap.TilemapObject.TilemapSprite, UVCoords>();

			foreach(TilemapSpriteUV tilemapSpriteUV in tilemapSpriteUVArray_) {
				uvCoordsDictionary_[tilemapSpriteUV.tilemapSprite] = new UVCoords {
					uv00 = new Vector2(tilemapSpriteUV.uv00Pixels.x / textureWidth, tilemapSpriteUV.uv00Pixels.y / textureHeight),
					uv11 = new Vector2(tilemapSpriteUV.uv11Pixels.x / textureWidth, tilemapSpriteUV.uv11Pixels.y / textureHeight),
				};
			}
		}

		public void SetGrid(MovementTilemap tilemap, Grid<MovementTilemap.TilemapObject> grid_) {
			this.grid_ = grid_;
			UpdateHeatMapVisual();

			grid_.OnGridObjectChanged += Grid_OnGridValueChanged;
			tilemap.OnLoaded += Tilemap_OnLoaded;
		}

		private void Tilemap_OnLoaded(object sender, System.EventArgs e) {
			updateMesh_ = true;
		}

		private void Grid_OnGridValueChanged(object sender, Grid<MovementTilemap.TilemapObject>.OnGridObjectChangedEventArgs e) {
			updateMesh_ = true;
		}

		private void LateUpdate() {
			if(updateMesh_) {
				updateMesh_ = false;
				UpdateHeatMapVisual();
			}
		}

		private void UpdateHeatMapVisual() {
			Utils.CreateEmptyMeshArrays(grid_.GetWidth() * grid_.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

			for(int x = 0; x < grid_.GetWidth(); x++) {
				for(int y = 0; y < grid_.GetHeight(); y++) {
					int index = x * grid_.GetHeight() + y;
					Vector3 quadSize = new Vector3(1, 1) * grid_.GetCellSize();

					MovementTilemap.TilemapObject gridObject = grid_.GetGridObject(x, y);
					MovementTilemap.TilemapObject.TilemapSprite tilemapSprite = gridObject.GetTilemapSprite();
					Vector2 gridUV00, gridUV11;
					if(tilemapSprite == MovementTilemap.TilemapObject.TilemapSprite.None) {
						gridUV00 = Vector2.zero;
						gridUV11 = Vector2.zero;
						quadSize = Vector3.zero;
					} else {
						UVCoords uvCoords = uvCoordsDictionary_[tilemapSprite];
						gridUV00 = uvCoords.uv00;
						gridUV11 = uvCoords.uv11;
					}
					Utils.AddToMeshArrays(vertices, uv, triangles, index, grid_.GetWorldPosition(x, y) + quadSize * .5f, gridObject.GetRotation(), quadSize, gridUV00, gridUV11);
				}
			}

			mesh_.vertices = vertices;
			mesh_.uv = uv;
			mesh_.triangles = triangles;
		}
	}
}
