using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationBlackwell.Core;

namespace OperationBlackwell.Player {
    public class GridCombatSystem : MonoBehaviour {

        [SerializeField] private UnitGridCombat[] unitGridCombatArray;

        private State state;
        private UnitGridCombat unitGridCombat;
        private List<UnitGridCombat> blueTeamList;
        private List<UnitGridCombat> redTeamList;
        private int blueTeamActiveUnitIndex;
        private int redTeamActiveUnitIndex;
        private bool canMoveThisTurn;
        private bool canAttackThisTurn;

        private enum State {
            Normal,
            Waiting
        }

        private void Awake() {
            state = State.Normal;
        }

        private void Start() {
            blueTeamList = new List<UnitGridCombat>();
            redTeamList = new List<UnitGridCombat>();
            blueTeamActiveUnitIndex = -1;
            redTeamActiveUnitIndex = -1;

            // Set all UnitGridCombat on their GridPosition
            foreach (UnitGridCombat unitGridCombat in unitGridCombatArray) {
                // GameController.Instance.GetGrid().NodeFromWorldPoint(unitGridCombat.GetPosition())
                    // .SetUnitGridCombat(unitGridCombat);

                if (unitGridCombat.GetTeam() == Team.Blue) {
                    blueTeamList.Add(unitGridCombat);
                } else {
                    redTeamList.Add(unitGridCombat);
                }
            }

            SelectNextActiveUnit();
            UpdateValidMovePositions();
        }

        private void SelectNextActiveUnit() {
            if (unitGridCombat == null || unitGridCombat.GetTeam() == Team.Red) {
                unitGridCombat = GetNextActiveUnit(Team.Blue);
            } else {
                unitGridCombat = GetNextActiveUnit(Team.Red);
            }

            // GameController.Instance.SetCameraFollowPosition(unitGridCombat.GetPosition());
            canMoveThisTurn = true;
            canAttackThisTurn = true;
        }

        private UnitGridCombat GetNextActiveUnit(Team team) {
            if (team == Team.Blue) {
                blueTeamActiveUnitIndex = (blueTeamActiveUnitIndex + 1) % blueTeamList.Count;
                if (blueTeamList[blueTeamActiveUnitIndex] == null) { // || blueTeamList[blueTeamActiveUnitIndex].IsDead()) {
                    // Unit is Dead, get next one
                    return GetNextActiveUnit(team);
                } else {
                    return blueTeamList[blueTeamActiveUnitIndex];
                }
            } else {
                redTeamActiveUnitIndex = (redTeamActiveUnitIndex + 1) % redTeamList.Count;
                if (redTeamList[redTeamActiveUnitIndex] == null) { //|| redTeamList[redTeamActiveUnitIndex].IsDead()) {
                    // Unit is Dead, get next one
                    return GetNextActiveUnit(team);
                } else {
                    return redTeamList[redTeamActiveUnitIndex];
                }
            }
        }

        private void UpdateValidMovePositions() {
            Grid<Tilemap.Node> grid = GameController.Instance.GetGrid();
            GridPathfinding gridPathfinding = GameController.Instance.gridPathfinding;

            // Get Unit Grid Position X, Y
            grid.GetXY(unitGridCombat.GetPosition(), out int unitX, out int unitY);

            // // Set entire Tilemap to Invisible
            // GameController.Instance.GetMovementTilemap().SetAllTilemapSprite(
            //     MovementTilemap.TilemapObject.TilemapSprite.None
            // );

            // Reset Entire Grid ValidMovePositions
            for (int x = 0; x < grid.gridSizeX; x++) {
                for (int y = 0; y < grid.gridSizeY; y++) {
                    grid.NodeFromWorldPoint(new Vector3(x, y)).SetIsValidMovePosition(false);
                }
            }

            int maxMoveDistance = 5;
            for (int x = unitX - maxMoveDistance; x <= unitX + maxMoveDistance; x++) {
                for (int y = unitY - maxMoveDistance; y <= unitY + maxMoveDistance; y++) {
                    if (gridPathfinding.IsWalkable(x, y)) {
                        // Position is Walkable
                        if (gridPathfinding.HasPath(unitX, unitY, x, y)) {
                            // There is a Path
                            if (gridPathfinding.GetPath(unitX, unitY, x, y).Count <= maxMoveDistance) {
                                // Path within Move Distance

                                // // Set Tilemap Tile to Move
                                // GameController.Instance.GetMovementTilemap().SetTilemapSprite(
                                //     x, y, MovementTilemap.TilemapObject.TilemapSprite.Move
                                // );

                                grid.NodeFromWorldPoint(new Vector3(x, y)).SetIsValidMovePosition(true);
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
            switch (state) {
                case State.Normal:
                    if (Input.GetMouseButtonDown(0)) {
                        Grid<Tilemap.Node> grid = GameController.Instance.GetGrid();
                        Tilemap.Node gridObject = grid.NodeFromWorldPoint(Utils.GetMouseWorldPosition());

                        // Check if clicking on a unit position
                        if (gridObject.GetUnitGridCombat() != null) {
                            // // Clicked on top of a Unit
                            // if (unitGridCombat.IsEnemy(gridObject.GetUnitGridCombat())) {
                            //     // Clicked on an Enemy of the current unit
                            //     if (unitGridCombat.CanAttackUnit(gridObject.GetUnitGridCombat())) {
                            //         // Can Attack Enemy
                            //         if (canAttackThisTurn) {
                            //             canAttackThisTurn = false;
                            //             // Attack Enemy
                            //             state = State.Waiting;
                            //             unitGridCombat.AttackUnit(gridObject.GetUnitGridCombat(), () => {
                            //                 state = State.Normal;
                            //                 TestTurnOver();
                            //             });
                            //         }
                            //     } else {
                            //         // Cannot attack enemy
                            //         CodeMonkey.CMDebug.TextPopupMouse("Cannot attack!");
                            //     }
                            //     break;
                            // } else {
                            //     // Not an enemy
                            // }
                        } else {
                            // No unit here
                        }

                        if (gridObject.GetIsValidMovePosition()) {
                            // Valid Move Position

                            if (canMoveThisTurn) {
                                canMoveThisTurn = false;

                                state = State.Waiting;

                                // // Set entire Tilemap to Invisible
                                // GameController.Instance.GetMovementTilemap().SetAllTilemapSprite(
                                //     MovementTilemap.TilemapObject.TilemapSprite.None
                                // );

                                // Remove Unit from current Grid Object
                                grid.NodeFromWorldPoint(unitGridCombat.GetPosition()).ClearUnitGridCombat();
                                // Set Unit on target Grid Object
                                gridObject.SetUnitGridCombat(unitGridCombat);

                                unitGridCombat.MoveTo(Utils.GetMouseWorldPosition(), () => {
                                    state = State.Normal;
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
            if (!canMoveThisTurn && !canAttackThisTurn) {
                // Cannot move or attack, turn over
                ForceTurnOver();
            }
        }

        private void ForceTurnOver() {
            SelectNextActiveUnit();
            UpdateValidMovePositions();
        }
    }
}
