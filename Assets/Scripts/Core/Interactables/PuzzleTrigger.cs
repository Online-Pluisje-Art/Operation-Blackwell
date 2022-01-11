using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OperationBlackwell.Core {
	public class PuzzleTrigger : MonoBehaviour, IInteractable {
		[SerializeField] private int range_;
		[SerializeField] private int cost_;
		[SerializeField] private int puzzleId_;

		public static event System.EventHandler<int> PuzzleLaunched;

		private List<Transform> children_;
		private bool solved_;

		private void Start() {
			GameController.instance.GetGrid().GetGridObject(transform.position).SetInteractable(this);
			children_ = new List<Transform>();
			solved_ = false;
			foreach(Transform child in transform) {
				children_.Add(child);
			}
			GameController.instance.PuzzleCompleted += OnPuzzleCompleted;
		}

		private void OnDestroy() {
			GameController.instance.PuzzleCompleted -= OnPuzzleCompleted;
		}

		public void Interact(CoreUnit unit) {
			if(solved_ || GridCombatSystem.instance.GetState() != GridCombatSystem.State.OutOfCombat) {
				return;
			}
			GridCombatSystem.instance.SetState(GridCombatSystem.State.Cutscene);
			PuzzleLaunched?.Invoke(this, puzzleId_);
		}

		public bool IsInRange(Vector3 unitPos) {
			// Calculate the distance between the two units. But due to the -1 we can attack diagonal units, but also sometimes 1 node extra on the range.
			int nodesBetweenPlayers = Utils.CalculatePoints(unitPos, transform.position).Count - 1;
			return nodesBetweenPlayers <= range_;
		}

		public int GetCost() {
			return cost_;
		}

		private void OnPuzzleCompleted(object sender, int id) {
			if(puzzleId_ != id) {
				return;
			}
			children_[0].gameObject.SetActive(false);
			children_[1].gameObject.SetActive(true);
			solved_ = true;
		}
	}
}
