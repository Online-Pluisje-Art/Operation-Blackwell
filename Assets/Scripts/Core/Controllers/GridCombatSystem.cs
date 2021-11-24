using System.Collections.Generic;
using UnityEngine;
using System;

namespace OperationBlackwell.Core {
	public class GridCombatSystem : MonoBehaviour {

		public static GridCombatSystem Instance { get; private set; }
		[SerializeField] private CoreUnit[] unitGridCombatArray_;

		private State state_;
		private CoreUnit unitGridCombat_;
		private List<CoreUnit> blueTeamList_;
		private List<CoreUnit> redTeamList_;
		private int blueTeamActiveUnitIndex_;
		private int redTeamActiveUnitIndex_;

		private List<PathNode> currentPathUnit_;
		private int pathLength_;
		private int turn_;
		private List<OrderObject> orderList_;

		public EventHandler<EventArgs> OnUnitDeath;
		public EventHandler<UnitPositionEvent> OnUnitSelect;
		public EventHandler<UnitPositionEvent> OnUnitMove;
		public EventHandler<UnitEvent> OnUnitActionPointsChanged;
		public EventHandler<string> OnWeaponChanged;
		public EventHandler<int> OnTurnEnded;

		public class UnitEvent : EventArgs {
			public CoreUnit unit;
		}

		public class UnitPositionEvent : UnitEvent {
			public Vector3 position;
		}

		private enum State {
			Normal,
			UnitSelected,
			EndingTurn,
			Waiting
		}

		private enum MouseButtons {
			Leftclick,
			Rightclick,
			Middlemouseclick
		}

		private void Awake() {
			Instance = this;
			state_ = State.Normal;
			OnUnitDeath += RemoveUnitOnDeath;
		}

		private void Start() {
			turn_ = 1;
			blueTeamList_ = new List<CoreUnit>();
			redTeamList_ = new List<CoreUnit>();
			blueTeamActiveUnitIndex_ = -1;
			redTeamActiveUnitIndex_ = -1;

			// Set all UnitGridCombat on their GridPosition
			foreach(CoreUnit unitGridCombat_ in unitGridCombatArray_) {
				GameController.Instance.GetGrid().GetGridObject(unitGridCombat_.GetPosition())
					.SetUnitGridCombat(unitGridCombat_);

				if(unitGridCombat_.GetTeam() == Team.Blue) {
					blueTeamList_.Add(unitGridCombat_);
				} else {
					redTeamList_.Add(unitGridCombat_);
				}
			}

			orderList_ = new List<OrderObject>();

			OnUnitSelect?.Invoke(this, new UnitPositionEvent() {
				unit = blueTeamList_[0],
				position = blueTeamList_[0].GetPosition()
			});

			OnTurnEnded?.Invoke(this, turn_);
		}

		private void OnDestroy() {
			OnUnitDeath -= RemoveUnitOnDeath;
		}

		private void RemoveUnitOnDeath(object sender, EventArgs e) {
			CoreUnit unit = (CoreUnit)sender;
			if(unit.GetTeam() == Team.Blue) {
				blueTeamList_.Remove(unit);
			} else {
				redTeamList_.Remove(unit);
			}
			Grid<Tilemap.Node> grid = GameController.Instance.GetGrid();
			Tilemap.Node gridObject = grid.GetGridObject(unit.GetPosition());
			gridObject.ClearUnitGridCombat();
		}

		private void SelectNextActiveUnit() {
			if(unitGridCombat_ == null || unitGridCombat_.GetTeam() == Team.Red) {
				unitGridCombat_ = GetNextActiveUnit(Team.Blue);
			} else {
				unitGridCombat_ = GetNextActiveUnit(Team.Red);
			}

			UnitEvent unitEvent = new UnitEvent() {
				unit = unitGridCombat_
			};
			OnUnitActionPointsChanged?.Invoke(this, unitEvent);
			OnWeaponChanged?.Invoke(this, unitGridCombat_.GetActiveWeapon());
		}

