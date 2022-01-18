using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OperationBlackwell.Core;
using OperationBlackwell.Player;

namespace OperationBlackwell.AI {
	public class AIController : Singleton<AIController> {
		[SerializeField] private List<CombatStage> stages_;
		private CombatStage currentStage_;
		private List<AIUnit> activeUnits_;
		private List<Tilemap.Node> endPoints_;
		private List<int> loadedStages_;

		private void Start() {
			if(stages_.Count == 0) {
				Debug.LogError("No stages found in AIController");
			}
			
			currentStage_ = new CombatStage();
			activeUnits_ = new List<AIUnit>();
			endPoints_ = new List<Tilemap.Node>();
			loadedStages_ = new List<int>();

			// Subscribe to events
			if(GridCombatSystem.instance != null) {
				GridCombatSystem.instance.AIStageLoaded += OnAIStageLoaded;
				GridCombatSystem.instance.AIStageUnloaded += OnAIStageUnloaded;
				GridCombatSystem.instance.AISetTurn += OnAISetTurn;
			}
		}

		private void OnAIStageLoaded(object sender, int stage) {
			LoadStage(stage);
		}

		private void OnAIStageUnloaded(object sender, int stage) {
			UnloadStage();
		}

		private void OnAISetTurn(object sender, System.EventArgs e) {
			SetUnitActionsTurn();
		}

		public void LoadStage(int id) {
			if(currentStage_.ID == id || loadedStages_.Contains(id)) {
				Debug.LogError("Stage already loaded");
				return;
			}
			int index = stages_.FindIndex(x => x.ID == id);
			if(index < 0) {
				Debug.LogError("Stage not found with id: " + id);
				currentStage_ = new CombatStage();
				return;
			}
			currentStage_ = stages_[index];

			List<CoreUnit> unitsToLoad = new List<CoreUnit>();
			foreach(AIUnit unit in currentStage_.units) {
				unit.LoadUnit();
				activeUnits_.Add(unit);
				unitsToLoad.Add(unit);
			}
			GridCombatSystem.instance.LoadAllEnemies(unitsToLoad);
			loadedStages_.Add(currentStage_.ID);
		}

		public void UnloadStage() {
			currentStage_ = new CombatStage();
			activeUnits_.Clear();
		}

		public void SpawnUnits(List<AIUnit> units) {
			List<CoreUnit> unitsToLoad = new List<CoreUnit>();
			foreach(AIUnit unit in units) {
				unit.LoadUnit();
				activeUnits_.Add(unit);
				unitsToLoad.Add(unit);
			}
			GridCombatSystem.instance.LoadAllEnemies(unitsToLoad);
		}

		public void SetUnitActionsTurn() {
			GridCombatSystem combatSystem = GridCombatSystem.instance;
			Grid<Tilemap.Node> grid = GameController.instance.GetGrid();
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
				while(unit.HasActionPoints()) {
					// The <= 1 is because each tile to move has a cost of 2 and otherwise we would end up in a deadlock.
					if(unit.GetActionPoints() <= 1) {
						break;
					}
					foreach(CoreUnit enemy in enemies) {
						if(IsUnitInRange(unit, enemy)) {
							walk = false;
							enemyToAttack = enemy;
							break;
						}
					}

					List<Actions> actions = unit.LoadActions().GetQueue();

					if(walk) {
						Tilemap.Node node;
						Actions unitAction;
						int pathLength;
						if(actions.Count == 0) {
							UpdateValidMovePositions(unit, unit.GetPosition());
							node = SelectEndPoint();
							pathLength = GameController.instance.gridPathfinding.GetPath(unit.GetPosition(), grid.GetPosition(node.gridX, node.gridY)).Count * 2;
							unitAction = new Actions(Actions.ActionType.Move, node, grid.GetPosition(node.gridX, node.gridY),
								grid.GetGridObject(unit.GetPosition()), unit.GetPosition(), unit, null, pathLength);
						} else {
							UpdateValidMovePositions(unit, actions[actions.Count - 1].destinationPos);
							node = SelectEndPoint();
							pathLength = GameController.instance.gridPathfinding.GetPath(actions[actions.Count - 1].destinationPos, grid.GetPosition(node.gridX, node.gridY)).Count * 2;
							unitAction = new Actions(Actions.ActionType.Move, node, grid.GetPosition(node.gridX, node.gridY),
								grid.GetGridObject(actions[actions.Count - 1].destinationPos), actions[actions.Count - 1].destinationPos, unit, null, pathLength);
						}
						unit.SaveAction(unitAction);
					} else {
						Actions.AttackType attackType = unit.GetAttackType();
						Actions unitAction = null;
						int attackCost = unit.GetAttackCost();
						if(actions.Count == 0) {
							unitAction = new Actions(Actions.ActionType.Attack, attackType, grid.GetGridObject(unit.GetPosition()), unit.GetPosition(),
								grid.GetGridObject(unit.GetPosition()), unit.GetPosition(), unit, enemyToAttack, attackCost);
						} else {
							unitAction = new Actions(Actions.ActionType.Attack, attackType, actions[actions.Count - 1].destination, actions[actions.Count - 1].destinationPos,
								actions[actions.Count - 1].destination, actions[actions.Count - 1].destinationPos, unit, enemyToAttack, attackCost);
						}
						unit.SaveAction(unitAction);
					}
				}
				obj = CreateOrderObject(unit);
				combatSystem.AddOrderObject(obj);
			}
			GridCombatSystem.instance.AITurnSet?.Invoke(this, System.EventArgs.Empty);
		}

		private OrderObject CreateOrderObject(AIUnit unit) {
			int cost = 0;
			int initiative = 0;
			List<Actions> actions = unit.LoadActions().GetQueue();
			foreach(Actions action in actions) {
				cost += action.GetTotalCost();
			}
			initiative = GenerateInitiative(cost, 0, 0);
			initiative += activeUnits_.FindIndex(x => x == unit);
			return new OrderObject(initiative, unit, cost);
		}

		private int GenerateInitiative(int cost, int pathLength, int attackRange) {
			int initiative = cost + pathLength + attackRange;
			return initiative;
		}

		private bool IsUnitInRange(AIUnit unit, CoreUnit enemy) {
			if(unit.CanAttackUnit(enemy, unit.GetPosition())) {
				return true;
			}
			return false;
		}

		private void UpdateValidMovePositions(AIUnit unit, Vector3 position) {
			Grid<Tilemap.Node> grid = GameController.instance.GetGrid();
			GridPathfinding gridPathfinding = GameController.instance.gridPathfinding;
			Tilemap.Node node;

			// Get Unit Grid Position X, Y
			grid.GetXY(position, out int unitX, out int unitY);

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
			Grid<Tilemap.Node> grid = GameController.instance.GetGrid();
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
					endPoint = endPoints_[randomIndex];
					endPoints_.RemoveAt(randomIndex);
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
