using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OperationBlackwell.Core;

namespace OperationBlackwell.UI {
	public class HUDController : MonoBehaviour {

		[SerializeField] private Text ActionPointsText_;

		private void Awake() {
			GridCombatSystem.Instance.OnUnitActionPointsChanged += UpdateActionPoints;
		}

		private void UpdateActionPoints(object sender, GridCombatSystem.UnitEvent args) {
			ActionPointsText_.text = "Action Points: " + args.unit.GetActionPoints().ToString();
		}
	}
}