		private CoreUnit GetNextActiveUnit(Team team) {
			if(team == Team.Blue) {
				if(blueTeamList_.Count == 0) {
					return GetNextActiveUnit(Team.Red);
				} else {
					blueTeamActiveUnitIndex_ = (blueTeamActiveUnitIndex_ + 1) % blueTeamList_.Count;
					return GetUnitTeam(blueTeamList_, blueTeamActiveUnitIndex_, team);
				}
			} else {
				if(redTeamList_.Count == 0) {
					return GetNextActiveUnit(Team.Blue);
				} else {
					redTeamActiveUnitIndex_ = (redTeamActiveUnitIndex_ + 1) % redTeamList_.Count;
					return GetUnitTeam(redTeamList_, redTeamActiveUnitIndex_, team);
				}
			}
		}

		private CoreUnit GetUnitTeam(List<CoreUnit> teamList, int index, Team team) {
			if(index < 0 || index >= teamList.Count) {
				return null;
			}
			if(teamList[index] == null || teamList[index].IsDead()) {
				// Unit is Dead, get next one
				return GetNextActiveUnit(team);
			} else {
				OnUnitSelect?.Invoke(this, new UnitPositionEvent() {
					unit = teamList[index],
					position = teamList[index].GetPosition()
				});
				return teamList[index];
			}
		}

		public void UpdateValidMovePositions(Vector3 position) {
			Grid<Tilemap.Node> grid = GameController.Instance.GetGrid();
			GridPathfinding gridPathfinding = GameController.Instance.gridPathfinding;

			// Get Unit Grid Position X, Y
			grid.GetXY(position, out int unitX, out int unitY);

			// Set entire Tilemap to Invisible
			GameController.Instance.GetMovementTilemap().SetAllTilemapSprite(
				MovementTilemap.TilemapObject.TilemapSprite.None
			);

			// Reset Entire Grid ValidMovePositions
			for(int x = 0; x < grid.GetWidth(); x++) {
				for(int y = 0; y < grid.GetHeight(); y++) {
					grid.GetGridObject(x, y).SetIsValidMovePosition(false);
				}
			}

			int maxMoveDistance = unitGridCombat_.GetActionPoints() + 1;
			for(int x = unitX - maxMoveDistance; x <= unitX + maxMoveDistance; x++) {
				for(int y = unitY - maxMoveDistance; y <= unitY + maxMoveDistance; y++) {
					if(x < 0 || x >= grid.GetWidth() || y < 0 || y >= grid.GetHeight()) {
						continue;
					}

					if(gridPathfinding.IsWalkable(x, y) && x != unitX || y != unitY) {
						// Position is Walkable
						if(gridPathfinding.HasPath(unitX, unitY, x, y)) {
							// There is a Path
							if(gridPathfinding.GetPath(unitX, unitY, x, y).Count <= maxMoveDistance) {
								// Path within Move Distance

								// Set Tilemap Tile to Move
								GameController.Instance.GetMovementTilemap().SetTilemapSprite(
									x, y, MovementTilemap.TilemapObject.TilemapSprite.Move
								);

								grid.GetGridObject(x, y).SetIsValidMovePosition(true);
							} else {
								// Path outside Move Distance!
							}
						} else {
							// No valid Path
						}
					} else {
						// Position is not Walkable
					}
				}
			}
		}

