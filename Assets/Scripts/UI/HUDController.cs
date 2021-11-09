using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OperationBlackwell.Core;

namespace OperationBlackwell.UI {
	public class HUDController : MonoBehaviour {

		[SerializeField] private Text actionPointsText_;
		[SerializeField] private Text weaponText_;
		[SerializeField] private Text turnCounterText_;

		private void Awake() {
			GridCombatSystem.Instance.OnUnitActionPointsChanged += UpdateActionPoints;
			GridCombatSystem.Instance.OnWeaponChanged += UpdateWeapon;
			GridCombatSystem.Instance.OnTurnEnded += UpdateTurnCounter;
		}

		private void UpdateActionPoints(object sender, GridCombatSystem.UnitEvent args) {
			actionPointsText_.text = "Action Points: " + args.unit.GetActionPoints().ToString();
		}

		private void UpdateWeapon(object sender, string name) {
			weaponText_.text = "Current Weapon: " + name;
		}

		private void UpdateActionPoints(object sender, GridCombatSystem.UnitEvent args) {
			if(args.unit != null) {
				actionPointsText_.text = "Action Points: " + args.unit.GetActionPoints().ToString();
			} else {
				actionPointsText_.text = "";
			}
		}

		private void UpdateTurnCounter(object sender, int turn) {
			turnCounterText_.text = "Turn: " + turn.ToString();
		}
	}
}
