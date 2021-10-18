using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationBlackwell.Core;

namespace OperationBlackwell.Player {
    public class UnitGridCombat : MonoBehaviour, IGameObject {

        [SerializeField] private Team team;
        [SerializeField] private int actionPoints_;
        [SerializeField] private int maxActionPoints_;
        
        private PlayerBase characterBase;
        private GameObject selectedGameObject;
        private MovePositionPathfinding movePosition;
        private State state;

        private enum State {
            Normal,
            Moving,
            Attacking
        }

        private void Awake() {
            characterBase = GetComponent<PlayerBase>();
            selectedGameObject = transform.Find("Selected").gameObject;
            movePosition = GetComponent<MovePositionPathfinding>();
            //SetSelectedVisible(false);
            state = State.Normal;
        }

        private void Update() {
            switch (state) {
                case State.Normal:
                    break;
                case State.Moving:
                    break;
                case State.Attacking:
                    break;
            }
        }

        public void SetSelectedVisible(bool visible) {
            selectedGameObject.SetActive(visible);
        }

        public void MoveTo(Vector3 targetPosition, Action onReachedPosition) {
            state = State.Moving;
            movePosition.SetMovePosition(targetPosition, () => {
                state = State.Normal;
                onReachedPosition();
            });
        }

        public Vector3 GetPosition() {
            return transform.position;
        }

        public Team GetTeam() {
            return team;
        }

        public bool IsEnemy(IGameObject unitGridCombat) {
            return unitGridCombat.GetTeam() != team;
        }

        public void SetActionPoints(int actionPoints) {
            actionPoints_ = actionPoints;
        }

        public int GetActionPoints() {
            return actionPoints_;
        }

        public void ResetActionPoints() {
            actionPoints_ = maxActionPoints_;
        }
    }
}
