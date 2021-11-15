using System.Collections;
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

			OnUnitSelect?.Invoke(this, new UnitPositionEvent() {
				unit = blueTeamList_[0],
				position = blueTeamList_[0].GetPosition()
			});

			OnTurnEnded?.Invoke(this, turn_);
			// SelectNextActiveUnit();
			// UpdateValidMovePositions();
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
			gridObject.SetUnitGridCombat(null);
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

		public void UpdateValidMovePositions() {
			Grid<Tilemap.Node> grid = GameController.Instance.GetGrid();
			GridPathfinding gridPathfinding = GameController.Instance.gridPathfinding;

			// Get Unit Grid Position X, Y
			grid.GetXY(unitGridCombat_.GetPosition(), out int unitX, out int unitY);

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

			foreach(Tilemap.Node node in grid.GetAllGridObjects()) {
				if(node.walkable) {
					gridPathfinding.SetWalkable(node.gridX, node.gridY, true);
				} else {
					gridPathfinding.SetWalkable(node.gridX, node.gridY, false);
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
			if(gridObject != null) {
				CoreUnit unit = gridObject.GetUnitGridCombat();
				if(unit != null && unitGridCombat_ != null && unitGridCombat_.CanAttackUnit(unit)
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

					ResetArrowVisual();
					// Set arrow to target position
					if(gridObject.GetIsValidMovePosition()) {
						SetArrowWithPath();
					}
					
					unit = gridObject.GetUnitGridCombat();
					if(unit != null && unit.GetTeam() == Team.Blue) {
						GameController.Instance.GetSelectorTilemap().SetTilemapSprite(
							gridObject.gridX, gridObject.gridY, MovementTilemap.TilemapObject.TilemapSprite.Move
						);
						if(Input.GetMouseButtonDown((int)MouseButtons.Leftclick)) {
							if(unit != null && unit.GetTeam() == Team.Blue) {
								OnUnitSelect?.Invoke(this, new UnitPositionEvent() {
									unit = unit,
									position = unit.GetPosition()
								});
								unitGridCombat_ = gridObject.GetUnitGridCombat();
								state_ = State.UnitSelected;
							}
						}
					} else {
						GameController.Instance.GetSelectorTilemap().SetTilemapSprite(
							gridObject.gridX, gridObject.gridY, MovementTilemap.TilemapObject.TilemapSprite.Move
						);
					}
					// Save the actions for the unit
					// Update the unit's action points
					// Wait until user selects new unit
					// OldUnitStuff();
					break;
				case State.UnitSelected:
					UnitEvent unitEvent = new UnitEvent() {
						unit = unitGridCombat_
					};
					OnUnitActionPointsChanged?.Invoke(this, unitEvent);
					
					//load the units actions

					UpdateValidMovePositions();
					
					grid = GameController.Instance.GetGrid();
					gridObject = grid.GetGridObject(Utils.GetMouseWorldPosition());
					
					if(gridObject == null) {
						return;
					}

					GameController.Instance.GetSelectorTilemap().SetTilemapSprite(
						gridObject.gridX, gridObject.gridY, MovementTilemap.TilemapObject.TilemapSprite.Move
					);

					unit = gridObject.GetUnitGridCombat();
					if(unit != null && unit.GetTeam() == Team.Blue && unit != unitGridCombat_) {
						if(Input.GetMouseButtonDown((int)MouseButtons.Leftclick)) {
							if(unit != null && unit.GetTeam() == Team.Blue) {
								OnUnitSelect?.Invoke(this, new UnitPositionEvent() {
									unit = unit,
									position = unit.GetPosition()
								});
								unitGridCombat_ = gridObject.GetUnitGridCombat();
								state_ = State.UnitSelected;
							}
						}
					} else if(gridObject.GetIsValidMovePosition()) {
						// Draw arrow acording to pathfinding path
						if(Input.GetMouseButtonDown((int)MouseButtons.Rightclick)) {
							// Save the actions for the unit
							if(gridObject.GetIsValidMovePosition()) {
								// Valid Move Position
								if(unitGridCombat_.GetActionPoints() > 0) {
									state_ = State.Waiting;

									// Set entire Tilemap to Invisible
									GameController.Instance.GetMovementTilemap().SetAllTilemapSprite(
										MovementTilemap.TilemapObject.TilemapSprite.None
									);
									CursorController.Instance.SetActiveCursorType(CursorController.CursorType.Arrow);

									// Remove Unit from current Grid Object
									grid.GetGridObject(unitGridCombat_.GetPosition()).ClearUnitGridCombat();
									// Set Unit on target Grid Object
									gridObject.SetUnitGridCombat(unitGridCombat_);

									pathLength_ = GameController.Instance.gridPathfinding.GetPath(unitGridCombat_.GetPosition(), Utils.GetMouseWorldPosition()).Count - 1;

									unitGridCombat_.MoveTo(Utils.GetMouseWorldPosition(), () => {
										if(unitGridCombat_.GetActionPoints() - pathLength_ > 0) {
											OnUnitMove?.Invoke(this, new UnitPositionEvent() {
												unit = unitGridCombat_,
												position = unitGridCombat_.GetPosition()
											});
										}
										if(unitGridCombat_.GetActionPoints() - pathLength_ == 0){
											state_ = State.EndingTurn;
										} else {
											state_ = State.UnitSelected;
										}
										unitGridCombat_.SetActionPoints(unitGridCombat_.GetActionPoints() - pathLength_);
										UnitEvent unitEvent = new UnitEvent() {
											unit = unitGridCombat_
										};
										OnUnitActionPointsChanged?.Invoke(this, unitEvent);
									});
								}
							}
						} else if(Input.GetMouseButtonDown((int)MouseButtons.Leftclick)) {
							DeselectUnit();
						}
					} else if(unitGridCombat_.CanAttackUnit(unit)) {
						if(Input.GetMouseButtonDown((int)MouseButtons.Rightclick)) {
							// Can Attack Enemy
							int attackCost = unitGridCombat_.GetAttackCost();
							if(unitGridCombat_.GetActionPoints() >= attackCost) {
								// Attack Enemy
								// state_ = State.Waiting;
								unitGridCombat_.SetActionPoints(unitGridCombat_.GetActionPoints() - attackCost);
								unitGridCombat_.AttackUnit(unit, () => {
									if(unitGridCombat_.GetActionPoints() == 0){
										state_ = State.EndingTurn;
									} else {
										state_ = State.UnitSelected;
									}
									UnitEvent unitEvent = new UnitEvent() {
										unit = unitGridCombat_
									};
									OnUnitActionPointsChanged?.Invoke(this, unitEvent);
								});
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
			UnitEvent unitEvent = new UnitEvent() {
				unit = unitGridCombat_
			};
			OnUnitActionPointsChanged?.Invoke(this, unitEvent);
			ResetMoveTiles();
			state_ = State.Normal;
		}

		private void ForceTurnOver() {
			// Execute all unit actions and end turn
			turn_++;
			OnTurnEnded?.Invoke(this, turn_);
			DeselectUnit();
			ResetAllActionPoints();
			ResetMoveTiles();
		}

		private void ResetAllActionPoints() {
			foreach(CoreUnit unit in unitGridCombatArray_) {
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

		private void SetArrowWithPath() {
			currentPathUnit_ = GameController.Instance.gridPathfinding.GetPath(unitGridCombat_.GetPosition(), Utils.GetMouseWorldPosition());
			MovementTilemap arrowMap = GameController.Instance.GetArrowTilemap();
			Grid<Tilemap.Node> grid = GameController.Instance.GetGrid();
			int x = 0, y = 0;
			foreach(PathNode node in currentPathUnit_) {
				x = node.xPos;
				y = node.yPos;

				if(grid.GetGridObject(x, y).GetUnitGridCombat() != unitGridCombat_) {
					if(grid.GetGridObject(x, y) != grid.GetGridObject(Utils.GetMouseWorldPosition())) {
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
			}
		}

		private void ResetArrowVisual() {
			GameController.Instance.GetArrowTilemap().SetAllTilemapSprite(MovementTilemap.TilemapObject.TilemapSprite.None);
		}
		
		private void OldUnitStuff() {
			Grid<Tilemap.Node> grid = GameController.Instance.GetGrid();
			Tilemap.Node gridObject = grid.GetGridObject(Utils.GetMouseWorldPosition());

			if(gridObject.GetUnitGridCombat() != null && unitGridCombat_.CanAttackUnit(gridObject.GetUnitGridCombat())
				&& gridObject.GetUnitGridCombat() != unitGridCombat_ && gridObject.GetUnitGridCombat().GetTeam() != unitGridCombat_.GetTeam()) {
				CursorController.Instance.SetActiveCursorType(CursorController.CursorType.Attack);
			} else if(gridObject.GetIsValidMovePosition()) {
				CursorController.Instance.SetActiveCursorType(CursorController.CursorType.Move);
			} else {
				CursorController.Instance.SetActiveCursorType(CursorController.CursorType.Arrow);
			}

			if(Input.GetMouseButtonDown((int)MouseButtons.Leftclick)) {

				if(gridObject.GetIsValidMovePosition()) {
					// Valid Move Position

					if(unitGridCombat_.GetActionPoints() > 0) {
						// state_ = State.Waiting;

						// Set entire Tilemap to Invisible
						GameController.Instance.GetMovementTilemap().SetAllTilemapSprite(
							MovementTilemap.TilemapObject.TilemapSprite.None
						);

						// Remove Unit from current Grid Object
						grid.GetGridObject(unitGridCombat_.GetPosition()).ClearUnitGridCombat();
						// Set Unit on target Grid Object
						gridObject.SetUnitGridCombat(unitGridCombat_);

						pathLength_ = GameController.Instance.gridPathfinding.GetPath(unitGridCombat_.GetPosition(), Utils.GetMouseWorldPosition()).Count - 1;

						unitGridCombat_.MoveTo(Utils.GetMouseWorldPosition(), () => {
							state_ = State.Normal;
							if(unitGridCombat_.GetActionPoints() - pathLength_ > 0) {
								OnUnitMove?.Invoke(this, new UnitPositionEvent() {
									unit = unitGridCombat_,
									position = Utils.GetMouseWorldPosition()
								});
							}
							unitGridCombat_.SetActionPoints(unitGridCombat_.GetActionPoints() - pathLength_);
							UnitEvent unitEvent = new UnitEvent() {
								unit = unitGridCombat_
							};
							OnUnitActionPointsChanged?.Invoke(this, unitEvent);
							UpdateValidMovePositions();
							TestTurnOver();
						});
					}
				}

				// Check if clicking on a unit position
				if(gridObject.GetUnitGridCombat() != null) {
					// Clicked on top of a Unit
					if(unitGridCombat_.CanAttackUnit(gridObject.GetUnitGridCombat())) {
						// Can Attack Enemy
						int attackCost = unitGridCombat_.GetAttackCost();
						if(unitGridCombat_.GetActionPoints() >= attackCost) {
							// Attack Enemy
							// state_ = State.Waiting;
							unitGridCombat_.SetActionPoints(unitGridCombat_.GetActionPoints() - attackCost);
							UnitEvent unitEvent = new UnitEvent() {
								unit = unitGridCombat_
							};
							OnUnitActionPointsChanged?.Invoke(this, unitEvent);
							unitGridCombat_.AttackUnit(gridObject.GetUnitGridCombat(), () => {
								state_ = State.Normal;
								UpdateValidMovePositions();
								TestTurnOver();
							});
						}
					} else {
						// Cannot attack enemy
					}
			} else {
					// No unit here
				}
			}

			if(Input.GetKeyDown(KeyCode.Space)) {
				ForceTurnOver();
			}
		}
	}
}
