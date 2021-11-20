using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class TilemapVisual : MonoBehaviour {

		[System.Serializable]
		public struct NodeSpriteUV {
			public Tilemap.Node.NodeSprite nodeSprite;
			public Vector2Int uv00Pixels;
			public Vector2Int uv11Pixels;
		}

		private struct UVCoords {
			public Vector2 uv00;
			public Vector2 uv11;
		}
		
		private Grid<Tilemap.Node> grid_;
		private Mesh mesh_;
		private bool updateMesh_;

		[SerializeField] private NodeSpriteUV[] nodeSpriteUVArray_;
		private Dictionary<Tilemap.Node.NodeSprite, UVCoords> uvCoordsDictionary_;

		private void Awake() {
			mesh_ = new Mesh();
			GetComponent<MeshFilter>().mesh = mesh_;

			Texture texture = GetComponent<MeshRenderer>().material.mainTexture;
			float textureWidth = texture.width;
			float textureHeight = texture.height;

			uvCoordsDictionary_ = new Dictionary<Tilemap.Node.NodeSprite, UVCoords>();

			foreach(NodeSpriteUV nodeSpriteUV in nodeSpriteUVArray_) {
				uvCoordsDictionary_[nodeSpriteUV.nodeSprite] = new UVCoords {
					uv00 = new Vector2(nodeSpriteUV.uv00Pixels.x / textureWidth, nodeSpriteUV.uv00Pixels.y / textureHeight),
					uv11 = new Vector2(nodeSpriteUV.uv11Pixels.x / textureWidth, nodeSpriteUV.uv11Pixels.y / textureHeight),
				};
			}
		}

		public void SetGrid(Tilemap tilemap, Grid<Tilemap.Node> grid) {
			this.grid_ = grid;
			UpdateNodeVisual();

			grid_.OnGridObjectChanged += Grid_OnGridValueChanged;
			tilemap.OnLoaded += TilemapVisual_OnLoaded;
		}

		private void Grid_OnGridValueChanged(object sender, Grid<Tilemap.Node>.OnGridObjectChangedEventArgs e) {
			updateMesh_ = true;
		}

		private void TilemapVisual_OnLoaded(object sender, System.EventArgs e) {
			updateMesh_ = true;
		}

		private void LateUpdate() {
			if(updateMesh_) {
				updateMesh_ = false;
				UpdateNodeVisual();
			}
		}

		private void UpdateNodeVisual() {
			Utils.CreateEmptyMeshArrays(grid_.gridSizeX * grid_.gridSizeY, out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

			for(int x = 0; x < grid_.gridSizeX; x++) {
				for(int y = 0; y < grid_.gridSizeY; y++) {
					int index = x * grid_.gridSizeY + y;
					Vector3 quadSize = new Vector3(1, 1) * grid_.cellSize;

					Tilemap.Node node;
					Vector3 worldPosition = grid_.GetWorldPosition(x, y);
					node = grid_.GetGridObject(worldPosition);
					Tilemap.Node.NodeSprite nodeSprite = node.GetNodeSprite();
					Vector2 gridUV00, gridUV11;
					if (nodeSprite == Tilemap.Node.NodeSprite.NONE) {
						gridUV00 = Vector2.zero;
						gridUV11 = Vector2.zero;
						quadSize = Vector3.zero;
					} else {
						UVCoords uvCoords = uvCoordsDictionary_[nodeSprite];
						gridUV00 = uvCoords.uv00;
						gridUV11 = uvCoords.uv11;
					}
					Utils.AddToMeshArrays(vertices, uv, triangles, index, worldPosition + quadSize * .5f, 0f, quadSize, gridUV00, gridUV11);
				}
			}
			mesh_.vertices = vertices;
			mesh_.uv = uv;
			mesh_.triangles = triangles;
		}
	}
}