		private void LateUpdate() {
			Grid<Tilemap.Node> grid = GameController.Instance.GetGrid();
			Tilemap.Node gridObject = grid.GetGridObject(Utils.GetMouseWorldPosition());
			List<Actions> actions = new List<Actions>();
			if(unitGridCombat_ != null) {
				actions = unitGridCombat_.LoadActions().GetQueue();
			}
			
			if(gridObject != null) {
				CoreUnit unit = gridObject.GetUnitGridCombat();
				if(actions.Count == 0) {
					if(unit != null && unitGridCombat_ != null && unitGridCombat_.CanAttackUnit(unit, Vector3.zero)
						&& unit != unitGridCombat_ && unit.GetTeam() != unitGridCombat_.GetTeam()) {
						CursorController.Instance.SetActiveCursorType(CursorController.CursorType.Attack);
					} else if(gridObject.GetIsValidMovePosition() && unitGridCombat_ != null && state_ == State.UnitSelected) {
						CursorController.Instance.SetActiveCursorType(CursorController.CursorType.Move);
					} else if(unit != null && unit.GetTeam() == Team.Blue && (state_ == State.Normal || state_ == State.UnitSelected)) {
						CursorController.Instance.SetActiveCursorType(CursorController.CursorType.Select);
					} else {
						CursorController.Instance.SetActiveCursorType(CursorController.CursorType.Arrow);
					}
				} else if(actions.Count > 0) {
					if(unit != null && unitGridCombat_ != null && unitGridCombat_.CanAttackUnit(unit, actions[actions.Count - 1].destinationPos)
						&& unit != unitGridCombat_ && unit.GetTeam() != unitGridCombat_.GetTeam()) {
						CursorController.Instance.SetActiveCursorType(CursorController.CursorType.Attack);
					} else if(gridObject.GetIsValidMovePosition() && unitGridCombat_ != null && state_ == State.UnitSelected) {
						CursorController.Instance.SetActiveCursorType(CursorController.CursorType.Move);
					} else if(unit != null && unit.GetTeam() == Team.Blue && (state_ == State.Normal || state_ == State.UnitSelected)) {
						CursorController.Instance.SetActiveCursorType(CursorController.CursorType.Select);
					} else {
						CursorController.Instance.SetActiveCursorType(CursorController.CursorType.Arrow);
					}
				}
			}
		}

