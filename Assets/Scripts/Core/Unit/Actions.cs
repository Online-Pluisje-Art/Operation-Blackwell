using UnityEngine;

namespace OperationBlackwell.Core {
	[System.Serializable]
	public class Actions : IQueueItem {
		public enum ActionType {
			None,
			Move,
			Attack,
			Interact,
		}

		public enum AttackType {
			None,
			Melee,
			RangedEnergy,
			RangedKinetic,
			AOE,
		}

		public ActionType type { get; private set; }
		public AttackType attackType { get; private set; }
		public Tilemap.Node destination { get; private set; }
		public Vector3 destinationPos { get; private set; }
		public Tilemap.Node origin { get; private set; }
		public Vector3 originPos { get; private set; }
		public CoreUnit invoker { get; private set; }
		public CoreUnit target { get; private set; }
		public int cost { get; private set; }
		private bool isComplete_;
		private bool hasExecuted_;

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
			this.isComplete_ = false;
			this.hasExecuted_ = false;
		}

		public Actions(ActionType type, AttackType attatckType, Tilemap.Node destination, Vector3 desitantionPos, Tilemap.Node origin, Vector3 originPos, 
			CoreUnit invoker, CoreUnit target, int cost) {
			this.type = type;
			this.attackType = attatckType;
			this.destination = destination;
			this.destinationPos = desitantionPos;
			this.origin = origin;
			this.originPos = originPos;
			this.invoker = invoker;
			this.target = target;
			this.cost = cost;
			this.isComplete_ = false;
			this.hasExecuted_ = false;
		}

		public void Execute() {
			switch(type) {
				case ActionType.Move:
					// Remove Unit from current Grid Object
					origin.ClearUnitGridCombat();
					invoker.MoveTo(destinationPos, originPos, () => {
						// Set Unit on target Grid Object
						if(destination.GetUnitGridCombat() == null) {
							destination.SetUnitGridCombat(invoker);
						} else {
							Tilemap.Node node = GameController.instance.GetGrid().GetGridObject(invoker.GetPosition());
							node.SetUnitGridCombat(invoker);
						}
						
						GridCombatSystem.UnitEvent unitEvent = new GridCombatSystem.UnitEvent() {
							unit = invoker
						};
						GridCombatSystem.Instance.OnUnitActionPointsChanged?.Invoke(this, unitEvent);
						isComplete_ = true;
					});
					break;
				case ActionType.Attack:
					invoker.AttackUnit(target, attackType, () => {
						GridCombatSystem.UnitEvent unitEvent = new GridCombatSystem.UnitEvent() {
							unit = invoker
						};
						GridCombatSystem.Instance.OnUnitActionPointsChanged?.Invoke(this, unitEvent);
						isComplete_ = true;
					});
					break;
				case ActionType.Interact:
					IInteractable interactable = destination.GetInteractable();
					interactable.Interact(invoker);
					break;
				default:
					break;
			}
			hasExecuted_ = true;
		}

		public bool HasExecuted() {
			return hasExecuted_;
		}

		public bool IsComplete() {
			return isComplete_;
		}

		public int GetTotalCost() {
			return cost;
		}

		public int GetInitiative() {
			return 0;
		}
	}
}
