using UnityEngine;

namespace OperationBlackwell.Core {
	public class PuzzleController : MonoBehaviour {
		[SerializeField] private Puzzle[] puzzles;

		private PuzzleBlock emptyPuzzleBlock_;
		[SerializeField] private float puzzleSize_;
		[SerializeField] private GameObject puzzleBlockPrefab_;
		private Vector2 puzzleOffset_;

		private void Start() {
			CreatePuzzle(0);
		}

		public void CreatePuzzle(int puzzleIndex) {
			int blocksPerLine = puzzles[puzzleIndex].BlocksPerLine();
			puzzleOffset_ = new Vector2(
				puzzleSize_ / 2,
				puzzleSize_ / 2
			);
			for(int x = 0; x < blocksPerLine; x++) {
				for(int y = 0; y < blocksPerLine; y++) {
					GameObject puzzleBlock = Instantiate(puzzleBlockPrefab_, new Vector3(0, 0, 0), Quaternion.identity);
					puzzleBlock.transform.position = -Vector2.one * (blocksPerLine - 1) * .5f + 
						new Vector2(x * puzzleSize_ / (blocksPerLine - 1), y * puzzleSize_ / (blocksPerLine - 1)) - puzzleOffset_;
					puzzleBlock.transform.position = new Vector3(puzzleBlock.transform.position.x, puzzleBlock.transform.position.y, -1);
					puzzleBlock.transform.localScale = new Vector3(puzzleSize_ / (blocksPerLine - 1), puzzleSize_ / (blocksPerLine - 1), 1);
					puzzleBlock.transform.SetParent(transform, false);

					PuzzleBlock puzzleBlockScript = puzzleBlock.AddComponent<PuzzleBlock>();
					puzzleBlockScript.OnPuzzleBlockClicked += OnPlayerMoveBlockInput;

					// This removes the bottom right block of the puzzle
					if(y == 0 && x == blocksPerLine - 1) {
						emptyPuzzleBlock_ = puzzleBlockScript;
						puzzleBlock.SetActive(false);
					}
				}
			}
		}

		public void DestroyPuzzle() {
			foreach(Transform child in transform) {
				Destroy(child.gameObject);
			}
		}

		public void OnPlayerMoveBlockInput(object sender, PuzzleBlock block) {
			Vector3 targetPosition = emptyPuzzleBlock_.transform.position;
			emptyPuzzleBlock_.transform.position = block.transform.position;
			block.transform.position = targetPosition;
		}
	}
}
