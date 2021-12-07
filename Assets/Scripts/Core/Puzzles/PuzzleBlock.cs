using System;
using UnityEngine;
using System.Collections;

namespace OperationBlackwell.Core {
	public class PuzzleBlock : MonoBehaviour, IQueueItem {

		public EventHandler<PuzzleBlock> OnPuzzleBlockClicked;
		public EventHandler<EventArgs> OnPuzzleBlockFinished;
		public Vector2Int coord;
		private Vector2Int startCoord_;

		public void Init(Texture2D texture, Vector2Int coord) {
			this.startCoord_ = coord;
			this.coord = coord;
			MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.material.shader = Shader.Find("Unlit/Texture");
			meshRenderer.material.mainTexture = texture;
		}

		private void OnMouseDown() {
			OnPuzzleBlockClicked?.Invoke(this, this);
		}

		public void MoveToPostion(Vector3 target, float duration) {
			StartCoroutine(AnimateMovement(target, duration));
		}

		IEnumerator AnimateMovement(Vector3 target, float duration) {
			float startTime = Time.time;
			Vector3 startPosition = transform.position;
			while(Time.time - startTime < duration) {
				float t = (Time.time - startTime) / duration;
				transform.position = Vector3.Lerp(startPosition, target, t);
				yield return null;
			}
			transform.position = target;
			OnPuzzleBlockFinished?.Invoke(this, EventArgs.Empty);
		}

		public bool IsAtStartingCoord() {
			return coord == startCoord_;
		}


		// These functions are required by the IQueueItem interface, but not used in this class.
		public int GetInitiative() {
			return coord.x + coord.y;
		}

		public int GetTotalCost() {
			return coord.x + coord.y;
		}
	}
}
