using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationBlackwell.Core;

namespace OperationBlackwell.Player {
	public class GridCombatSystem : MonoBehaviour {
		[SerializeField] private UnitGridCombat[] unitGridCombatArray_;

		private State state_;
		private UnitGridCombat unitGridCombat_;
		private List<UnitGridCombat> blueTeamList_;
		private List<UnitGridCombat> redTeamList_;
		private int blueTeamActiveUnitIndex_;
		private int redTeamActiveUnitIndex_;

		private enum State {
			Normal,
			Waiting
		}

		private void Awake() {
			state_ = State.Normal;
		}

		private void Start() {
			blueTeamList_ = new List<UnitGridCombat>();
			redTeamList_ = new List<UnitGridCombat>();
			blueTeamActiveUnitIndex_ = -1;
			redTeamActiveUnitIndex_ = -1;

			// Set all UnitGridCombat on their GridPosition
			foreach (UnitGridCombat unitGridCombat_ in unitGridCombatArray_) {
				GameController.Instance.GetGrid().GetGridObject(unitGridCombat_.GetPosition())
					.SetUnitGridCombat(unitGridCombat_);

				if (unitGridCombat_.GetTeam() == Team.Blue) {
					blueTeamList_.Add(unitGridCombat_);
				} else {
					redTeamList_.Add(unitGridCombat_);
				}
			}

			SelectNextActiveUnit();
			UpdateValidMovePositions();
		}

		private void SelectNextActiveUnit() {
			if (unitGridCombat_ == null || unitGridCombat_.GetTeam() == Team.Red) {
				unitGridCombat_ = GetNextActiveUnit(Team.Blue);
			} else {
				unitGridCombat_ = GetNextActiveUnit(Team.Red);
			}

			// GameController.Instance.SetCameraFollowPosition(unitGridCombat_.GetPosition());
		}

		private UnitGridCombat GetNextActiveUnit(Team team) {
			if (team == Team.Blue) {
				blueTeamActiveUnitIndex_ = (blueTeamActiveUnitIndex_ + 1) % blueTeamList_.Count;
				if (blueTeamList_[blueTeamActiveUnitIndex_] == null) {// || blueTeamList_[blueTeamActiveUnitIndex_].IsDead()) {
					// Unit is Dead, get next one
					return GetNextActiveUnit(team);
				} else {
					return blueTeamList_[blueTeamActiveUnitIndex_];
				}
			} else {
				redTeamActiveUnitIndex_ = (redTeamActiveUnitIndex_ + 1) % redTeamList_.Count;
				if (redTeamList_[redTeamActiveUnitIndex_] == null) { //|| redTeamList_[redTeamActiveUnitIndex_].IsDead()) {
					// Unit is Dead, get next one
					return GetNextActiveUnit(team);
				} else {
					return redTeamList_[redTeamActiveUnitIndex_];
				}
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
			for (int x = 0; x < grid.GetWidth(); x++) {
				for (int y = 0; y < grid.GetHeight(); y++) {
					grid.GetGridObject(x, y).SetIsValidMovePosition(false);
				}
			}

			int maxMoveDistance = unitGridCombat_.GetActionPoints() + 1;
			for (int x = unitX - maxMoveDistance; x <= unitX + maxMoveDistance; x++) {
				for (int y = unitY - maxMoveDistance; y <= unitY + maxMoveDistance; y++) {
					if (gridPathfinding.IsWalkable(x, y)) {
						// Position is Walkable
						if (gridPathfinding.HasPath(unitX, unitY, x, y)) {
							// There is a Path
							if (gridPathfinding.GetPath(unitX, unitY, x, y).Count <= maxMoveDistance) {
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
			switch (state_) {
				case State.Normal:
					if (Input.GetMouseButtonDown(0)) {
						Grid<Tilemap.Node> grid = GameController.Instance.GetGrid();
						Tilemap.Node gridObject = grid.GetGridObject(Utils.GetMouseWorldPosition());

						if (gridObject.GetIsValidMovePosition()) {
							// Valid Move Position

							if (unitGridCombat_.GetActionPoints() > 0) {
								state_ = State.Waiting;

								// Set entire Tilemap to Invisible
								GameController.Instance.GetMovementTilemap().SetAllTilemapSprite(
									MovementTilemap.TilemapObject.TilemapSprite.None
								);

								// Remove Unit from current Grid Object
								grid.GetGridObject(unitGridCombat_.GetPosition()).ClearUnitGridCombat();
								// Set Unit on target Grid Object
								gridObject.SetUnitGridCombat(unitGridCombat_);

								unitGridCombat_.MoveTo(Utils.GetMouseWorldPosition(), () => {
									state_ = State.Normal;
									unitGridCombat_.SetActionPoints(unitGridCombat_.GetActionPoints() - 1);
									UpdateValidMovePositions();
									TestTurnOver();
								});
							}
						}
					}

					if (Input.GetKeyDown(KeyCode.Space)) {
						ForceTurnOver();
					}
					break;
				case State.Waiting:
					break;
			}
		}

		private void TestTurnOver() {
			if (unitGridCombat_.GetActionPoints() <= 0) {
				// Cannot move or attack, turn over
				ForceTurnOver();
			}
		}

		private void ForceTurnOver() {
			unitGridCombat_.ResetActionPoints();
			SelectNextActiveUnit();
			UpdateValidMovePositions();
		}
	}
}
