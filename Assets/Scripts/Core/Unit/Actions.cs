using UnityEngine;

namespace OperationBlackwell.Core {
	public class Actions {
		public enum ActionType {
			None,
			Move,
			Attack,
		}

		public ActionType type { get; private set; }
		public Tilemap.Node destination { get; private set; }
		public Tilemap.Node origin { get; private set; }
		public CoreUnit invoker { get; private set; }
		public CoreUnit target { get; private set; }
		public int cost { get; private set; }

		public Actions(ActionType type, Tilemap.Node destination, Tilemap.Node origin, CoreUnit invoker, CoreUnit target, int cost) {
			this.type = type;
			this.destination = destination;
			this.origin = origin;
			this.invoker = invoker;
			this.target = target;
			this.cost = cost;
		}

		public bool Execute() {
			bool actionSucces = false;
			switch (type) {
				case ActionType.Move:
					invoker.MoveTo(destination.worldPosition, () => {
						// Remove Unit from current Grid Object
						origin.ClearUnitGridCombat();
						// Set Unit on target Grid Object
						destination.SetUnitGridCombat(invoker);
						GridCombatSystem.UnitEvent unitEvent = new GridCombatSystem.UnitEvent() {
							unit = invoker
						};
						GridCombatSystem.Instance.OnUnitActionPointsChanged?.Invoke(this, unitEvent);
						actionSucces = true;
					});
					break;
				case ActionType.Attack:
					break;
				default:
					break;
			}
			return actionSucces;
		}
	}
}