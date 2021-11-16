using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OperationBlackwell.Core;

namespace OperationBlackwell.UI {
	public class HUDController : MonoBehaviour {

		[SerializeField] private Text actionPointsText_;
		[SerializeField] private Text weaponText_;

		private void Awake() {
			GridCombatSystem.Instance.OnUnitActionPointsChanged += UpdateActionPoints;
			GridCombatSystem.Instance.OnWeaponChanged += UpdateWeapon;
		}

		private void UpdateActionPoints(object sender, GridCombatSystem.UnitEvent args) {
			actionPointsText_.text = "Action Points: " + args.unit.GetActionPoints().ToString();
		}

		private void UpdateWeapon(object sender, string name) {
			weaponText_.text = "Current Weapon: " + name;
		}
	}
}