		private void Update() {
			HandleWeaponSwitch();
			GameController.Instance.GetSelectorTilemap().SetAllTilemapSprite(
				MovementTilemap.TilemapObject.TilemapSprite.None
			);

			Grid<Tilemap.Node> grid;
			Tilemap.Node gridObject;
			CoreUnit unit;
			switch(state_) {
				case State.Normal:
					grid = GameController.Instance.GetGrid();
					gridObject = grid.GetGridObject(Utils.GetMouseWorldPosition());

					if(gridObject == null) {
						return;
					}
					
					unit = gridObject.GetUnitGridCombat();
					GameController.Instance.GetSelectorTilemap().SetTilemapSprite(
						gridObject.gridX, gridObject.gridY, MovementTilemap.TilemapObject.TilemapSprite.Move
					);
					if(unit != null && unit.GetTeam() == Team.Blue) {
						if(Input.GetMouseButtonDown((int)MouseButtons.Leftclick)) {
							if(unit != null && unit.GetTeam() == Team.Blue) {
								OnUnitSelect?.Invoke(this, new UnitPositionEvent() {
									unit = unit,
									position = unit.GetPosition()
								});
								unitGridCombat_ = unit;
								state_ = State.UnitSelected;
							}
						}
					}
					break;
				case State.UnitSelected:
					UnitEvent unitEvent = new UnitEvent() {
						unit = unitGridCombat_
					};
					OnUnitActionPointsChanged?.Invoke(this, unitEvent);

					grid = GameController.Instance.GetGrid();
					gridObject = grid.GetGridObject(Utils.GetMouseWorldPosition());
					
					if(gridObject == null) {
						return;
					}

					GameController.Instance.GetSelectorTilemap().SetTilemapSprite(
						gridObject.gridX, gridObject.gridY, MovementTilemap.TilemapObject.TilemapSprite.Move
					);

					ResetMoveTiles();
					ResetArrowVisual();
					// Set arrow to target position
					List<Actions> actions = unitGridCombat_.LoadActions().GetQueue();
					if(unitGridCombat_ != null && actions.Count == 0) {
						UpdateValidMovePositions(unitGridCombat_.GetPosition());
						if(gridObject.GetIsValidMovePosition()) {
							SetArrowWithPath(Vector3.zero, Vector3.zero);
						}
					} else {
						foreach(Actions action in actions) {
							if(action.type == Actions.ActionType.Move) {
								ResetMoveTiles();
								SetArrowWithPath(action.originPos, action.destinationPos);
								UpdateValidMovePositions(action.destinationPos);
							}
						}
						if(gridObject.GetIsValidMovePosition() && actions.Count != 0) {
							SetArrowWithPath(actions[actions.Count - 1].destinationPos, Utils.GetMouseWorldPosition());
						}
					}

					unit = gridObject.GetUnitGridCombat();
					if(unit != null && unit.GetTeam() == Team.Blue && unit != unitGridCombat_) {
						if(Input.GetMouseButtonDown((int)MouseButtons.Leftclick)) {
							if(unit != null && unit.GetTeam() == Team.Blue) {
								OnUnitSelect?.Invoke(this, new UnitPositionEvent() {
									unit = unit,
									position = unit.GetPosition()
								});
								unitGridCombat_ = unit;
								state_ = State.UnitSelected;
							}
						}
					} else if(gridObject.GetIsValidMovePosition()) {
						// Draw arrow acording to pathfinding path
						if(Input.GetMouseButtonDown((int)MouseButtons.Rightclick)) {
							// Save the actions for the unit
							if(gridObject.GetIsValidMovePosition()) {
								// Valid Move Position
								if(unitGridCombat_.HasActionPoints()) {
									state_ = State.Waiting;

									// Set entire Tilemap to Invisible
									GameController.Instance.GetMovementTilemap().SetAllTilemapSprite(
										MovementTilemap.TilemapObject.TilemapSprite.None
									);
									CursorController.Instance.SetActiveCursorType(CursorController.CursorType.Arrow);

									Actions unitAction = null;
									if(actions.Count == 0) {
										pathLength_ = GameController.Instance.gridPathfinding.GetPath(unitGridCombat_.GetPosition(), Utils.GetMouseWorldPosition()).Count - 1;
										unitAction = new Actions(Actions.ActionType.Move, gridObject, Utils.GetMouseWorldPosition(),
											grid.GetGridObject(unitGridCombat_.GetPosition()), unitGridCombat_.GetPosition(), unitGridCombat_, null, pathLength_);
									} else {
										pathLength_ = GameController.Instance.gridPathfinding.GetPath(actions[actions.Count - 1].destinationPos, Utils.GetMouseWorldPosition()).Count - 1;
										unitAction = new Actions(Actions.ActionType.Move, gridObject, Utils.GetMouseWorldPosition(),
											grid.GetGridObject(actions[actions.Count - 1].destinationPos), actions[actions.Count - 1].destinationPos, unitGridCombat_, null, pathLength_);
									}
									unitGridCombat_.SaveAction(unitAction);

									OrderObject unitOrder = GetOrderObject(unitGridCombat_);
									if(unitOrder == null) {
										int cost = GenerateTotalCost(0, pathLength_, 0);
										int initiative = GenerateInitiative(cost, pathLength_, 0);
										unitOrder = new OrderObject(initiative, unitGridCombat_, cost);
										orderList_.Add(unitOrder);
									} else {
										int newCost = GenerateTotalCost(unitOrder.GetTotalCost(), pathLength_, 0);
										int newInitiative = GenerateInitiative(newCost, pathLength_, 0);
										unitOrder.SetTotalCost(newCost);
										unitOrder.SetInitiative(newInitiative);
									}
									if(UnitsHaveActionsPoints()) {
										state_ = State.UnitSelected;
									} else {
										state_ = State.EndingTurn;
									}
								}
							}
						} else if(Input.GetMouseButtonDown((int)MouseButtons.Leftclick)) {
							DeselectUnit();
						}
					} else if(gridObject.GetUnitGridCombat() != null && gridObject.GetUnitGridCombat().GetTeam() != Team.Blue) {
						if(Input.GetMouseButtonDown((int)MouseButtons.Rightclick)) {
							// Can Attack Enemy
							int attackCost = unitGridCombat_.GetAttackCost();
							if(unitGridCombat_.GetActionPoints() >= attackCost) {
								// Attack Enemy
								state_ = State.Waiting;
								Actions.AttackType attackType = unitGridCombat_.GetAttackType();
								Actions unitAction = null;
								if(actions.Count == 0) {
									if(unitGridCombat_.CanAttackUnit(unit, Vector3.zero)) {
										unitAction = new Actions(Actions.ActionType.Attack, attackType, gridObject, Utils.GetMouseWorldPosition(),
											grid.GetGridObject(unitGridCombat_.GetPosition()), unitGridCombat_.GetPosition(), unitGridCombat_, unit, attackCost);
									}
								} else {
									if(unitGridCombat_.CanAttackUnit(unit, actions[actions.Count - 1].destinationPos)) {
										unitAction = new Actions(Actions.ActionType.Attack, attackType, gridObject, Utils.GetMouseWorldPosition(),
											grid.GetGridObject(actions[actions.Count - 1].destinationPos), actions[actions.Count - 1].destinationPos, unitGridCombat_, unit, attackCost);
									}
								}
								unitGridCombat_.SaveAction(unitAction);

								OrderObject unitOrder = GetOrderObject(unitGridCombat_);
								if(unitOrder == null) {
									int cost = GenerateTotalCost(0, 0, 0);
									int initiative = GenerateInitiative(cost, 0, 0);
									unitOrder = new OrderObject(initiative, unitGridCombat_, cost);
									orderList_.Add(unitOrder);
								} else {
									int newCost = GenerateTotalCost(unitOrder.GetTotalCost(), 0, 0);
									int newInitiative = GenerateInitiative(newCost, 0, 0);
									unitOrder.SetTotalCost(newCost);
									unitOrder.SetInitiative(newInitiative);
								}
								if(UnitsHaveActionsPoints()) {
									state_ = State.UnitSelected;
								} else {
									state_ = State.EndingTurn;
								}
							}
						}
					} else if(!gridObject.GetIsValidMovePosition()) {
						if(Input.GetMouseButtonDown((int)MouseButtons.Leftclick)) {
							DeselectUnit();
						}
					}
					break;
				case State.EndingTurn:
					ForceTurnOver();
					break;
				default:
					break;
			}

			if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
				// End Turn
				state_ = State.EndingTurn;
				return;
			}
			if(Input.GetKeyDown(KeyCode.R)) {
				if(unitGridCombat_ != null) {
					unitGridCombat_.ClearActions();
				}
			}
		}

