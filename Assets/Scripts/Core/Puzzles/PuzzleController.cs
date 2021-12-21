using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace OperationBlackwell.Core {
	public class PuzzleController : MonoBehaviour {
		public static PuzzleController Instance { get; private set; }

		public System.EventHandler<System.EventArgs> PuzzleStarted;
		public System.EventHandler<System.EventArgs> PuzzleEnded;

		private Image background_;

		[Header("Puzzles")]
		[SerializeField] private Puzzle[] puzzles;

		private PuzzleBlock emptyPuzzleBlock_;
		private PuzzleBlock[,] puzzleBlocks_;
		private int blocksPerLine_;
		private enum PuzzleState {
			Inactive,
			Active,
			Solved
		}
		private PuzzleState currentState_;
		private Transform puzzleVictory_;

		[Header("Sizes")]
		[SerializeField] private float puzzleSize_;
		[SerializeField] private GameObject puzzleBlockPrefab_;
		private Vector2 puzzleOffset_;

		private Queue<PuzzleBlock> input_;
		private bool blockIsMoving_;

		[Header("Shuffle")]
		[SerializeField] private int shuffleCount_;
		private int shuffleCountRemaining_;
		Vector2Int prevShuffle_;

		[Header("Move Timings")]
		[SerializeField][Range(0, 1)] private float shuffleMoveTime_;
		[SerializeField][Range(0, 1)] private float defaultMoveTime_;

		private void Awake() {
			if(Instance == null) {
				Instance = this;
			} else {
				Destroy(gameObject);
				return;
			}
		}

		private void Start() {
			foreach(Transform child in transform) {
				puzzleVictory_ = child;
			}
			puzzleVictory_.gameObject.SetActive(false);
			background_ = GetComponent<Image>();
			background_.enabled = false;
		}

		public void CreatePuzzle(int puzzleIndex) {
			blocksPerLine_ = puzzles[puzzleIndex].BlocksPerLine();
			puzzleOffset_ = new Vector2(
				puzzleSize_ / 2,
				puzzleSize_ / 2
			);
			puzzleBlocks_ = new PuzzleBlock[blocksPerLine_, blocksPerLine_];
			Texture2D[,] puzzleSlices = ImageSplicer.SplitImage(
				Utils.TextureFromSprite(puzzles[puzzleIndex].GetSprite()),
				blocksPerLine_
			);
			for(int x = 0; x < blocksPerLine_; x++) {
				for(int y = 0; y < blocksPerLine_; y++) {
					GameObject puzzleBlock = Instantiate(puzzleBlockPrefab_, new Vector3(0, 0, 0), Quaternion.identity);
					puzzleBlock.transform.position = -Vector2.one * (blocksPerLine_ - 1) * .5f +
						new Vector2(x * puzzleSize_ / (blocksPerLine_ - 1), y * puzzleSize_ / (blocksPerLine_ - 1)) - puzzleOffset_;
					puzzleBlock.transform.position = new Vector3(puzzleBlock.transform.position.x, puzzleBlock.transform.position.y, -1);
					puzzleBlock.transform.localScale = new Vector3(puzzleSize_ / (blocksPerLine_ - 1), puzzleSize_ / (blocksPerLine_ - 1), 1);
					puzzleBlock.transform.SetParent(transform, false);

					PuzzleBlock puzzleBlockScript = puzzleBlock.AddComponent<PuzzleBlock>();
					puzzleBlockScript.OnPuzzleBlockClicked += OnPlayerMoveBlockInput;
					puzzleBlockScript.OnPuzzleBlockFinished += OnPuzzleBlockFinished;
					puzzleBlockScript.Init(puzzleSlices[x, y], new Vector2Int(x, y));

					puzzleBlocks_[x, y] = puzzleBlockScript;

					// This removes the bottom right block of the puzzle
					if(y == 0 && x == blocksPerLine_ - 1) {
						emptyPuzzleBlock_ = puzzleBlockScript;
						puzzleBlock.SetActive(false);
					}
				}
			}
			input_ = new Queue<PuzzleBlock>();
			background_.enabled = true;
			PuzzleStarted?.Invoke(this, System.EventArgs.Empty);
			StartShuffle();
		}

		public void DestroyPuzzle() {
			foreach(Transform child in transform) {
				Destroy(child.gameObject);
			}
			background_.enabled = false;
			PuzzleEnded?.Invoke(this, System.EventArgs.Empty);
			GridCombatSystem.Instance.SetState(GridCombatSystem.State.OutOfCombat);
		}

		public void OnPlayerMoveBlockInput(object sender, PuzzleBlock block) {
			if(currentState_ != PuzzleState.Active) {
				return;
			}
			input_.Enqueue(block);
			MakeNextMove();
		}

		public void OnPuzzleBlockFinished(object sender, System.EventArgs e) {
			blockIsMoving_ = false;

			if(currentState_ == PuzzleState.Active) {
				if(IsPuzzleSolved()) {
					currentState_ = PuzzleState.Solved;
					emptyPuzzleBlock_.gameObject.SetActive(true);
					puzzleVictory_.gameObject.SetActive(true);
				} else {
					MakeNextMove();
				}
			}
			

			if(shuffleCountRemaining_ > 0) {
				MakeNextShuffleMove();
			} else {
				currentState_ = PuzzleState.Active;
			}
		}

		private void MoveBlock(PuzzleBlock block, float duration) {
			// Check if block is adjacent to empty block
			if((block.coord - emptyPuzzleBlock_.coord).sqrMagnitude == 1) {
				// Swap blocks
				puzzleBlocks_[block.coord.x , block.coord.y] = emptyPuzzleBlock_;
				puzzleBlocks_[emptyPuzzleBlock_.coord.x, emptyPuzzleBlock_.coord.y] = block;

				Vector2Int emptyCoord = emptyPuzzleBlock_.coord;
				emptyPuzzleBlock_.coord = block.coord;
				block.coord = emptyCoord;

				Vector3 targetPosition = emptyPuzzleBlock_.transform.position;
				emptyPuzzleBlock_.transform.position = block.transform.position;
				block.MoveToPostion(targetPosition, duration);
				blockIsMoving_ = true;
			}
		}

		private void MakeNextMove() {
			while(input_.Count > 0 && !blockIsMoving_) {
				MoveBlock(input_.Dequeue(), defaultMoveTime_);
			}
		}

		private void StartShuffle() {
			shuffleCountRemaining_ = shuffleCount_;
			currentState_ = PuzzleState.Inactive;
			MakeNextShuffleMove();
		}

		private void MakeNextShuffleMove() {
			Vector2Int[] directions = new Vector2Int[] {
				new Vector2Int(0, 1),	// Up
				new Vector2Int(1, 0),	// Right
				new Vector2Int(0, -1),	// Down
				new Vector2Int(-1, 0)	// Left
			};

			int randomIndex = Random.Range(0, directions.Length);

			for(int i = 0; i < directions.Length; i++) {
				Vector2Int randomDirection = directions[(randomIndex + i) % directions.Length];
				if(randomDirection != prevShuffle_ * -1) {
					Vector2Int randomCoord = emptyPuzzleBlock_.coord + randomDirection;

					if(randomCoord.x >= 0 && randomCoord.x < blocksPerLine_ &&
						randomCoord.y >= 0 && randomCoord.y < blocksPerLine_) {
						MoveBlock(puzzleBlocks_[randomCoord.x, randomCoord.y], shuffleMoveTime_);
						shuffleCountRemaining_--;
						prevShuffle_ = randomDirection;
						break;
					}
				}
			}
			
		}

		public bool IsPuzzleSolved() {
			foreach(PuzzleBlock block in puzzleBlocks_) {
				if(!block.IsAtStartingCoord()) {
					return false;
				}
			}
			return true;
		}
	}
}
