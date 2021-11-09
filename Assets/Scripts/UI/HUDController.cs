using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OperationBlackwell.Core;

namespace OperationBlackwell.UI {
	public class HUDController : MonoBehaviour {

		[SerializeField] private Text ActionPointsText_;
		[SerializeField] private Text WeaponText_;

		private void Awake() {
			GridCombatSystem.Instance.OnUnitActionPointsChanged += UpdateActionPoints;
			GridCombatSystem.Instance.OnWeaponChanged += UpdateWeapon;
		}

		private void UpdateActionPoints(object sender, GridCombatSystem.UnitEvent args) {
			ActionPointsText_.text = "Action Points: " + args.unit.GetActionPoints().ToString();
		}

		private void UpdateWeapon(object sender, string name) {
			WeaponText_.text = "Current Weapon: " + name;
		}
	}
}