		private void HandleWeaponSwitch() {
			if(Input.GetKeyDown(KeyCode.Alpha1)) {
				unitGridCombat_.SetActiveWeapon(0);
				OnWeaponChanged?.Invoke(this, unitGridCombat_.GetActiveWeapon());
			}
			if(Input.GetKeyDown(KeyCode.Alpha2)) {
				unitGridCombat_.SetActiveWeapon(1);
				OnWeaponChanged?.Invoke(this, unitGridCombat_.GetActiveWeapon());
			}
		}

		public void TestTurnOver() {
			if(unitGridCombat_.GetActionPoints() <= 0) {
				// Cannot move or attack, turn over
				ForceTurnOver();
			}
		}

		private void DeselectUnit() {
			unitGridCombat_ = null;
			ResetMoveTiles();
			ResetArrowVisual();
			UnitEvent unitEvent = new UnitEvent() {
				unit = unitGridCombat_
			};
			OnUnitActionPointsChanged?.Invoke(this, unitEvent);
			state_ = State.Normal;
		}

		private void ForceTurnOver() {
			// Execute all unit actions and end turn
			turn_++;
			OnTurnEnded?.Invoke(this, turn_);
			DeselectUnit();
			ExecuteAllActions();
			ResetAllActionPoints();
		}

