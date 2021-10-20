using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OperationBlackwell.UI {
	public class HUDController : MonoBehaviour {

		[SerializeField] private Text ActionPointsText_;

		private void Update() {
			ActionPointsText_.text = "Action Points: " + OperationBlackwell.Player.GridCombatSystem.Instance.GetActiveUnit().GetActionPoints();
		}

	}
}
