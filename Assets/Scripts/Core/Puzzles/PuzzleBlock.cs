using System;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class PuzzleBlock : MonoBehaviour {

		public EventHandler<PuzzleBlock> OnPuzzleBlockClicked;

		private void OnMouseDown() {
			OnPuzzleBlockClicked?.Invoke(this, this);
		}
	}
}