		private void ExecuteAllActions() {
			if(orderList_ == null || orderList_.Count == 0) {
				return;
			}

			orderList_.Sort((x, y) => x.GetInitiative().CompareTo(y.GetInitiative()));

			foreach(OrderObject order in orderList_) {
				order.ExecuteActions();
			}
		}

		private void ResetAllActionPoints() {
			foreach(CoreUnit unit in blueTeamList_) {
				unit.ResetActionPoints();
			}
			foreach(CoreUnit unit in redTeamList_) {
				unit.ResetActionPoints();
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
			GameController.Instance.GetMovementTilemap().SetAllTilemapSprite(
				MovementTilemap.TilemapObject.TilemapSprite.None
			);
		}

		/*
		 * This function generates the totalcost of the units actions.
		 * As params the function needs the current cost, the pathlength and the range of the attack.
		 * The function has the ability to add modifiers to the totalcost calculation.
		 */
		private int GenerateTotalCost(int cost, int pathLength, int attackRange) {
			int totalCost = cost + pathLength + attackRange;
			return totalCost;
		}

		private int GenerateInitiative(int cost, int pathLength, int attackRange) {
			int initiative = UnityEngine.Random.Range(1, 10);
			return initiative;
		}

		private OrderObject GetOrderObject(CoreUnit unit) {
			if(orderList_ == null || orderList_.Count == 0) {
				return null;
			}

			foreach(OrderObject order in orderList_) {
				if(order.GetUnit() == unit) {
					return order;
				}
			}

			return null;
		}

		private bool UnitsHaveActionsPoints() {
			foreach(CoreUnit unit in blueTeamList_) {
				if(unit.HasActionPoints()) {
					return true;
				}
			}
			return false;
		}

		public CoreUnit GetActiveUnit() {
			return unitGridCombat_;
		}

		// The methods `CalculatePoints` is from https://www.redblobgames.com/grids/line-drawing.html and adjusted accordingly.

		// Calculates the length between two Vector3's and returns N nodes between them.
		public List<Vector3> CalculatePoints(Vector3 p0, Vector3 p1) {
			List<Vector3> points = new List<Vector3>();
			// A cast to int is used here to make sure the variable has a whole number
			float diagonalLength = (int)Vector3.Distance(p0, p1);
			for(int step = 0; step <= diagonalLength; step++) {
				float pointOnLine = diagonalLength == 0 ? 0.0f : step / diagonalLength;
				points.Add(Vector3.Lerp(p0, p1, pointOnLine));
			}
			return points;
		}

		// If the start and end are not needed from the actionpath then it uses the selected player position and mouseposition for its calculation.
		private void SetArrowWithPath(Vector3 start, Vector3 end) {
			if(start == Vector3.zero && end == Vector3.zero) {
				start = unitGridCombat_.GetPosition();
				end = Utils.GetMouseWorldPosition();
			}
			currentPathUnit_ = GameController.Instance.gridPathfinding.GetPath(start, end);
			MovementTilemap arrowMap = GameController.Instance.GetArrowTilemap();
			Grid<Tilemap.Node> grid = GameController.Instance.GetGrid();
			List<Actions> actions = unitGridCombat_.LoadActions().GetQueue();
			int x = 0, y = 0;
			foreach(PathNode node in currentPathUnit_) {
				x = node.xPos;
				y = node.yPos;

				if(grid.GetGridObject(x, y).GetUnitGridCombat() != unitGridCombat_ && grid.GetGridObject(x, y) != grid.GetGridObject(start)) {
					if(grid.GetGridObject(x, y) != grid.GetGridObject(end)) {
						if((node.parent.xPos > x || node.parent.xPos < x) && node.parent.yPos == y) {
							arrowMap.SetRotation(x, y, 90f);
							arrowMap.SetTilemapSprite(x, y, MovementTilemap.TilemapObject.TilemapSprite.ArrowStraight);
						} 
						if((node.parent.yPos > y || node.parent.yPos < y) && node.parent.xPos == x) {
							arrowMap.SetRotation(x, y, 0f);
							arrowMap.SetTilemapSprite(x, y, MovementTilemap.TilemapObject.TilemapSprite.ArrowStraight);
						}
					} else {
						if(node.parent.xPos > x && node.parent.yPos == y) {
							arrowMap.SetRotation(x, y, 90f);
						} else if(node.parent.xPos < x && node.parent.yPos == y) {
							arrowMap.SetRotation(x, y, -90f);
						} else if(node.parent.xPos == x && node.parent.yPos > y) {
							arrowMap.SetRotation(x, y, 180f);
						} else if(node.parent.xPos == x && node.parent.yPos < y) {
							arrowMap.SetRotation(x, y, 0f);
						}
						arrowMap.SetTilemapSprite(x, y, MovementTilemap.TilemapObject.TilemapSprite.ArrowEnd);
					}

					if(node.parent.parent == null) {
						continue;
					}

					if((node.parent.parent.xPos == node.parent.xPos && node.parent.xPos < x 
						&& node.parent.parent.yPos > node.parent.yPos && node.parent.yPos == y)
						|| (node.parent.parent.xPos > node.parent.xPos && node.parent.xPos == x 
						&& node.parent.parent.yPos == node.parent.yPos && node.parent.yPos < y)) {
						arrowMap.SetRotation(node.parent.xPos, node.parent.yPos, 90f);
						arrowMap.SetTilemapSprite(node.parent.xPos, node.parent.yPos, MovementTilemap.TilemapObject.TilemapSprite.ArrowCorner);
					}
					if((node.parent.parent.xPos == node.parent.xPos && node.parent.xPos > x 
						&& node.parent.parent.yPos > node.parent.yPos && node.parent.yPos == y)
						|| (node.parent.parent.xPos < node.parent.xPos && node.parent.xPos == x 
						&& node.parent.parent.yPos == node.parent.yPos && node.parent.yPos < y)) {
						arrowMap.SetRotation(node.parent.xPos, node.parent.yPos, 180f);
						arrowMap.SetTilemapSprite(node.parent.xPos, node.parent.yPos, MovementTilemap.TilemapObject.TilemapSprite.ArrowCorner);
					}
					if((node.parent.parent.xPos == node.parent.xPos && node.parent.xPos < x 
						&& node.parent.parent.yPos < node.parent.yPos && node.parent.yPos == y)
						|| (node.parent.parent.xPos > node.parent.xPos && node.parent.xPos == x 
						&& node.parent.parent.yPos == node.parent.yPos && node.parent.yPos > y)) {
						arrowMap.SetRotation(node.parent.xPos, node.parent.yPos, 0f);
						arrowMap.SetTilemapSprite(node.parent.xPos, node.parent.yPos, MovementTilemap.TilemapObject.TilemapSprite.ArrowCorner);
					}
					if((node.parent.parent.xPos == node.parent.xPos && node.parent.xPos > x 
						&& node.parent.parent.yPos < node.parent.yPos && node.parent.yPos == y)
						|| (node.parent.parent.xPos < node.parent.xPos && node.parent.xPos == x 
						&& node.parent.parent.yPos == node.parent.yPos && node.parent.yPos > y)) {
						arrowMap.SetRotation(node.parent.xPos, node.parent.yPos, -90f);
						arrowMap.SetTilemapSprite(node.parent.xPos, node.parent.yPos, MovementTilemap.TilemapObject.TilemapSprite.ArrowCorner);
					}
				}
				if(actions.Count != 0 && node.parent != null) {
					
				}
			}
		}

		private void ResetArrowVisual() {
			GameController.Instance.GetArrowTilemap().SetAllTilemapSprite(MovementTilemap.TilemapObject.TilemapSprite.None);
		}
	}
}
