using UnityEngine;
using System.Collections;

namespace OperationBlackwell.Core {
	public class Vent : MonoBehaviour, IInteractable {
		[SerializeField] private int range_;
		[SerializeField] private int cost_;
		[SerializeField] private int offsetX_;
		[SerializeField] private int offsetY_;
		[SerializeReference] private Vent targetVent_;

		private void Start() {
			GameController.instance.GetGrid().GetGridObject(transform.position).SetInteractable(this);
		}

		public void Interact(CoreUnit unit) {
			GridCombatSystem combatInstance = GridCombatSystem.instance;
			CoreUnit player = unit;
			Grid<Tilemap.Node> grid = GameController.instance.GetGrid();
			Vector3 targetPosition = new Vector3(targetVent_.transform.position.x + targetVent_.offsetX_, targetVent_.transform.position.y + targetVent_.offsetY_, 0);
			Tilemap.Node gridObject = grid.GetGridObject(targetPosition);
			// Remove player from current position
			grid.GetGridObject(player.GetPosition()).ClearUnitGridCombat();
			// Set player on target position
			gridObject.SetUnitGridCombat(player);
			player.transform.position = targetPosition;
			combatInstance.OnUnitMove?.Invoke(this, new GridCombatSystem.UnitPositionEvent() {
				unit = player,
				position = targetPosition
			});
			GridCombatSystem.UnitEvent unitEvent = new GridCombatSystem.UnitEvent() {
				unit = player
			};
			player.SetActionPoints(player.GetActionPoints() - cost_);
			combatInstance.OnUnitActionPointsChanged?.Invoke(this, unitEvent);
		}

		public bool IsInRange(Vector3 unitPos) {
			// Calculate the distance between the two units. But due to the -1 we can attack diagonal units, but also sometimes 1 node extra on the range.
			int nodesBetweenPlayers = Utils.CalculatePoints(unitPos, transform.position).Count - 1;
			return nodesBetweenPlayers <= range_;
		}

		public int GetCost() {
			return cost_;
		}
	}
}
