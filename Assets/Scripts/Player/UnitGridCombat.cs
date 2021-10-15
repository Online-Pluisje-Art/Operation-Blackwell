using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationBlackwell.Core;

namespace OperationBlackwell.Player {
    public class UnitGridCombat : MonoBehaviour, IGameObject {

        [SerializeField] private Team team;

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
            movePosition.SetMovePosition(targetPosition + new Vector3(1, 1), () => {
                state = State.Normal;
                onReachedPosition();
            });
        }

        public bool CanAttackUnit(UnitGridCombat unitGridCombat) {
            return Vector3.Distance(GetPosition(), unitGridCombat.GetPosition()) < 50f;
        }

        public void AttackUnit(UnitGridCombat unitGridCombat, Action onAttackComplete) {
            // state = State.Attacking;

            // ShootUnit(unitGridCombat, () => {
            //     if (!unitGridCombat.IsDead()) {
            //         ShootUnit(unitGridCombat, () => {
            //             if (!unitGridCombat.IsDead()) {
            //                 ShootUnit(unitGridCombat, () => {
            //                     if (!unitGridCombat.IsDead()) {
            //                         ShootUnit(unitGridCombat, () => {
            //                             state = State.Normal;
            //                             onAttackComplete();
            //                         });
            //                     } else { state = State.Normal; onAttackComplete(); }
            //                 });
            //             } else { state = State.Normal; onAttackComplete(); }
            //         });
            //     } else { state = State.Normal; onAttackComplete(); }
            // });
            onAttackComplete();
        }

        // private void ShootUnit(UnitGridCombat unitGridCombat, Action onShootComplete) {
        //     GetComponent<IMoveVelocity>().Disable();
        //     Vector3 attackDir = (unitGridCombat.GetPosition() - transform.position).normalized;
        //     //UtilsClass.ShakeCamera(.6f, .1f);
        //     GameHandler_GridCombatSystem.Instance.ScreenShake();

        //     characterBase.PlayShootAnimation(attackDir, (Vector3 vec) => {
        //         Shoot_Flash.AddFlash(vec);
        //         WeaponTracer.Create(vec, unitGridCombat.GetPosition() + UtilsClass.GetRandomDir() * UnityEngine.Random.Range(-2f, 4f));
        //         unitGridCombat.Damage(this, UnityEngine.Random.Range(4, 12));
        //     }, () => {
        //         characterBase.PlayIdleAnim();
        //         GetComponent<IMoveVelocity>().Enable();

        //         onShootComplete();
        //     });
        // }

        // public void Damage(UnitGridCombat attacker, int damageAmount) {
        //     Vector3 bloodDir = (GetPosition() - attacker.GetPosition()).normalized;
        //     Blood_Handler.SpawnBlood(GetPosition(), bloodDir);

        //     DamagePopup.Create(GetPosition(), damageAmount, false);
        //     healthSystem.Damage(damageAmount);
        //     if (healthSystem.IsDead()) {
        //         FlyingBody.Create(GameAssets.i.pfEnemyFlyingBody, GetPosition(), bloodDir);
        //         Destroy(gameObject);
        //     } else {
        //         // Knockback
        //         //transform.position += bloodDir * 5f;
        //     }
        // }

        public Vector3 GetPosition() {
            return transform.position;
        }

        public Team GetTeam() {
            return team;
        }

        public bool IsEnemy(IGameObject unitGridCombat) {
            return unitGridCombat.GetTeam() != team;
        }
    }
}
