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
		private int pathLength_;

		public EventHandler<EventArgs> OnUnitDeath;
		public EventHandler<UnitPositionEvent> OnUnitSelect;
		public EventHandler<UnitPositionEvent> OnUnitMove;

		public class UnitPositionEvent : EventArgs {
			public CoreUnit unit;
			public Vector3 position;
		}

		private enum State {
			Normal,
			Waiting
		}

		private void Awake() {
			Instance = this;
			state_ = State.Normal;
			OnUnitDeath += RemoveUnitOnDeath;
		}

		private void Start() {
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

			SelectNextActiveUnit();
			UpdateValidMovePositions();
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

		private void UpdateValidMovePositions() {
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

			int maxMoveDistance = unitGridCombat_.GetActionPoints() + 1;
			for(int x = unitX - maxMoveDistance; x <= unitX + maxMoveDistance; x++) {
				for(int y = unitY - maxMoveDistance; y <= unitY + maxMoveDistance; y++) {
					if(x < 0 || x >= grid.GetWidth() || y < 0 || y >= grid.GetHeight()) {
						continue;
					}
					if(GameController.Instance.grid.GetGridObject(x, y).GetUnitGridCombat() != null) {
						continue;
					}

					if(gridPathfinding.IsWalkable(x, y)) {
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

		private void Update() {
			switch(state_) {
				case State.Normal:
					if(Input.GetMouseButtonDown(0)) {
						Grid<Tilemap.Node> grid = GameController.Instance.GetGrid();
						Tilemap.Node gridObject = grid.GetGridObject(Utils.GetMouseWorldPosition());

						if(gridObject.GetIsValidMovePosition()) {
							// Valid Move Position

							if(unitGridCombat_.GetActionPoints() > 0) {
								state_ = State.Waiting;

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
									UpdateValidMovePositions();
									TestTurnOver();
								});
							}
						}

						// Check if clicking on a unit position
						if(gridObject.GetUnitGridCombat() != null && unitGridCombat_.GetActionPoints() > 0) {
							// Clicked on top of a Unit
							if(unitGridCombat_.IsEnemy(gridObject.GetUnitGridCombat())) {
								// Clicked on an Enemy of the current unit
								if(unitGridCombat_.CanAttackUnit(gridObject.GetUnitGridCombat())) {
									// Can Attack Enemy
									// 3 is chosen as a placeholder for the attack cost
									if(unitGridCombat_.GetActionPoints() >= 3) {
										// Attack Enemy
										state_ = State.Waiting;
										unitGridCombat_.SetActionPoints(unitGridCombat_.GetActionPoints() - 3);
										unitGridCombat_.AttackUnit(gridObject.GetUnitGridCombat(), () => {
											state_ = State.Normal;
											UpdateValidMovePositions();
											TestTurnOver();
										});
									}
								} else {
									// Cannot attack enemy
								}
								break;
							} else {
								// Not an enemy
							}
						} else {
							// No unit here
						}
					}

					if(Input.GetKeyDown(KeyCode.Space)) {
						ForceTurnOver();
					}
					break;
				case State.Waiting:
					break;
				default:
					break;
			}
		}

		private void TestTurnOver() {
			if(unitGridCombat_.GetActionPoints() <= 0) {
				// Cannot move or attack, turn over
				ForceTurnOver();
			}
		}

		private void ForceTurnOver() {
			unitGridCombat_.ResetActionPoints();
			SelectNextActiveUnit();
			UpdateValidMovePositions();
		}

		public CoreUnit GetActiveUnit() {
			return unitGridCombat_;
		}
	}
}
