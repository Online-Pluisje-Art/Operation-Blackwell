using UnityEngine;

namespace OperationBlackwell.Core {
	[CreateAssetMenu(fileName = "New Puzzle", menuName = "Puzzles/Puzzle")]
	public class Puzzle : ScriptableObject {
		public enum PuzzleDifficulty {
			Easy,
			Medium,
			Hard
		}
		[SerializeField] private Sprite puzzleImage_;
		[SerializeField] private int puzzleID_;
		[SerializeField] private PuzzleDifficulty puzzleDifficulty_;

		public Sprite GetSprite() {
			return puzzleImage_;
		}

		public int GetID() {
			return puzzleID_;
		}

		public int BlocksPerLine() {
			switch (puzzleDifficulty_) {
				case PuzzleDifficulty.Easy:
					return 3;
				case PuzzleDifficulty.Medium:
					return 4;
				case PuzzleDifficulty.Hard:
					return 5;
				default:
					return 0;
			}
		}
	}
}
