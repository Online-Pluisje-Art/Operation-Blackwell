using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OperationBlackwell.Core;
using OperationBlackwell.Player;

namespace OperationBlackwell.AI {
	public class AIController : MonoBehaviour {
		public static AIController instance { get; private set; }
		[SerializeField] private List<CombatStage> stages_;
		private CombatStage currentStage_;
		private List<AIUnit> activeUnits_;

		private void Awake() {
			if(instance == null) {
				instance = this;
			} else {
				Destroy(gameObject);
			}

			if(stages_.Count == 0) {
				Debug.LogError("No stages found in AIController");
			}

			currentStage_ = null;
			activeUnits_ = new List<AIUnit>();
		}

		public void LoadStage(int index) {
			if(index < 0 || index >= stages_.Count) {
				Debug.LogError("Invalid stage index");
				currentStage_ = null;
				return;
			}

			currentStage_ = stages_[index];
			foreach(AIUnit unit in currentStage_.GetAIUnits()) {
				unit.LoadUnit();
				activeUnits_.Add(unit);
			}
		}

		public void UnloadStage() {
			currentStage_ = null;
			activeUnits_.Clear();
		}

		public void SetUnitActionsTurn() {
			GridCombatSystem combatSystem = GridCombatSystem.Instance;
			// List<CoreUnit> enemies = combatSystem.GetBlueTeam();
			/**
			 * Loop through all units.
			 * If the unit is not dead, decide the best actions for the unit. 
			 * * (the order of the actions matters as it executes them in order)
			 * ! For now we will just have the unit move to the nearest enemy. Or attack the nearest enemy. 
			 * ! (This depends if the unit has enough action points and if the unit is in range of the enemy)
			 * Set the actions of that unit in the actions variable of the unit.
			 * After all the actions of a unit are set, make a orderobject for the unit.
			 * Add the orderObject to the list in gridcombatsystem.
			 */
			// foreach(AIUnit unit in activeUnits_) {
			// 	if(unit.IsDead()) {
			// 		continue;
			// 	}

			// 	// TODO: Decide the best actions for the unit.
			// 	// For now we will just have the unit move to the nearest enemy. Or attack the nearest enemy.
			// 	// (This depends if the unit has enough action points and if the unit is in range of the enemy)
			// 	while(unit.HasActionPoints()) {
					
			// 	}
			// }
		}

		private OrderObject CreateOrderObject(AIUnit unit) {
			int cost = 0;
			int initiative = 0;
			List<Actions> actions = unit.LoadActions().GetQueue();
			foreach(Actions action in actions) {
				cost += action.GetTotalCost();
			}
			initiative = GenerateInitiative(cost, 0, 0);
			return new OrderObject(initiative, unit, cost);
		}

		private int GenerateInitiative(int cost, int pathLength, int attackRange) {
			int initiative = UnityEngine.Random.Range(1, 10);
			return initiative;
		}
	}
}
