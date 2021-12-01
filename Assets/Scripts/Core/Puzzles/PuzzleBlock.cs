using System;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class PuzzleBlock : MonoBehaviour {

		public EventHandler<PuzzleBlock> OnPuzzleBlockClicked;
		public Vector2Int coord;

		public void Init(Texture2D texture, Vector2Int coord) {
			this.coord = coord;
			MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.material.shader = Shader.Find("Unlit/Texture");
			meshRenderer.material.mainTexture = texture;
		}

		private void OnMouseDown() {
			OnPuzzleBlockClicked?.Invoke(this, this);
		}
	}
}
