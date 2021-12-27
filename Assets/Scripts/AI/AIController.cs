using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OperationBlackwell.Core;
using OperationBlackwell.Player;

namespace OperationBlackwell.AI {
	public class AIController : BaseAIController {
		public static AIController instance { get; private set; }
		[SerializeField] private List<CombatStage> stages_;
		private CombatStage currentStage_;
		private List<AIUnit> activeUnits_;
		private List<Tilemap.Node> endPoints_;

		private void Awake() {
			if(instance == null) {
				instance = this;
			} else {
				Destroy(gameObject);
			}

			if(stages_.Count == 0) {
				Debug.LogError("No stages found in AIController");
			}
			
			currentStage_ = new CombatStage();
			activeUnits_ = new List<AIUnit>();
			endPoints_ = new List<Tilemap.Node>();
		}

		public override void LoadStage(int index) {
			if(currentStage_.ID == index) {
				return;
			}
			bool found = false;
			for(int i = 0; i < stages_.Count; i++) {
				if(stages_[i].ID == index) {
					found = true;
					currentStage_ = stages_[i];
					break;
				}
			}
			if(index < 0 || !found) {
				Debug.LogError("Invalid stage index");
				currentStage_ = new CombatStage();
				return;
			}
			
			List<CoreUnit> unitsToLoad = new List<CoreUnit>();
			foreach(AIUnit unit in currentStage_.units) {
				unit.LoadUnit();
				activeUnits_.Add(unit);
				unitsToLoad.Add(unit);
			}
			GridCombatSystem.Instance.LoadAllEnemies(unitsToLoad);
		}

		public void UnloadStage() {
			currentStage_ = new CombatStage();
			activeUnits_.Clear();
		}

		public override void SetUnitActionsTurn() {
			GridCombatSystem combatSystem = GridCombatSystem.Instance;
			Grid<Tilemap.Node> grid = GameController.Instance.GetGrid();
			List<CoreUnit> enemies = combatSystem.GetBlueTeam();
			bool walk;
			CoreUnit enemyToAttack = null;
			OrderObject obj = null;
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
			foreach(AIUnit unit in activeUnits_) {
				walk = true;
				enemyToAttack = null;
				obj = null;
				if(unit.IsDead()) {
					continue;
				}

				// TODO: Decide the best actions for the unit.
				// For now we will just have the unit move to the nearest enemy. Or attack the nearest enemy.
				// (This depends if the unit has enough action points and if the unit is in range of the enemy)
				// The == 1 is because each tile to move has a cost of 2 and otherwise we would end up in a deadlock.
				while(unit.HasActionPoints() && unit.GetActionPoints() > 1) {
					foreach(CoreUnit enemy in enemies) {
						if(IsUnitInRange(unit, enemy)) {
							walk = false;
							enemyToAttack = enemy;
							break;
						}
					}

					if(walk) {
						UpdateValidMovePositions(unit);
						Tilemap.Node node = SelectEndPoint();
						Actions unitAction;
						int pathLength = GameController.Instance.gridPathfinding.GetPath(unit.GetPosition(), grid.GetPosition(new Vector3(node.gridX, node.gridY))).Count * 2;
						unitAction = new Actions(Actions.ActionType.Move, node, grid.GetPosition(new Vector3(node.gridX, node.gridY)),
							grid.GetGridObject(unit.GetPosition()), unit.GetPosition(), unit, null, pathLength);
						unit.SaveAction(unitAction);
					} else {
						Actions.AttackType attackType = unit.GetAttackType();
						Actions unitAction = null;
						int attackCost = unit.GetAttackCost();
						unitAction = new Actions(Actions.ActionType.Attack, attackType, grid.GetGridObject(unit.GetPosition()), unit.GetPosition(),
							grid.GetGridObject(enemyToAttack.GetPosition()), enemyToAttack.GetPosition(), unit, enemyToAttack, attackCost);
						unit.SaveAction(unitAction);
					}
				}
				obj = CreateOrderObject(unit);
				combatSystem.AddOrderObject(obj);
			}
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

		private bool IsUnitInRange(AIUnit unit, CoreUnit enemy) {
			if(unit.CanAttackUnit(enemy, unit.GetPosition())) {
				return true;
			}
			return false;
		}

		private void UpdateValidMovePositions(AIUnit unit) {
			Grid<Tilemap.Node> grid = GameController.Instance.GetGrid();
			GridPathfinding gridPathfinding = GameController.Instance.gridPathfinding;
			Tilemap.Node node;

			// Get Unit Grid Position X, Y
			grid.GetXY(unit.GetPosition(), out int unitX, out int unitY);

			ResetMoveTiles();

			int maxMoveDistance = unit.GetActionPoints() / 2;
			for(int x = unitX - maxMoveDistance; x <= unitX + maxMoveDistance; x++) {
				for(int y = unitY - maxMoveDistance; y <= unitY + maxMoveDistance; y++) {
					if(x < 0 || x >= grid.GetWidth() || y < 0 || y >= grid.GetHeight()) {
						continue;
					}

					if(gridPathfinding.IsWalkable(x, y) && x != unitX || y != unitY) {
						int length = gridPathfinding.GetPath(unitX, unitY, x, y).Count;
						// Position is Walkable
						if(length > 0 && length <= maxMoveDistance) {
							// There is a Path
							node = grid.GetGridObject(x, y);
							node.SetIsValidMovePosition(true);
							endPoints_.Add(node);
						}
					}
				}
			}
		}

		private void ResetMoveTiles() {
			Grid<Tilemap.Node> grid = GameController.Instance.GetGrid();
			// Reset Entire Grid ValidMovePositions
			for(int x = 0; x < grid.GetWidth(); x++) {
				for(int y = 0; y < grid.GetHeight(); y++) {
					grid.GetGridObject(x, y).SetIsValidMovePosition(false);
				}
			}
			endPoints_.Clear();
		}

		private Tilemap.Node SelectEndPoint() {
			Tilemap.Node endPoint = null;
			//Select random end point from list
			if(endPoints_.Count > 0) {
				do {
					int randomIndex = UnityEngine.Random.Range(0, endPoints_.Count);
					endPoint = endPoints_[0];
					endPoints_.RemoveAt(0);
				} while(endPoint.GetIsValidMovePosition() == false);
			}
			return endPoint;
		}
	}
}

[System.Serializable]
public struct CombatStage {
	public int ID;
	public List<AIUnit> units;
}
