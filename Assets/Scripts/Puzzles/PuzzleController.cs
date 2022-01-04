using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using OperationBlackwell.Core;

namespace OperationBlackwell.Puzzles {
	public class PuzzleController : Singleton<PuzzleController> {
		public class PuzzleEndedArgs : System.EventArgs {
			public int id;
		}

		public System.EventHandler<System.EventArgs> PuzzleStarted;

		private Image background_;

		[Header("Puzzles")]
		[SerializeField] private Puzzle[] puzzles;
		[SerializeField] private GameObject puzzleVictory_;
		[SerializeField] private GameObject puzzleTimer_;
		private CircularTimer timerScript_;

		private PuzzleBlock emptyPuzzleBlock_;
		private PuzzleBlock[,] puzzleBlocks_;
		private int blocksPerLine_;
		private enum PuzzleState {
			Inactive,
			Active,
			Solved,
			Failed
		}
		private PuzzleState currentState_;
		private Puzzle currentPuzzle_;

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

		private void Start() {
			PuzzleTrigger.PuzzleLaunched += OnPuzzleStarted;
			puzzleVictory_.SetActive(false);
			puzzleTimer_.SetActive(false);
			timerScript_ = puzzleTimer_.GetComponent<CircularTimer>();
			background_ = GetComponent<Image>();
			background_.enabled = false;
		}

		private void OnDestroy() {
			PuzzleTrigger.PuzzleLaunched -= OnPuzzleStarted;
		}

		private void OnPuzzleStarted(object sender, int id) {
			CreatePuzzle(id);
		}

		public void CreatePuzzle(int puzzleIndex) {
			currentPuzzle_ = puzzles[puzzleIndex];
			blocksPerLine_ = currentPuzzle_.BlocksPerLine();
			puzzleOffset_ = new Vector2(
				puzzleSize_ / 2,
				puzzleSize_ / 2
			);
			puzzleBlocks_ = new PuzzleBlock[blocksPerLine_, blocksPerLine_];
			Texture2D[,] puzzleSlices = ImageSplicer.SplitImage(
				Utils.TextureFromSprite(currentPuzzle_.GetSprite()),
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
			puzzleTimer_.SetActive(true);
			PuzzleStarted?.Invoke(this, System.EventArgs.Empty);
			StartShuffle();
		}

		public void DestroyPuzzle() {
			foreach(Transform child in transform) {
				if(child.gameObject == puzzleVictory_ || child.gameObject == puzzleTimer_) {
					continue;
				}
				Destroy(child.gameObject);
			}
			puzzleVictory_.SetActive(false);
			puzzleTimer_.SetActive(false);
			background_.enabled = false;
			GameController.instance.PuzzleEnded?.Invoke(this, currentPuzzle_.GetID());
			GridCombatSystem.instance.SetState(GridCombatSystem.State.OutOfCombat);
			timerScript_.StopTimer();
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
					puzzleVictory_.SetActive(true);
				} else {
					MakeNextMove();
				}
			}

			if(shuffleCountRemaining_ > 0) {
				MakeNextShuffleMove();
			} else {
				currentState_ = PuzzleState.Active;
				timerScript_.duration = currentPuzzle_.PuzzleDuration();
				timerScript_.StartTimer();
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
			timerScript_.PauseTimer();
			return true;
		}
	}
}
