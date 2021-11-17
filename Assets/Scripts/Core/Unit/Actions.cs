using UnityEngine;
using System.Threading.Tasks;

namespace OperationBlackwell.Core {
	public class Actions {
		public enum ActionType {
			None,
			Move,
			Attack,
		}

		public ActionType type { get; private set; }
		public Tilemap.Node destination { get; private set; }
		public Vector3 destinationPos { get; private set; }
		public Tilemap.Node origin { get; private set; }
		public Vector3 originPos { get; private set; }
		public CoreUnit invoker { get; private set; }
		public CoreUnit target { get; private set; }
		public int cost { get; private set; }

		private bool success_;

		public Actions(ActionType type, Tilemap.Node destination, Vector3 desitantionPos, Tilemap.Node origin, Vector3 originPos, 
			CoreUnit invoker, CoreUnit target, int cost) {
			this.type = type;
			this.destination = destination;
			this.destinationPos = desitantionPos;
			this.origin = origin;
			this.originPos = originPos;
			this.invoker = invoker;
			this.target = target;
			this.cost = cost;
		}

		public bool Execute() {
			success_ = false;
			switch (type) {
				case ActionType.Move:
					invoker.MoveTo(destinationPos, () => {
						// Remove Unit from current Grid Object
						origin.ClearUnitGridCombat();
						// Set Unit on target Grid Object
						destination.SetUnitGridCombat(invoker);
						GridCombatSystem.UnitEvent unitEvent = new GridCombatSystem.UnitEvent() {
							unit = invoker
						};
						GridCombatSystem.Instance.OnUnitActionPointsChanged?.Invoke(this, unitEvent);
					});
					if(success_) {
						Debug.Log("Movement Successful");
					}
					break;
				case ActionType.Attack:
					break;
				default:
					break;
			}
			return success_;
		}
	}
}
