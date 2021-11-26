using UnityEngine;
using System.Collections;

namespace OperationBlackwell.Core {
	public class Crate : MonoBehaviour, IInteractable {
		[SerializeField] private int range_;
		[SerializeField] private int cost_;
		private void Start() {
			GameController.Instance.GetGrid().GetGridObject(transform.position).SetInteractable(this);
		}

		public void Interact(CoreUnit unit) {
			Debug.Log("Interacting with crate");
		}

		public bool IsInRange(Vector3 unitPos) {
			// Calculate the distance between the two units. But due to the -1 we can attack diagonal units, but also sometimes 1 node extra on the range.
			int nodesBetweenPlayers = GridCombatSystem.Instance.CalculatePoints(unitPos, transform.position).Count - 1;
			return nodesBetweenPlayers <= range_;
		}

		public int GetCost() {
			return cost_;
		}
	}
}
